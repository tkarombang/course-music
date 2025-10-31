using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class MigrasiBaruDariSaya : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "IdCategory",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "IdCategory",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "IdCategory",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "IdCourse",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "IdCourse",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "IdCourse",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "IdCourse",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "IdCourse",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "IdCourse",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "IdCourse",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "IdCourse",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "IdCategory",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "IdCategory",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "IdCategory",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "IdCategory",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "IdCategory",
                keyValue: 5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "IdCategory", "ImageCategory", "NameCategory" },
                values: new object[,]
                {
                    { 1, "Drum.png", "Drum" },
                    { 2, "Piano.png", "Piano" },
                    { 3, "Gitar.png", "Gitar" },
                    { 4, "Biola.png", "Biola" },
                    { 5, "Saxophone.png", "Saxophone" },
                    { 6, "Saxophone.png", "Flute" },
                    { 7, "Gitar.png", "Bass" },
                    { 8, "Menyanyi.png", "Menyanyi" }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "IdCourse", "Harga", "IdCategory", "ImageCourse", "NamaCourse" },
                values: new object[,]
                {
                    { 1, 8500000m, 1, "d4.png", "Kursus Drummer Special Coach (Eno Netral)" },
                    { 2, 5450000m, 1, "d1.png", "Expert Level Drummer Lessons" },
                    { 3, 13000000m, 1, "d2.png", "From Zero to Professional Drummer (Complit Package)" },
                    { 4, 2200000m, 1, "d3.png", "Drummer for kids (Level Basic/1)" },
                    { 5, 11650000m, 2, "Copy of Rectangle 12-5.png", "Kursu Piano : From Zero to Pro (Full Package)" },
                    { 6, 1600000m, 3, "Copy of Rectangle 12-2.png", "[Beginner] Guitar class for kids " },
                    { 7, 3000000m, 4, "Copy of Rectangle 12-3.png", "Biola Mid-Level Course" },
                    { 8, 7350000m, 5, "Copy of Rectangle 12-6.png", "Expert Level Saxophone" }
                });
        }
    }
}
