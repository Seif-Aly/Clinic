using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic_Complex_Management_System1.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdForDoctorAndPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Patients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Doctors",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Doctors");
        }
    }
}
