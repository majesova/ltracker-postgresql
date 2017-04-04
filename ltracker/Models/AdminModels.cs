using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
}