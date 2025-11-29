using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class Update_AddRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokenModel_Users_UserId",
                table: "RefreshTokenModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokenModel",
                table: "RefreshTokenModel");

            migrationBuilder.RenameTable(
                name: "RefreshTokenModel",
                newName: "RefreshTokens");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokenModel_UserId",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "RefreshTokenModel");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokenModel",
                newName: "IX_RefreshTokenModel_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokenModel",
                table: "RefreshTokenModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokenModel_Users_UserId",
                table: "RefreshTokenModel",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
