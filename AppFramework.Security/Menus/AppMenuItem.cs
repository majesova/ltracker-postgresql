using System.Collections.Generic;

namespace AppFramework.Security.Menus
{
    public class AppMenuItem
    {
        public int? Id { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public string PathToResource { get; set; }

        #region menuReference
        public AppMenu AppMenu { get; set; }
        public string AppMenuKey { get; set; }
        #endregion
        public int? ParentId { get; set; }
        public AppMenuItem Parent { get; set; }

        #region childrens

        public ICollection<AppMenuItem> Children { get; set; }
        #endregion

        #region Related Permission
        public int? PermissionId { get; set; }
        public AppPermission Permission { get; set; }
        #endregion

    }
}
