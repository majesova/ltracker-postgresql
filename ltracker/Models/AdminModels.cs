using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ltracker.Models
{
    #region ViewModels
    public class AppUserViewModel
    {
        public long Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }


    public class EditAppUserViewModel
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public ICollection<AppRoleViewModel> AvailableRoles { get; set; }
        public long[] SelectedRoles { get; set; }
    }

    public class DetailsAppUserViewModel
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public ICollection<AppRoleViewModel> AssignedRoles { get; set; }
    }

    public class AppRoleViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class NewAppRoleViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ICollection<AppPermissionViewModel> AvailablePermissions { get; set; }
        public int[] SelectedPermissions { get; set; }
    }

    public class EditAppRoleViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ICollection<AppPermissionViewModel> AvailablePermissions { get; set; }
        public int[] SelectedPermissions { get; set; }
    }

    public class DetailsAppRoleViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ICollection<AppPermissionViewModel> Permissions { get; set; }
    }

    public class AppPermissionViewModel
    {
        public int Id { get; set; }
        public string ActionName { get; set; }
        public string ResourceName { get; set; }
    }
    #endregion

    #region menus
    public class MenuViewModel
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public AppRoleViewModel Role { get; set; }
    }

    public class NewMenuViewModel
    {
        [Required]
        [MaxLength(25)]
        public string Key { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public bool? IsActive { get; set; }
    }

    public class EditMenuViewModel
    {
        [Required]
        [MaxLength(25)]
        public string Key { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public bool? IsActive { get; set; }
    }

    public class MenuItemListViewModel
    {
        public string MenuKey { get; set; }
        public string MenuName { get; set; }
        public ICollection<MenuItemViewModel> MenuItems { get; set; }
    }

    public class MenuItemViewModel
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public string PathToResource { get; set; }
        public int? ParentId { get; set; }
        public MenuItemViewModel Parent { get; set; }
    }

    public class NewMenuItemViewModel
    {
        [MaxLength(25)]
        public string MenuKey { get; set; }
        [MaxLength(50)]
        public string MenuName { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Order { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PathToResource { get; set; }
        public SelectList AvailablePermissions { get; set; }
        public int? PermissionId { get; set; }
        public SelectList AvailableMenuItems { get; set; }
        public int? ParentId { get; set; }
    }


    public class EditMenuItemViewModel
    {
        public int Id { get; set; }
        [MaxLength(25)]
        public string MenuKey { get; set; }
        [MaxLength(50)]
        public string MenuName { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Order { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PathToResource { get; set; }
        public SelectList AvailablePermissions { get; set; }
        public int? PermissionId { get; set; }
        public SelectList AvailableMenuItems { get; set; }
        public int? ParentId { get; set; }
    }
    #endregion
}