using System;

namespace DemoApp.Tui.Attributes
{
    public class MenuItemDetailsAttribute : Attribute
    {
        public MenuItemDetailsAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
    }
}