using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddIdPaymentToCheckout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdPayment",
                table: "Checkouts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_IdPayment",
                table: "Checkouts",
                column: "IdPayment");

            migrationBuilder.AddForeignKey(
                name: "FK_Checkouts_Payments_IdPayment",
                table: "Checkouts",
                column: "IdPayment",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checkouts_Payments_IdPayment",
                table: "Checkouts");

            migrationBuilder.DropIndex(
                name: "IX_Checkouts_IdPayment",
                table: "Checkouts");

            migrationBuilder.DropColumn(
                name: "IdPayment",
                table: "Checkouts");
        }
    }
}
