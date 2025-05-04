using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAndRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contents_UserId",
                schema: "public",
                table: "Contents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                schema: "public",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_User_UserId",
                schema: "public",
                table: "Contents",
                column: "UserId",
                principalSchema: "public",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_User_UserId",
                schema: "public",
                table: "Projects",
                column: "UserId",
                principalSchema: "public",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_User_UserId",
                schema: "public",
                table: "Contents");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_User_UserId",
                schema: "public",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "User",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_Contents_UserId",
                schema: "public",
                table: "Contents");
        }
    }
}
