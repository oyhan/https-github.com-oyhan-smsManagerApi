using Microsoft.EntityFrameworkCore.Migrations;

namespace PSYCO.SmsManager.Data.Migrations
{
    public partial class userHolderAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HolderId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                column: "ConcurrencyStamp",
                value: "7e082970-8550-4b95-94a1-af76e7720207");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e8fca675-3c1a-43d9-97ae-a90f477ecb07", "AQAAAAEAACcQAAAAECvE5CPyfwm6QUd+no7RvjrCijTAu74f/fNPt/e0fq07P6MAdKLZVjBj+Rrq0PBkhw==" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_HolderId",
                table: "AspNetUsers",
                column: "HolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_HolderId",
                table: "AspNetUsers",
                column: "HolderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_HolderId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_HolderId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "HolderId",
                table: "AspNetUsers");

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
    }
}
