using System;

namespace DemoApp.Tui.Attributes
{
    public class MenuItemDetailsAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        
        public MenuItemDetailsAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}