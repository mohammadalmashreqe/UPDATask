using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class UpdateEmpPosTBL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeePosition_Employees_EmployeesID",
                table: "EmployeePosition");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeePosition_Positions_PositionsID",
                table: "EmployeePosition");

            migrationBuilder.RenameColumn(
                name: "PositionsID",
                table: "EmployeePosition",
                newName: "PositionID");

            migrationBuilder.RenameColumn(
                name: "EmployeesID",
                table: "EmployeePosition",
                newName: "EmployeeID");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeePosition_PositionsID",
                table: "EmployeePosition",
                newName: "IX_EmployeePosition_PositionID");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeePosition_Employees_EmployeeID",
                table: "EmployeePosition",
                column: "EmployeeID",
                principalTable: "Employees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeePosition_Positions_PositionID",
                table: "EmployeePosition",
                column: "PositionID",
                principalTable: "Positions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeePosition_Employees_EmployeeID",
                table: "EmployeePosition");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeePosition_Positions_PositionID",
                table: "EmployeePosition");

            migrationBuilder.RenameColumn(
                name: "PositionID",
                table: "EmployeePosition",
                newName: "PositionsID");

            migrationBuilder.RenameColumn(
                name: "EmployeeID",
                table: "EmployeePosition",
                newName: "EmployeesID");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeePosition_PositionID",
                table: "EmployeePosition",
                newName: "IX_EmployeePosition_PositionsID");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeePosition_Employees_EmployeesID",
                table: "EmployeePosition",
                column: "EmployeesID",
                principalTable: "Employees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeePosition_Positions_PositionsID",
                table: "EmployeePosition",
                column: "PositionsID",
                principalTable: "Positions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
