using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lightClinic.Patient.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConvertOTPIdToLong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Otps",
                schema: "dbo",
                table: "Otps");

            migrationBuilder.RenameTable(
                name: "Otps",
                schema: "dbo",
                newName: "AuthOtp",
                newSchema: "dbo");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "dbo",
                table: "AuthOtp",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthOtp",
                schema: "dbo",
                table: "AuthOtp",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthOtp",
                schema: "dbo",
                table: "AuthOtp");

            migrationBuilder.RenameTable(
                name: "AuthOtp",
                schema: "dbo",
                newName: "Otps",
                newSchema: "dbo");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "dbo",
                table: "Otps",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Otps",
                schema: "dbo",
                table: "Otps",
                column: "Id");
        }
    }
}
