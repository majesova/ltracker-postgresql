namespace AppFramework.Security.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataMenusMenuname : DbMigration
    {
        public override void Up()
        {
            AddColumn("security.app_menus", "name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("security.app_menus", "name");
        }
    }
}
