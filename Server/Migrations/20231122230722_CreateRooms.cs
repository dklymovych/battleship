using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Server.Models;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class CreateRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    GameCode = table.Column<string>(type: "varchar(6)", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    Player1Id = table.Column<int>(type: "integer", nullable: false),
                    Player2Id = table.Column<int>(type: "integer", nullable: true),
                    WinnerId = table.Column<int>(type: "integer", nullable: true),
                    GameStartedAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    GameEndedAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    ShipsPosition1 = table.Column<Dictionary<string, List<List<Coordinate>>>>(type: "json", nullable: false),
                    ShipsPosition2 = table.Column<Dictionary<string, List<List<Coordinate>>>>(type: "json", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.GameCode);
                    table.ForeignKey(
                        name: "FK_Rooms_Players_Player1Id",
                        column: x => x.Player1Id,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rooms_Players_Player2Id",
                        column: x => x.Player2Id,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rooms_Players_WinnerId",
                        column: x => x.WinnerId,
                        principalTable: "Players",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Player1Id",
                table: "Rooms",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Player2Id",
                table: "Rooms",
                column: "Player2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_WinnerId",
                table: "Rooms",
                column: "WinnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rooms");
        }
    }
}
