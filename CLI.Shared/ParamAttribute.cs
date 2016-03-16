using System;

namespace CLI.Shared
{
    /// <summary>
    /// Attribute to define method parameter's detail to show in the CLI interface
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParamAttribute : Attribute
    {
        /// <summary>
        /// Property that define the string to show in the CLI
        /// </summary>
        public string Name;
        /// <summary>
        /// Property that define the string to setup method parameter by cmd
        /// </summary>
        public string CmdLine;
        public ParamAttribute(string name)
        {
            Name = name;
        }
        public ParamAttribute(string name, string cmdLine)
        {
            Name = name;
            CmdLine = cmdLine;
        }
    }
}
