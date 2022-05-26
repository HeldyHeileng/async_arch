using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace analytics.Migrations
{
    public partial class creditdebet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Transactions",
                newName: "Debit");

            migrationBuilder.AddColumn<int>(
                name: "Credit",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Credit",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "Debit",
                table: "Transactions",
                newName: "Amount");
        }
    }
}
