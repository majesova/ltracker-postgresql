namespace AppFramework.Security.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "security.appaction",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "security.appresource",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "security.approles",
                c => new
                    {
                        id = c.Long(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.id)
                .Index(t => t.name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "security.apppermissions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        actionid = c.Int(nullable: false),
                        resourceid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("security.appaction", t => t.actionid, cascadeDelete: true)
                .ForeignKey("security.appresource", t => t.resourceid, cascadeDelete: true)
                .Index(t => t.actionid)
                .Index(t => t.resourceid);
            
            CreateTable(
                "security.appuserroles",
                c => new
                    {
                        userid = c.Long(nullable: false),
                        roleid = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.userid, t.roleid })
                .ForeignKey("security.approles", t => t.roleid, cascadeDelete: true)
                .ForeignKey("security.appusers", t => t.userid, cascadeDelete: true)
                .Index(t => t.userid)
                .Index(t => t.roleid);
            
            CreateTable(
                "security.appusers",
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
                "security.appuserclaims",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userid = c.Long(nullable: false),
                        claimtype = c.String(),
                        claimvalue = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("security.appusers", t => t.userid, cascadeDelete: true)
                .Index(t => t.userid);
            
            CreateTable(
                "security.appuserlogins",
                c => new
                    {
                        loginprovider = c.String(nullable: false, maxLength: 128),
                        providerkey = c.String(nullable: false, maxLength: 128),
                        userid = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.loginprovider, t.providerkey, t.userid })
                .ForeignKey("security.appusers", t => t.userid, cascadeDelete: true)
                .Index(t => t.userid);
            
            CreateTable(
                "security.apppermsinroles",
                c => new
                    {
                        roleid = c.Long(nullable: false),
                        permissionid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.roleid, t.permissionid })
                .ForeignKey("security.approles", t => t.roleid, cascadeDelete: true)
                .ForeignKey("security.apppermissions", t => t.permissionid, cascadeDelete: true)
                .Index(t => t.roleid)
                .Index(t => t.permissionid);
            
        }
        
        public override void Down()
        {
            DropForeignKey("security.appuserroles", "userid", "security.appusers");
            DropForeignKey("security.appuserlogins", "userid", "security.appusers");
            DropForeignKey("security.appuserclaims", "userid", "security.appusers");
            DropForeignKey("security.appuserroles", "roleid", "security.approles");
            DropForeignKey("security.apppermsinroles", "permissionid", "security.apppermissions");
            DropForeignKey("security.apppermsinroles", "roleid", "security.approles");
            DropForeignKey("security.apppermissions", "resourceid", "security.appresource");
            DropForeignKey("security.apppermissions", "actionid", "security.appaction");
            DropIndex("security.apppermsinroles", new[] { "permissionid" });
            DropIndex("security.apppermsinroles", new[] { "roleid" });
            DropIndex("security.appuserlogins", new[] { "userid" });
            DropIndex("security.appuserclaims", new[] { "userid" });
            DropIndex("security.appusers", "UserNameIndex");
            DropIndex("security.appuserroles", new[] { "roleid" });
            DropIndex("security.appuserroles", new[] { "userid" });
            DropIndex("security.apppermissions", new[] { "resourceid" });
            DropIndex("security.apppermissions", new[] { "actionid" });
            DropIndex("security.approles", "RoleNameIndex");
            DropTable("security.apppermsinroles");
            DropTable("security.appuserlogins");
            DropTable("security.appuserclaims");
            DropTable("security.appusers");
            DropTable("security.appuserroles");
            DropTable("security.apppermissions");
            DropTable("security.approles");
            DropTable("security.appresource");
            DropTable("security.appaction");
        }
    }
}
