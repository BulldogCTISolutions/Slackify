using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slackify.Migrations;

public partial class InitialMigrationDatabaseSetup : Migration
{
    protected override void Up( MigrationBuilder migrationBuilder )
    {
        migrationBuilder.CreateTable(
            name: "Slackify_Users",
            columns: table => new
            {
                Id = table.Column<int>( type: "int", nullable: false )
                    .Annotation( "SqlServer:Identity", "1, 1" ),
                UserName = table.Column<string>( type: "nvarchar(max)", nullable: true ),
                Email = table.Column<string>( type: "nvarchar(max)", nullable: true ),
                Picture = table.Column<string>( type: "nvarchar(max)", nullable: true ),
                DateJoined = table.Column<DateTime>( type: "datetime2", nullable: false )
            },
            constraints: table =>
            {
                table.PrimaryKey( "PK_Slackify_Users", x => x.Id );
            } );

        migrationBuilder.CreateTable(
            name: "Slackify_Messages",
            columns: table => new
            {
                Id = table.Column<int>( type: "int", nullable: false )
                    .Annotation( "SqlServer:Identity", "1, 1" ),
                FromUserId = table.Column<int>( type: "int", nullable: false ),
                ToUserId = table.Column<int>( type: "int", nullable: false ),
                Chat = table.Column<string>( type: "nvarchar(max)", nullable: true ),
                CreatedDate = table.Column<DateTime>( type: "datetime2", nullable: false )
            },
            constraints: table =>
            {
                table.PrimaryKey( "PK_Slackify_Messages", x => x.Id );
                table.ForeignKey(
                    name: "FK_Slackify_Messages_Slackify_Users_FromUserId",
                    column: x => x.FromUserId,
                    principalTable: "Slackify_Users",
                    principalColumn: "Id" );
                table.ForeignKey(
                    name: "FK_Slackify_Messages_Slackify_Users_ToUserId",
                    column: x => x.ToUserId,
                    principalTable: "Slackify_Users",
                    principalColumn: "Id" );
            } );

        migrationBuilder.CreateIndex(
            name: "IX_Slackify_Messages_FromUserId",
            table: "Slackify_Messages",
            column: "FromUserId" );

        migrationBuilder.CreateIndex(
            name: "IX_Slackify_Messages_ToUserId",
            table: "Slackify_Messages",
            column: "ToUserId" );
    }

    protected override void Down( MigrationBuilder migrationBuilder )
    {
        migrationBuilder.DropTable(
            name: "Slackify_Messages" );

        migrationBuilder.DropTable(
            name: "Slackify_Users" );
    }
}
