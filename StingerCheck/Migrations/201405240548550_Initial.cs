namespace StingerCheck.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Movies",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TomatoId = c.String(maxLength: 50),
                        Title = c.String(),
                        TomatoUrl = c.String(),
                        HasMidStinger = c.Boolean(),
                        HasFinalStinger = c.Boolean(),
                        MidTeaser = c.Short(),
                        MidClosure = c.Short(),
                        MidGag = c.Short(),
                        MidEgg = c.Short(),
                        FinalTeaser = c.Short(),
                        FinalClosure = c.Short(),
                        FinalGag = c.Short(),
                        FinalEgg = c.Short(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.TomatoId, unique: true);
            
            CreateTable(
                "dbo.Stingers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        HasMidStinger = c.Boolean(nullable: false),
                        HasFinalStinger = c.Boolean(nullable: false),
                        MidTeaser = c.Short(nullable: false),
                        MidClosure = c.Short(nullable: false),
                        MidGag = c.Short(nullable: false),
                        MidEgg = c.Short(nullable: false),
                        FinalTeaser = c.Short(nullable: false),
                        FinalClosure = c.Short(nullable: false),
                        FinalGag = c.Short(nullable: false),
                        FinalEgg = c.Short(nullable: false),
                        Movie_Id = c.Long(),
                        User_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Movies", t => t.Movie_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.Movie_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Email = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Email, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Stingers", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Stingers", "Movie_Id", "dbo.Movies");
            DropIndex("dbo.Users", new[] { "Email" });
            DropIndex("dbo.Stingers", new[] { "User_Id" });
            DropIndex("dbo.Stingers", new[] { "Movie_Id" });
            DropIndex("dbo.Movies", new[] { "TomatoId" });
            DropTable("dbo.Users");
            DropTable("dbo.Stingers");
            DropTable("dbo.Movies");
        }
    }
}
