using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TranslatorApp.Migrations
{
    /// <inheritdoc />
    public partial class uriParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiUriParameters",
                table: "Translators",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TranslatorViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApiUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApiUriParameters = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslatorViewModel", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TranslatorViewModel");

            migrationBuilder.DropColumn(
                name: "ApiUriParameters",
                table: "Translators");
        }
    }
}
