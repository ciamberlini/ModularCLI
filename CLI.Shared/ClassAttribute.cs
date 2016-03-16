using System;

namespace CLI.Shared
{
    /// <summary>
     /// Attribute to define class detail to show in the CLI interface
     /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ClassAttribute : Attribute
    {
        /// <summary>
        /// Property that define the string to show in the CLI
        /// </summary>
        public string Name;
        /// <summary>
        /// Property that define the author of the module (optional)
        /// </summary>
        public string Author;
        /// <summary>
        /// Property that define the description of the module (optional)
        /// </summary>
        public string Description;
        /// <summary>
        /// Property that define the version of the module (optional)
        /// </summary>
        public string Version;
        /// <summary>
        /// Property that define the release date of the module (optional)
        /// </summary>
        public string ReleaseDate;
    }
}
