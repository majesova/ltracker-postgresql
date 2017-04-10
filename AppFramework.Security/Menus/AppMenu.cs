using System.Collections.Generic;

namespace AppFramework.Security.Menus
{
    public class AppMenu 
    {
        public string Key { get; set; }
        public bool? IsActive { get; set; }
        public string Name { get; set; }
        public ICollection<AppMenuItem> Items { get; set; }
        public long? RoleId { get; set; }
        public AppRole Role { get; set; }
    }
}
