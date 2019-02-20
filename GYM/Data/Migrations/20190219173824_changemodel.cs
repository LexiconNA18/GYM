using Microsoft.EntityFrameworkCore.Migrations;

namespace GYM.Data.Migrations
{
    public partial class changemodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserGymClass_AspNetUsers_ApplicationUserId",
                table: "ApplicationUserGymClass");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserGymClass_GymClass_GymClassId",
                table: "ApplicationUserGymClass");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserGymClass",
                table: "ApplicationUserGymClass");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "ApplicationUserGymClass",
                newName: "UserGym");

            migrationBuilder.RenameColumn(
                name: "De",
                table: "GymClass",
                newName: "Description");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserGymClass_GymClassId",
                table: "UserGym",
                newName: "IX_UserGym_GymClassId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGym",
                table: "UserGym",
                columns: new[] { "ApplicationUserId", "GymClassId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserGym_AspNetUsers_ApplicationUserId",
                table: "UserGym",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGym_GymClass_GymClassId",
                table: "UserGym",
                column: "GymClassId",
                principalTable: "GymClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGym_AspNetUsers_ApplicationUserId",
                table: "UserGym");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGym_GymClass_GymClassId",
                table: "UserGym");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGym",
                table: "UserGym");

            migrationBuilder.RenameTable(
                name: "UserGym",
                newName: "ApplicationUserGymClass");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "GymClass",
                newName: "De");

            migrationBuilder.RenameIndex(
                name: "IX_UserGym_GymClassId",
                table: "ApplicationUserGymClass",
                newName: "IX_ApplicationUserGymClass_GymClassId");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserGymClass",
                table: "ApplicationUserGymClass",
                columns: new[] { "ApplicationUserId", "GymClassId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserGymClass_AspNetUsers_ApplicationUserId",
                table: "ApplicationUserGymClass",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserGymClass_GymClass_GymClassId",
                table: "ApplicationUserGymClass",
                column: "GymClassId",
                principalTable: "GymClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
