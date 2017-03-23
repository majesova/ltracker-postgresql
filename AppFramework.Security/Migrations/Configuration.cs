namespace AppFramework.Security.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AppFramework.Security.AppSecurityContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AppFramework.Security.AppSecurityContext context)
        {
            var resource = new AppResource { Name = "Assignment" };
            context.Resources.Add(resource);
            context.SaveChanges();
            var permissionWrite = new AppPermission { Action = new AppAction { Name = "WRITE" }, Resource = resource };
            var permissionRead = new AppPermission { Action = new AppAction { Name = "READ" }, Resource = resource  };
            context.Permissions.Add(permissionRead);
            context.Permissions.Add(permissionWrite);
            context.SaveChanges();
            var role = new AppRole() { Name = "Admin" };
            context.Roles.Add(role);
            context.SaveChanges();
            var rolePermission = new AppRolePermission { RoleId = role.Id, PermissionId = permissionWrite.Id };
            var rolePermission2 = new AppRolePermission { RoleId = role.Id, PermissionId = permissionRead.Id };
            context.RolesPermissions.Add(rolePermission);
            context.RolesPermissions.Add(rolePermission2);
            context.SaveChanges();
        }
    }
}
