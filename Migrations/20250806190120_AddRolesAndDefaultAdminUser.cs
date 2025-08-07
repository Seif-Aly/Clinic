using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Clinic_Complex_Management_System1.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesAndDefaultAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("7dbc8218-e029-4644-a1ca-b27f30fb8a03"), null, "Admin", "ADMIN" },
                    { new Guid("bdc3d729-461f-4f56-8670-07f4e7174854"), null, "Doctor", "DOCTOR" },
                    { new Guid("f9a500aa-4587-4aea-8795-f52fd4fb8581"), null, "Patient", "PATIENT" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("efd76860-e9b8-4a2e-a008-4d0bd4b6bf34"), 0, "admin-concurrency-stamp", "admin@clinic.com", true, false, null, "ADMIN@CLINIC.COM", "ADMIN", "AQAAAAIAAYagAAAAEE9kuPBZ2JrMW2m6pBLqmawlspU09L0WKUg5hetgTNTIBMMFtLcMM7Kwd8ABzw6uIg==", null, false, "admin-security-stamp", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("7dbc8218-e029-4644-a1ca-b27f30fb8a03"), new Guid("efd76860-e9b8-4a2e-a008-4d0bd4b6bf34") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("bdc3d729-461f-4f56-8670-07f4e7174854"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("f9a500aa-4587-4aea-8795-f52fd4fb8581"));

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("7dbc8218-e029-4644-a1ca-b27f30fb8a03"), new Guid("efd76860-e9b8-4a2e-a008-4d0bd4b6bf34") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("7dbc8218-e029-4644-a1ca-b27f30fb8a03"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("efd76860-e9b8-4a2e-a008-4d0bd4b6bf34"));
        }
    }
}
