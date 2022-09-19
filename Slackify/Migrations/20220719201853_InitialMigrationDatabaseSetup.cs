using Microsoft.EntityFrameworkCore.Migrations;

namespace Slackify.Migrations;

public partial class InitialMigrationDatabaseSetup : Migration
{
    protected override void Up( MigrationBuilder migrationBuilder )
    {
        if( migrationBuilder is null )
        {
            throw new ArgumentNullException( nameof( migrationBuilder ),
                                             "Database goes boom! if there is no migrationBuilder" );
        }

        _ = migrationBuilder.CreateTable(
            name: "Users",
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
                _ = table.PrimaryKey( "PK_Users", x => x.Id );
            } );

        _ = migrationBuilder.CreateTable(
            name: "Messages",
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
                _ = table.PrimaryKey( "PK_Messages", x => x.Id );
                _ = table.ForeignKey(
                    name: "FK_Messages_Users_FromUserId",
                    column: x => x.FromUserId,
                    principalTable: "Users",
                    principalColumn: "Id" );
                _ = table.ForeignKey(
                    name: "FK_Messages_Users_ToUserId",
                    column: x => x.ToUserId,
                    principalTable: "Users",
                    principalColumn: "Id" );
            } );

        _ = migrationBuilder.CreateIndex(
            name: "IX_Messages_FromUserId",
            table: "Messages",
            column: "FromUserId" );

        _ = migrationBuilder.CreateIndex(
            name: "IX_Messages_ToUserId",
            table: "Messages",
            column: "ToUserId" );
    }

    protected override void Down( MigrationBuilder migrationBuilder )
    {
        if( migrationBuilder is null )
        {
            throw new ArgumentNullException( nameof( migrationBuilder ),
                                             "Database goes boom! if there is no migrationBuilder" );
        }

        _ = migrationBuilder.DropTable(
            name: "Messages" );

        _ = migrationBuilder.DropTable(
            name: "Users" );
    }
}
