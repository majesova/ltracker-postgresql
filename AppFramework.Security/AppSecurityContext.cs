using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Text.RegularExpressions;

namespace AppFramework.Security
{
    public class AppSecurityContext : IdentityDbContext<AppUser, AppRole, long, AppUserLogin, AppUserRole, AppUserClaim>
    {
        public static AppSecurityContext Create()
        {
            return new AppSecurityContext();
        }
        public AppSecurityContext() : base("LearningContext")
        {
        }

        public DbSet<AppResource> Resources { get; set; }
        public DbSet<AppAction> Actions { get; set; }
        public DbSet<AppPermission> Permissions { get; set; }
        public DbSet<AppUserRole> UserRoles { get; set; }
        public DbSet<AppRolePermission> RolesPermissions { get; set; }

        private string GetTableName(Type type)
        {
            var result = Regex.Replace(type.Name, ".[A-Z]", m => m.Value[0] + "_" + m.Value[1]);
            return result.ToLower();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("security");
            //nombres de propiedades en minúscula como pide postgresql
            modelBuilder.Properties().Configure(p => p.HasColumnName(p.ClrPropertyInfo.Name.ToLower()));
            //nombre de tablas en minúscula como lo pide postgresql
            modelBuilder.Types().Configure(c => c.ToTable(GetTableName(c.ClrType)));
            //Precisión por defecto de decimales a menos que se especifique otro
            modelBuilder.Properties<decimal>().Configure(config => config.HasPrecision(10, 2));
            //Todos los id son claves primarias
            modelBuilder.Entity<AppUser>().ToTable("app_users");
            modelBuilder.Entity<AppUser>().Property(p => p.Id).HasColumnName("id");
            modelBuilder.Entity<AppUser>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<AppUser>().Property(x => x.Version).IsConcurrencyToken();

            modelBuilder.Entity<AppRole>().ToTable("app_roles").Property(p => p.Id).HasColumnName("id");
            modelBuilder.Entity<AppRole>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<AppUserClaim>().ToTable("app_userclaims").Property(p => p.Id).HasColumnName("id");
            modelBuilder.Entity<AppUserRole>().ToTable("app_userroles");
            modelBuilder.Entity<AppUserLogin>().ToTable("app_userlogins");

            modelBuilder.Entity<AppRolePermission>().ToTable("app_rolepermissions").HasKey(x => new { x.RoleId, x.PermissionId });

            //Resource and Action
            modelBuilder.Entity<AppResource>().ToTable("app_resources").HasKey(x => x.Key);
            modelBuilder.Entity<AppAction>().ToTable("app_actions").HasKey(x => x.Key);

            modelBuilder.Entity<AppPermission>().ToTable("app_permissions").HasKey(p => p.Id);
            modelBuilder.Entity<AppPermission>().Property(x => x.Id).HasDatabaseGeneratedOption(databaseGeneratedOption: DatabaseGeneratedOption.Identity);

            //Action-Resource combination in Permission
            modelBuilder.Entity<AppPermission>().HasRequired(x => x.Action).WithMany().HasForeignKey(x => x.ActionKey);
            modelBuilder.Entity<AppPermission>().HasRequired(x => x.Resource).WithMany().HasForeignKey(x => x.ResourceKey);
        }
    }
}
