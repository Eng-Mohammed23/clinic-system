using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClinicSystem.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedingRolesAndUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "92b75286-d8f8-4061-9995-e6e23ccdee94", "f51e5a91-bced-49c2-8b86-c2e170c0846c", false, false, "Admin", "ADMIN" },
                    { "9eaa03df-8e4f-4161-85de-0f6e5e30bfd4", "5ee6bc12-5cb0-4304-91e7-6a00744e042a", true, false, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedBy", "CreatedOn", "EdittingBy", "EdittingOn", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "MobileNumber", "NormalizedEmail", "NormalizedUserName", "Note", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "fd1f4b8e-3a54-4ade-84ff-348e64b415ca", 0, "99d2bbc6-bc54-4248-a172-a77de3ae4430", null, new DateTime(2024, 10, 11, 22, 2, 47, 317, DateTimeKind.Utc).AddTicks(2550), null, null, "admin@clinic-system.com", true, "Admin", false, null, null, "ADMIN@CLINIC-SYSTEM.COM", "ADMIN@CLINIC-SYSTEM.COM", null, "AQAAAAIAAYagAAAAEA+YTGs5aeA8c6N23ivYeVfg9Uu2xtCK958mqLLoNM+6whyEWXE7EnTrE2LYpmieAg==", null, false, "55BF92C9EF0249CDA210D85D1A851BC9", false, "admin@clinic-system.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "92b75286-d8f8-4061-9995-e6e23ccdee94", "fd1f4b8e-3a54-4ade-84ff-348e64b415ca" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9eaa03df-8e4f-4161-85de-0f6e5e30bfd4");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "92b75286-d8f8-4061-9995-e6e23ccdee94", "fd1f4b8e-3a54-4ade-84ff-348e64b415ca" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "92b75286-d8f8-4061-9995-e6e23ccdee94");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fd1f4b8e-3a54-4ade-84ff-348e64b415ca");
        }
    }
}
