using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Client_Manager_App_Database.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Country",
                table: "Clients",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditingType",
                table: "Clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Clients",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasAgency",
                table: "Clients",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsScammer",
                table: "Clients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PaymentType",
                table: "Clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReWorkingRate",
                table: "Clients",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "EditingType",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "HasAgency",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IsScammer",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ReWorkingRate",
                table: "Clients");
        }
    }
}
