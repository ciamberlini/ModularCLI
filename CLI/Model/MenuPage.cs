using System.Collections.Generic;

namespace Cli.Model
{
    public class MenuPage
    {
        public string Title { get; set; }
        public List<MenuItem> MenuItems { get; set; }
        public MenuPage ParentPage { get; set; }
        public MenuPage(string title)
        {
            MenuItems = new List<MenuItem>();
            Title = title;
        }
    }

}
