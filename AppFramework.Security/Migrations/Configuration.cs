namespace AppFramework.Security.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<AppSecurityContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations";
        }

        protected override void Seed(AppSecurityContext context)
        {

            //Acciones
            var read = new AppAction { Key = "R", Name = "Leer" };
            var write = new AppAction { Key = "W", Name = "Escribir" };
            var modify = new AppAction { Key = "M", Name = "Modificar" };
            var delete = new AppAction { Key = "D", Name = "Eliminar" };
            var access = new AppAction { Key = "A", Name = "Acceder" };

            context.Actions.Add(read);
            context.Actions.Add(write);
            context.Actions.Add(modify);
            context.Actions.Add(delete);
            context.Actions.Add(access);

            //Recursos
            var roles = new AppResource { Key = "ROLES", Name= "Roles" };
             var usuarios = new AppResource { Key = "USERS", Name = "Usuarios" };

             context.Resources.Add(roles);
             context.Resources.Add(usuarios);
            
            var adminRol = new AppRole { Name = "Administrator" };

            var rRoles = new AppPermission { Action = read, Resource = roles };
            var wRoles = new AppPermission { Action =write , Resource = roles };
            var mRoles = new AppPermission { Action = modify, Resource = roles };
            var dRoles = new AppPermission { Action = delete, Resource = roles };

            var rUsers = new AppPermission { Action = read, Resource = usuarios };
            var wUsers= new AppPermission { Action = write, Resource = usuarios };
            var mUsers = new AppPermission { Action = modify, Resource = usuarios };
            var dUsers = new AppPermission { Action = delete, Resource = usuarios };

            context.Permissions.Add(rRoles);
            context.Permissions.Add(wRoles);
            context.Permissions.Add(mRoles);
            context.Permissions.Add(dRoles);

            context.Permissions.Add(rUsers);
            context.Permissions.Add(wUsers);
            context.Permissions.Add(mUsers);
            context.Permissions.Add(dUsers);

            context.Roles.Add(adminRol);

            context.RolesPermissions.Add(new AppRolePermission { Role = adminRol, Permission = rRoles });
            context.RolesPermissions.Add(new AppRolePermission { Role = adminRol, Permission = wRoles });
            context.RolesPermissions.Add(new AppRolePermission { Role = adminRol, Permission = mRoles });
            context.RolesPermissions.Add(new AppRolePermission { Role = adminRol, Permission = dRoles });

            context.RolesPermissions.Add(new AppRolePermission { Role = adminRol, Permission = rUsers });
            context.RolesPermissions.Add(new AppRolePermission { Role = adminRol, Permission = wUsers });
            context.RolesPermissions.Add(new AppRolePermission { Role = adminRol, Permission = mUsers });
            context.RolesPermissions.Add(new AppRolePermission { Role = adminRol, Permission = dUsers });

            context.SaveChanges();
        }
    }
}
