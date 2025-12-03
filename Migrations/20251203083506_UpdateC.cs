using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CuciSepatu",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NamaSepatu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JenisSepatu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JenisLayanan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipeLayanan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TanggalCuci = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalHarga = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuciSepatu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuciSepatu_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CuciSepatu_UserId",
                table: "CuciSepatu",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CuciSepatu");
        }
    }
}
