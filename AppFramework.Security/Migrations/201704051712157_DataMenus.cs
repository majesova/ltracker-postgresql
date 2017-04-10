namespace AppFramework.Security.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataMenus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "security.app_menuitems",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        order = c.Int(nullable: false),
                        name = c.String(),
                        pathtoresource = c.String(),
                        appmenukey = c.String(nullable: false, maxLength: 25),
                        parentid = c.Int(),
                        permissionid = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("security.app_menus", t => t.appmenukey, cascadeDelete: true)
                .ForeignKey("security.app_menuitems", t => t.parentid)
                .ForeignKey("security.app_permissions", t => t.permissionid)
                .Index(t => t.appmenukey)
                .Index(t => t.parentid)
                .Index(t => t.permissionid);
            
            CreateTable(
                "security.app_menus",
                c => new
                    {
                        key = c.String(nullable: false, maxLength: 25),
                        isactive = c.Boolean(),
                        roleid = c.Long(),
                    })
                .PrimaryKey(t => t.key)
                .ForeignKey("security.app_roles", t => t.roleid)
                .Index(t => t.roleid);
            
        }
        
        public override void Down()
        {
            DropForeignKey("security.app_menuitems", "permissionid", "security.app_permissions");
            DropForeignKey("security.app_menuitems", "parentid", "security.app_menuitems");
            DropForeignKey("security.app_menus", "roleid", "security.app_roles");
            DropForeignKey("security.app_menuitems", "appmenukey", "security.app_menus");
            DropIndex("security.app_menus", new[] { "roleid" });
            DropIndex("security.app_menuitems", new[] { "permissionid" });
            DropIndex("security.app_menuitems", new[] { "parentid" });
            DropIndex("security.app_menuitems", new[] { "appmenukey" });
            DropTable("security.app_menus");
            DropTable("security.app_menuitems");
        }
    }
}
