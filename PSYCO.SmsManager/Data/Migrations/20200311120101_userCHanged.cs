using Microsoft.EntityFrameworkCore.Migrations;

namespace PSYCO.SmsManager.Data.Migrations
{
    public partial class userCHanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                column: "ConcurrencyStamp",
                value: "f70c541c-dfb1-481f-86ec-33df7c0fddc3");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "edbdab85-d02c-463a-908e-976d609dfdde", "AQAAAAEAACcQAAAAECdWfBX/2u0ZjTSCerAgdFqWR5ESxwCTGpyDe4e5sisxCn06Oa3cRcemCOSN/SZV/A==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                column: "ConcurrencyStamp",
                value: "dc9eefcb-7cdb-432d-b3d6-ab2959f1d242");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "6a64e16b-97a8-4f7a-b178-6861bb1d71aa", "AQAAAAEAACcQAAAAEFBO6+VXiQStTnuPETL2SS2IBHP5NXhT3TIVtPqUjYvQSlstd+S5f+BiBwHfCZufLg==" });
        }
    }
}
