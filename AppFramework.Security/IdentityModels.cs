using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Web.Mvc;

namespace AppFramework.Security
{
    public class AppUser : IdentityUser<long, AppUserLogin, AppUserRole, AppUserClaim>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Info { get; set; }
        public string Version { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(AppUserManager manager)
        {
            // Tenga en cuenta que el valor de authenticationType debe coincidir con el definido en CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Agregar aquí notificaciones personalizadas de usuario
            return userIdentity;
        }
    }
    public class AppUserLogin : IdentityUserLogin<long> { }
    public class AppUserRole : IdentityUserRole<long>
    {
    }
    public class AppUserClaim : IdentityUserClaim<long> { }

    public class AppRole : IdentityRole<long, AppUserRole>
    {
    }

    public class AppPermission
    {
        public int Id { get; set; }
        public AppAction Action { get; set; }
        public int ActionId { get; set; }
        public AppResource Resource { get; set; }
        public int ResourceId { get; set; }
    }

    public class AppResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AppAction
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AppRolePermission
    {
        public long RoleId { get; set; }
        public AppRole Role { get; set; }
        public int PermissionId { get; set; }
        public AppPermission Permission { get; set; }
    }


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
        public DbSet<AppAction> Actions { get; }
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

            modelBuilder.Entity<AppPermission>().ToTable("app_permissions").HasKey(p => p.Id);
            modelBuilder.Entity<AppPermission>().Property(x => x.Id).HasDatabaseGeneratedOption(databaseGeneratedOption: DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<AppRolePermission>().ToTable("app_rolepermissions").HasKey(x => new { x.RoleId, x.PermissionId });
            modelBuilder.Entity<AppRolePermission>().HasRequired(x => x.Role).WithMany().HasForeignKey(x => x.RoleId);
            modelBuilder.Entity<AppRolePermission>().HasRequired(x => x.Permission).WithMany().HasForeignKey(x => x.PermissionId);

            //Resource and Action
            modelBuilder.Entity<AppResource>().ToTable("app_resources").HasKey(x => x.Id);
            modelBuilder.Entity<AppResource>().Property(x => x.Id).HasDatabaseGeneratedOption(databaseGeneratedOption: DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<AppAction>().ToTable("app_action").HasKey(x => x.Id);
            modelBuilder.Entity<AppAction>().Property(x => x.Id).HasDatabaseGeneratedOption(databaseGeneratedOption: DatabaseGeneratedOption.Identity);
            //Action-Resource combination in Permission
            modelBuilder.Entity<AppPermission>().HasRequired(x => x.Action).WithMany().HasForeignKey(x => x.ActionId);
            modelBuilder.Entity<AppPermission>().HasRequired(x => x.Resource).WithMany().HasForeignKey(x => x.ResourceId);

        }
    }
    public class AppUserStore : UserStore<AppUser, AppRole, long, AppUserLogin, AppUserRole, AppUserClaim>
    {
        public AppUserStore(AppSecurityContext context) : base(context)
        {
        }
    }

    public class AppUserValidator : UserValidator<AppUser, long>
    {
        public AppUserValidator(UserManager<AppUser, long> manager) : base(manager)
        {
        }
    }
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Conecte el servicio SMS aquí para enviar un mensaje de texto.
            return Task.FromResult(0);
        }
    }

    public class AppDataProtectorTokenProvider : DataProtectorTokenProvider<AppUser, long>
    {
        public AppDataProtectorTokenProvider(IDataProtector protector) : base(protector)
        {
        }
    }
    public class AppUserManager : UserManager<AppUser, long>
    {
        public AppUserManager(AppUserStore store) : base(store)
        {
        }

        public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
        {
            var userStore = new AppUserStore(context.Get<AppSecurityContext>());
            var manager = new AppUserManager(userStore);
            // Configure la lógica de validación de nombres de usuario

            manager.UserValidator = new AppUserValidator(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure la lógica de validación de contraseñas
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configurar valores predeterminados para bloqueo de usuario
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Registre proveedores de autenticación en dos fases. Esta aplicación usa los pasos Teléfono y Correo electrónico para recibir un código para comprobar el usuario
            // Puede escribir su propio proveedor y conectarlo aquí.
            manager.RegisterTwoFactorProvider("Código telefónico", new PhoneNumberTokenProvider<AppUser, long>
            {
                MessageFormat = "Su código de seguridad es {0}"
            });
            manager.RegisterTwoFactorProvider("Código de correo electrónico", new EmailTokenProvider<AppUser, long>
            {
                Subject = "Código de seguridad",
                BodyFormat = "Su código de seguridad es {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new AppDataProtectorTokenProvider(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    public class AppSignInManager : SignInManager<AppUser, long>
    {
        public AppSignInManager(AppUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(AppUser user)
        {
            return user.GenerateUserIdentityAsync((AppUserManager)UserManager);
        }

        public static AppSignInManager Create(IdentityFactoryOptions<AppSignInManager> options, IOwinContext context)
        {
            return new AppSignInManager(context.GetUserManager<AppUserManager>(), context.Authentication);
        }
    }

    public class AppRoleStore : RoleStore<AppRole, long, AppUserRole>
    {
        public AppRoleStore(AppSecurityContext ctx) : base(ctx)
        {
        }
    }

    public class AppRoleManager : RoleManager<AppRole, long>
    {
        public AppRoleManager(AppRoleStore store) : base(store)
        {
        }
    }

   
}
