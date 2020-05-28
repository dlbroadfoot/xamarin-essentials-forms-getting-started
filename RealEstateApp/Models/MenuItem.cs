using System;

namespace RealEstateApp.Models
{
    public class MenuItem
    {
        public MenuItemType MenuItemType { get; set; }

        public string Title { get; set; }

        public Type ViewModelType { get; set; }

        public bool IsEnabled { get; set; }

        public object NavigationData { get; set; }
    }
}