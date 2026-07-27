using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lightClinic.Patient.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Cities_CityId",
                schema: "dbo",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CityId",
                schema: "dbo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CityId",
                schema: "dbo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Latitude",
                schema: "dbo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Longitude",
                schema: "dbo",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                schema: "dbo",
                table: "Addresses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                schema: "dbo",
                table: "Addresses",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                schema: "dbo",
                table: "Addresses",
                type: "float",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CityId",
                schema: "dbo",
                table: "Addresses",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_AspNetUsers_UserId",
                schema: "dbo",
                table: "Addresses",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Cities_CityId",
                schema: "dbo",
                table: "Addresses",
                column: "CityId",
                principalSchema: "dbo",
                principalTable: "Cities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_AspNetUsers_UserId",
                schema: "dbo",
                table: "Cards",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_AspNetUsers_UserId",
                schema: "dbo",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Cities_CityId",
                schema: "dbo",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Cards_AspNetUsers_UserId",
                schema: "dbo",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_CityId",
                schema: "dbo",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "CityId",
                schema: "dbo",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "Latitude",
                schema: "dbo",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "Longitude",
                schema: "dbo",
                table: "Addresses");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                schema: "dbo",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                schema: "dbo",
                table: "AspNetUsers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                schema: "dbo",
                table: "AspNetUsers",
                type: "float",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CityId",
                schema: "dbo",
                table: "AspNetUsers",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Cities_CityId",
                schema: "dbo",
                table: "AspNetUsers",
                column: "CityId",
                principalSchema: "dbo",
                principalTable: "Cities",
                principalColumn: "Id");
        }
    }
}
