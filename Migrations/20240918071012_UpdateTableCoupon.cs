using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoMart.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableCoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                table: "Coupons",
                newName: "MinPurchase");

            migrationBuilder.AddColumn<int>(
                name: "DiscountPersent",
                table: "Coupons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Coupons",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxDiscount",
                table: "Coupons",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPersent",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "MaxDiscount",
                table: "Coupons");

            migrationBuilder.RenameColumn(
                name: "MinPurchase",
                table: "Coupons",
                newName: "DiscountAmount");
        }
    }
}
