namespace Cli.Model
{
    public enum EnumMenuType
    {
        Assembly,
        Class,
        Method
    }
    public class MenuItem
    {
        public string MenuString { get; set; }
        public string AssemblyName { get; set; }
        public string MethodName { get; set; }
        public MenuPage ChildPage { get; set; }
        public EnumMenuType MenuItemType { get; set; }
    }
}
