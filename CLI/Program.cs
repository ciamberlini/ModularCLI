using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.Windsor;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Cli.Model;
using CLI.Properties;
using CLI.Shared;
using log4net.Config;

namespace Cli
{
    class Program
    {
        private static readonly IWindsorContainer Container = new WindsorContainer();
        private static ICliLogger _log;
        private static MenuPage _currentMenu;

        static void Main(string[] args)
        {
            // Initialize log4net
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
            Container.AddFacility<LoggingFacility>(f => f.UseLog4Net());

            // Inject container in the container :-)
            Container.Register(
                Component.For<IWindsorContainer>()
                        .Instance(Container)
                );

            // Register log object implementation
            Container.Register(
                Component.For<ICliLogger>()
                        .ImplementedBy<CliLogger>()
                        .LifestyleSingleton()
                );
            _log = Container.Resolve<ICliLogger>();

            // Discovery modules in each folder specified in app.config (comma separated)
            foreach (string foldername in Settings.Default.ModulesFolder.Split(','))
            {
                Container.Register(
                    Classes.FromAssemblyInDirectory(new AssemblyFilter(foldername))
                            .BasedOn<ICli>()
                            .WithServices(typeof(ICli))
                            .LifestyleSingleton()
                );
            }

            // If no parameters was passed in command line create and show console menu
            if (args.Length == 0)
            {
                MenuPage assemblyPage = new MenuPage("Main Menu");
                foreach (ICli cli in Container.ResolveAll<ICli>())
                {
                    // Create objects for assembly menu
                    Type cliType = cli.GetType();
                    MenuItem homeMenuItem = (from m in assemblyPage.MenuItems
                        where m.MenuString == cliType.Assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title
                        select m).FirstOrDefault();
                    if (homeMenuItem == null)
                    {
                        var name = cliType.Assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;
                        homeMenuItem = new MenuItem
                        {
                            MenuString = name,
                            MenuItemType = EnumMenuType.Assembly
                        };
                        assemblyPage.MenuItems.Add(homeMenuItem);
                    }

                    // Create objects for class menu
                    if (homeMenuItem.ChildPage == null)
                    {
                        MenuPage classPage = new MenuPage(homeMenuItem.MenuString);
                        homeMenuItem.ChildPage = classPage;
                        classPage.ParentPage = assemblyPage;
                    }
                    MenuItem menuClass = new MenuItem
                    {
                        MenuString = cliType.GetCustomAttribute<ClassAttribute>().Name,
                        MenuItemType = EnumMenuType.Class
                    };
                    homeMenuItem.ChildPage.MenuItems.Add(menuClass);

                    // Create objects for methods menu
                    if (menuClass.ChildPage != null) continue;
                    MenuPage methodPage = new MenuPage(cliType.GetCustomAttribute<ClassAttribute>().Name);
                    menuClass.ChildPage = methodPage;
                    methodPage.ParentPage = homeMenuItem.ChildPage;
                    foreach (MethodInfo method in cliType.GetMethods())
                    {
                        if (method.GetCustomAttribute<MethodAttribute>() == null) continue;
                        MenuItem methodItem = new MenuItem
                        {
                            MenuString = method.GetCustomAttribute<MethodAttribute>().Name,
                            AssemblyName = cliType.FullName,
                            MethodName = method.Name,
                            MenuItemType = EnumMenuType.Method
                        };
                        methodPage.MenuItems.Add(methodItem);
                    }
                }

                // Show menu objects in the console
                ShowConsoleMenu(assemblyPage);
            }
            else
            {
                // resolve assembly by reflection
                string module = args.FirstOrDefault(arg => arg.ToLower().StartsWith("-module:"));
                ICli classInstance = module == null ? null : Container.Resolve<ICli>(module.Substring(8));

                if (classInstance == null)
                {
                    _log.Error("Missing -module parameter");
                    return;
                }

                // resolve method by reflection
                string method = args.FirstOrDefault(arg => arg.ToLower().StartsWith("-method:"));
                var classMethod = method == null ? null : classInstance.GetType().GetMethods().FirstOrDefault(m => m.GetCustomAttribute<MethodAttribute>().CmdLine == method.Substring(8));

                if (classMethod == null)
                {
                    _log.Error("Missing -method parameter");
                    return;
                }

                // resolve params by reflection
                List<object> parameters = new List<object>();
                parameters.AddRange(from arg in args from ParameterInfo parameter in classMethod.GetParameters()
                                    where parameter.GetCustomAttribute<ParamAttribute>() != null
                                    where parameter.GetCustomAttribute<ParamAttribute>().CmdLine != null
                                    where parameter.GetCustomAttribute<ParamAttribute>().CmdLine.ToLowerInvariant() == arg.Substring(0, arg.LastIndexOf(':')).ToLowerInvariant()
                                    select Convert.ChangeType(arg.Substring(arg.LastIndexOf(':') + 1), parameter.ParameterType));

                ExecuteMethod(classInstance, classMethod, parameters.ToArray());
            }

            // Disposing container
            Container.Dispose();
        }
        static void ShowConsoleMenu(MenuPage menuPage)
        {
            _currentMenu = menuPage;

            Console.Clear();
            Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
            Console.WriteLine(" ModularCLI - {0}", menuPage.Title);
            Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
            Console.WriteLine();

            int index = 1;
            foreach (MenuItem item in menuPage.MenuItems)
            {
                Console.WriteLine(" {0,-2} - {1,-50} ", index.ToString("00"), item.MenuString);
                index ++;
            }

            Console.WriteLine();
            Console.Write("ESC: Go Back | Menu : ");

            StringBuilder buffer = new StringBuilder();
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter && info.Key != ConsoleKey.Escape)
            {
                Console.Write(info.KeyChar);
                buffer.Append(info.KeyChar);
                info = Console.ReadKey(true);
            }

            string result = buffer.ToString();

            if (info.Key == ConsoleKey.Enter)
            {
                try
                {
                    int output = int.Parse(result);
                    if (menuPage.MenuItems[output - 1].MenuItemType != EnumMenuType.Method)
                    {
                        ShowConsoleMenu(menuPage.MenuItems[output - 1].ChildPage);
                    }
                    else
                    {
                        Console.WriteLine();
                        ICli classInstance = Container.Resolve<ICli>(menuPage.MenuItems[output - 1].AssemblyName);
                        Type classType = classInstance.GetType();
                        MethodInfo classMethod = classType.GetMethod(menuPage.MenuItems[output - 1].MethodName);
                        object[] parameters = new object[classMethod.GetParameters().Length];

                        int num = 0;
                        foreach (ParameterInfo param in classMethod.GetParameters())
                        {
                            Console.Write("{0}: ", param.GetCustomAttribute<ParamAttribute>().Name);
                            var value = Console.ReadLine();
                            try
                            {
                                parameters[num] = Convert.ChangeType(value, param.ParameterType);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("{0} must be of type {1}", param.Name, param.ParameterType);
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey();
                                throw;
                            }
                            num++;
                        }

                        ExecuteMethod(classInstance, classMethod, parameters);
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        ShowConsoleMenu(_currentMenu);

                    }
                }
                catch (Exception)
                {
                    ShowConsoleMenu(_currentMenu);
                }
            }

            if (info.Key == ConsoleKey.Escape)
            {
                if (_currentMenu.ParentPage != null)
                {
                    ShowConsoleMenu(_currentMenu.ParentPage);
                }
            }

        }

        private static void ExecuteMethod(ICli classInstance, MethodInfo classMethod, object[] parameters)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            _log.Debug("Start function");

            Console.Clear();
            try
            {
                classMethod.Invoke(classInstance, parameters);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            _log.InfoFormat("Ended with runtime {0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        }
    }
}
