namespace AppFramework.Security.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "security.app_action",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "security.app_permissions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        actionid = c.Int(nullable: false),
                        resourceid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("security.app_action", t => t.actionid, cascadeDelete: true)
                .ForeignKey("security.app_resources", t => t.resourceid, cascadeDelete: true)
                .Index(t => t.actionid)
                .Index(t => t.resourceid);
            
            CreateTable(
                "security.app_resources",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "security.app_roles",
                c => new
                    {
                        id = c.Long(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.id)
                .Index(t => t.name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "security.app_userroles",
                c => new
                    {
                        userid = c.Long(nullable: false),
                        roleid = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.userid, t.roleid })
                .ForeignKey("security.app_roles", t => t.roleid, cascadeDelete: true)
                .ForeignKey("security.app_users", t => t.userid, cascadeDelete: true)
                .Index(t => t.userid)
                .Index(t => t.roleid);
            
            CreateTable(
                "security.app_rolepermissions",
                c => new
                    {
                        roleid = c.Long(nullable: false),
                        permissionid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.roleid, t.permissionid })
                .ForeignKey("security.app_permissions", t => t.permissionid, cascadeDelete: true)
                .ForeignKey("security.app_roles", t => t.roleid, cascadeDelete: true)
                .Index(t => t.roleid)
                .Index(t => t.permissionid);
            
            CreateTable(
                "security.app_users",
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
                "security.app_userclaims",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userid = c.Long(nullable: false),
                        claimtype = c.String(),
                        claimvalue = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("security.app_users", t => t.userid, cascadeDelete: true)
                .Index(t => t.userid);
            
            CreateTable(
                "security.app_userlogins",
                c => new
                    {
                        loginprovider = c.String(nullable: false, maxLength: 128),
                        providerkey = c.String(nullable: false, maxLength: 128),
                        userid = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.loginprovider, t.providerkey, t.userid })
                .ForeignKey("security.app_users", t => t.userid, cascadeDelete: true)
                .Index(t => t.userid);
            
        }
        
        public override void Down()
        {
            DropForeignKey("security.app_userroles", "userid", "security.app_users");
            DropForeignKey("security.app_userlogins", "userid", "security.app_users");
            DropForeignKey("security.app_userclaims", "userid", "security.app_users");
            DropForeignKey("security.app_rolepermissions", "roleid", "security.app_roles");
            DropForeignKey("security.app_rolepermissions", "permissionid", "security.app_permissions");
            DropForeignKey("security.app_userroles", "roleid", "security.app_roles");
            DropForeignKey("security.app_permissions", "resourceid", "security.app_resources");
            DropForeignKey("security.app_permissions", "actionid", "security.app_action");
            DropIndex("security.app_userlogins", new[] { "userid" });
            DropIndex("security.app_userclaims", new[] { "userid" });
            DropIndex("security.app_users", "UserNameIndex");
            DropIndex("security.app_rolepermissions", new[] { "permissionid" });
            DropIndex("security.app_rolepermissions", new[] { "roleid" });
            DropIndex("security.app_userroles", new[] { "roleid" });
            DropIndex("security.app_userroles", new[] { "userid" });
            DropIndex("security.app_roles", "RoleNameIndex");
            DropIndex("security.app_permissions", new[] { "resourceid" });
            DropIndex("security.app_permissions", new[] { "actionid" });
            DropTable("security.app_userlogins");
            DropTable("security.app_userclaims");
            DropTable("security.app_users");
            DropTable("security.app_rolepermissions");
            DropTable("security.app_userroles");
            DropTable("security.app_roles");
            DropTable("security.app_resources");
            DropTable("security.app_permissions");
            DropTable("security.app_action");
        }
    }
}
