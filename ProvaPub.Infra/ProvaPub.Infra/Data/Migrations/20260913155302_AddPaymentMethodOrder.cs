using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProvaPub.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethodOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Numbers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Numbers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Hubert Legros" },
                    { 2, "Elisa Walter" },
                    { 3, "Laverne Rohan" },
                    { 4, "Gerardo Kuhlman" },
                    { 5, "Jennie Heaney" },
                    { 6, "Everett Johnson" },
                    { 7, "Geneva Stracke" },
                    { 8, "Harvey Ankunding" },
                    { 9, "Maxine Stroman" },
                    { 10, "Allison Parisian" },
                    { 11, "Terence Jenkins" },
                    { 12, "Mable Breitenberg" },
                    { 13, "Carroll Miller" },
                    { 14, "Benjamin Brekke" },
                    { 15, "Joanne Schneider" },
                    { 16, "Randal Hodkiewicz" },
                    { 17, "Carmen Moen" },
                    { 18, "Vincent Daniel" },
                    { 19, "Randy Rowe" },
                    { 20, "Dora Bartoletti" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Gorgeous Steel Tuna" },
                    { 2, "Intelligent Soft Pants" },
                    { 3, "Sleek Soft Cheese" },
                    { 4, "Intelligent Rubber Fish" },
                    { 5, "Tasty Frozen Ball" },
                    { 6, "Practical Plastic Cheese" },
                    { 7, "Gorgeous Cotton Hat" },
                    { 8, "Licensed Rubber Keyboard" },
                    { 9, "Gorgeous Wooden Bacon" },
                    { 10, "Fantastic Cotton Towels" },
                    { 11, "Practical Metal Hat" },
                    { 12, "Sleek Rubber Bacon" },
                    { 13, "Refined Steel Table" },
                    { 14, "Licensed Soft Salad" },
                    { 15, "Practical Rubber Chair" },
                    { 16, "Handcrafted Wooden Mouse" },
                    { 17, "Awesome Soft Sausages" },
                    { 18, "Licensed Soft Salad" },
                    { 19, "Unbranded Concrete Hat" },
                    { 20, "Fantastic Metal Chair" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Numbers_Number",
                table: "Numbers",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Numbers");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
