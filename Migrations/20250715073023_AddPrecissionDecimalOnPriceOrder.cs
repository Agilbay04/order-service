using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.Migrations
{
    /// <inheritdoc />
    public partial class AddPrecissionDecimalOnPriceOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ex_total_price",
                table: "order_details",
                newName: "total_price");

            migrationBuilder.AddColumn<decimal>(
                name: "sub_total_price",
                table: "orders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sub_total_price",
                table: "orders");

            migrationBuilder.RenameColumn(
                name: "total_price",
                table: "order_details",
                newName: "ex_total_price");
        }
    }
}
