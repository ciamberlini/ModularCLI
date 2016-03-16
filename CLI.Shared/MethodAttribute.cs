using System;

namespace CLI.Shared
{
    /// <summary>
    /// Attribute to define method detail to show in the CLI interface
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MethodAttribute : Attribute
    {
        /// <summary>
        /// Property that define the menu string to show in the CLI
        /// </summary>
        public string Name;
        /// <summary>
        /// Property that define the string to execute method by cmd
        /// </summary>
        public string CmdLine;
        public MethodAttribute(string name)
        {
            Name = name;
        }
        public MethodAttribute(string name, string cmdLine)
        {
            Name = name;
            CmdLine = cmdLine;
        }
    }
}
