namespace AppFramework.Security.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "public.app_action",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "public.app_permissions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        actionid = c.Int(nullable: false),
                        resourceid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("public.app_action", t => t.actionid, cascadeDelete: true)
                .ForeignKey("public.app_resources", t => t.resourceid, cascadeDelete: true)
                .Index(t => t.actionid)
                .Index(t => t.resourceid);
            
            CreateTable(
                "public.app_resources",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "public.app_roles",
                c => new
                    {
                        id = c.Long(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.id)
                .Index(t => t.name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "public.app_userroles",
                c => new
                    {
                        userid = c.Long(nullable: false),
                        roleid = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.userid, t.roleid })
                .ForeignKey("public.app_roles", t => t.roleid, cascadeDelete: true)
                .ForeignKey("public.app_users", t => t.userid, cascadeDelete: true)
                .Index(t => t.userid)
                .Index(t => t.roleid);
            
            CreateTable(
                "public.app_rolepermissions",
                c => new
                    {
                        roleid = c.Long(nullable: false),
                        permissionid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.roleid, t.permissionid })
                .ForeignKey("public.app_permissions", t => t.permissionid, cascadeDelete: true)
                .ForeignKey("public.app_roles", t => t.roleid, cascadeDelete: true)
                .Index(t => t.roleid)
                .Index(t => t.permissionid);
            
            CreateTable(
                "public.app_users",
                c => new
                    {
                        id = c.Long(nullable: false, identity: true),
                        firstname = c.String(),
                        lastname = c.String(),
                        info = c.String(),
                        version = c.String(),
                        email = c.String(maxLength: 256),
                        emailconfirmed = c.Boolean(nullable: false),
                        passwordhash = c.String(),
                        securitystamp = c.String(),
                        phonenumber = c.String(),
                        phonenumberconfirmed = c.Boolean(nullable: false),
                        twofactorenabled = c.Boolean(nullable: false),
                        lockoutenddateutc = c.DateTime(),
                        lockoutenabled = c.Boolean(nullable: false),
                        accessfailedcount = c.Int(nullable: false),
                        username = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.id)
                .Index(t => t.username, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "public.app_userclaims",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userid = c.Long(nullable: false),
                        claimtype = c.String(),
                        claimvalue = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("public.app_users", t => t.userid, cascadeDelete: true)
                .Index(t => t.userid);
            
            CreateTable(
                "public.app_userlogins",
                c => new
                    {
                        loginprovider = c.String(nullable: false, maxLength: 128),
                        providerkey = c.String(nullable: false, maxLength: 128),
                        userid = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.loginprovider, t.providerkey, t.userid })
                .ForeignKey("public.app_users", t => t.userid, cascadeDelete: true)
                .Index(t => t.userid);
            
        }
        
        public override void Down()
        {
            DropForeignKey("public.app_userroles", "userid", "public.app_users");
            DropForeignKey("public.app_userlogins", "userid", "public.app_users");
            DropForeignKey("public.app_userclaims", "userid", "public.app_users");
            DropForeignKey("public.app_rolepermissions", "roleid", "public.app_roles");
            DropForeignKey("public.app_rolepermissions", "permissionid", "public.app_permissions");
            DropForeignKey("public.app_userroles", "roleid", "public.app_roles");
            DropForeignKey("public.app_permissions", "resourceid", "public.app_resources");
            DropForeignKey("public.app_permissions", "actionid", "public.app_action");
            DropIndex("public.app_userlogins", new[] { "userid" });
            DropIndex("public.app_userclaims", new[] { "userid" });
            DropIndex("public.app_users", "UserNameIndex");
            DropIndex("public.app_rolepermissions", new[] { "permissionid" });
            DropIndex("public.app_rolepermissions", new[] { "roleid" });
            DropIndex("public.app_userroles", new[] { "roleid" });
            DropIndex("public.app_userroles", new[] { "userid" });
            DropIndex("public.app_roles", "RoleNameIndex");
            DropIndex("public.app_permissions", new[] { "resourceid" });
            DropIndex("public.app_permissions", new[] { "actionid" });
            DropTable("public.app_userlogins");
            DropTable("public.app_userclaims");
            DropTable("public.app_users");
            DropTable("public.app_rolepermissions");
            DropTable("public.app_userroles");
            DropTable("public.app_roles");
            DropTable("public.app_resources");
            DropTable("public.app_permissions");
            DropTable("public.app_action");
        }
    }
}
