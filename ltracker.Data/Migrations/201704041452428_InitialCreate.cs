namespace ltracker.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "public.assignedcourses",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        assignmentdate = c.DateTime(nullable: false),
                        iscompleted = c.Boolean(nullable: false),
                        startdate = c.DateTime(),
                        finishdate = c.DateTime(),
                        totalhours = c.Decimal(precision: 10, scale: 2),
                        individualid = c.Int(nullable: false),
                        courseid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("public.courses", t => t.courseid, cascadeDelete: true)
                .ForeignKey("public.individuals", t => t.individualid, cascadeDelete: true)
                .Index(t => t.individualid)
                .Index(t => t.courseid);
            
            CreateTable(
                "public.courses",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 500),
                        durationavg = c.Decimal(precision: 10, scale: 2),
                        description = c.String(maxLength: 500),
                        Topic_Id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("public.topics", t => t.Topic_Id)
                .Index(t => t.Topic_Id);
            
            CreateTable(
                "public.topics",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 500),
                        description = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "public.individuals",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 500),
                        email = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "public.courses_in_topics",
                c => new
                    {
                        courseid = c.Int(nullable: false),
                        topicid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.courseid, t.topicid })
                .ForeignKey("public.courses", t => t.courseid, cascadeDelete: true)
                .ForeignKey("public.topics", t => t.topicid, cascadeDelete: true)
                .Index(t => t.courseid)
                .Index(t => t.topicid);
            
        }
        
        public override void Down()
        {
            DropForeignKey("public.assignedcourses", "individualid", "public.individuals");
            DropForeignKey("public.assignedcourses", "courseid", "public.courses");
            DropForeignKey("public.courses_in_topics", "topicid", "public.topics");
            DropForeignKey("public.courses_in_topics", "courseid", "public.courses");
            DropForeignKey("public.courses", "Topic_Id", "public.topics");
            DropIndex("public.courses_in_topics", new[] { "topicid" });
            DropIndex("public.courses_in_topics", new[] { "courseid" });
            DropIndex("public.courses", new[] { "Topic_Id" });
            DropIndex("public.assignedcourses", new[] { "courseid" });
            DropIndex("public.assignedcourses", new[] { "individualid" });
            DropTable("public.courses_in_topics");
            DropTable("public.individuals");
            DropTable("public.topics");
            DropTable("public.courses");
            DropTable("public.assignedcourses");
        }
    }
}
