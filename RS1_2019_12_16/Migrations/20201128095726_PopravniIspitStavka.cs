using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace RS1_2019_12_16.Migrations
{
    public partial class PopravniIspitStavka : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PopravniIspitStavka",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Bodovi = table.Column<int>(nullable: false),
                    IsPristupio = table.Column<bool>(nullable: true),
                    OdjeljenjeStavkaId = table.Column<int>(nullable: false),
                    PopravniIspitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PopravniIspitStavka", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PopravniIspitStavka_OdjeljenjeStavka_OdjeljenjeStavkaId",
                        column: x => x.OdjeljenjeStavkaId,
                        principalTable: "OdjeljenjeStavka",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PopravniIspitStavka_PopravniIspit_PopravniIspitId",
                        column: x => x.PopravniIspitId,
                        principalTable: "PopravniIspit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PopravniIspitStavka_OdjeljenjeStavkaId",
                table: "PopravniIspitStavka",
                column: "OdjeljenjeStavkaId");

            migrationBuilder.CreateIndex(
                name: "IX_PopravniIspitStavka_PopravniIspitId",
                table: "PopravniIspitStavka",
                column: "PopravniIspitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PopravniIspitStavka");
        }
    }
}
