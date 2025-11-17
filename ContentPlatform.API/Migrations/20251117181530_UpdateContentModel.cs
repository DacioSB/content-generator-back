using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_Projects_ProjectId1",
                schema: "public",
                table: "Contents");

            migrationBuilder.DropIndex(
                name: "IX_Contents_ProjectId1",
                schema: "public",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "ProjectId1",
                schema: "public",
                table: "Contents");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "public",
                table: "Contents",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "public",
                table: "Contents",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "public",
                table: "Contents",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                schema: "public",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "public",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "public",
                table: "Contents");

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId1",
                schema: "public",
                table: "Contents",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contents_ProjectId1",
                schema: "public",
                table: "Contents",
                column: "ProjectId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_Projects_ProjectId1",
                schema: "public",
                table: "Contents",
                column: "ProjectId1",
                principalSchema: "public",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
