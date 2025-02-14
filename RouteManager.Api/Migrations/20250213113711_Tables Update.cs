using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RouteManager.Migrations
{
    /// <inheritdoc />
    public partial class TablesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RouteStops_Routes_RouteId",
                table: "RouteStops");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RouteStops",
                table: "RouteStops");

            migrationBuilder.EnsureSchema(
                name: "Routing");

            migrationBuilder.RenameTable(
                name: "Routes",
                newName: "Routes",
                newSchema: "Routing");

            migrationBuilder.RenameTable(
                name: "RouteStops",
                newName: "Stops",
                newSchema: "Routing");

            migrationBuilder.RenameColumn(
                name: "WindowStart",
                schema: "Routing",
                table: "Stops",
                newName: "TimeWindowBegin");

            migrationBuilder.RenameColumn(
                name: "WindowEnd",
                schema: "Routing",
                table: "Stops",
                newName: "TimeWindowEnd");

            migrationBuilder.RenameIndex(
                name: "IX_RouteStops_RouteId",
                schema: "Routing",
                table: "Stops",
                newName: "IX_Stops_RouteId");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DispatchTime",
                schema: "Routing",
                table: "Routes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<Guid>(
                name: "RouteId",
                schema: "Routing",
                table: "Stops",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "DwellTime",
                schema: "Routing",
                table: "Stops",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "Routing",
                table: "Stops",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stops",
                schema: "Routing",
                table: "Stops",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Locations",
                schema: "Routing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    References = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Metadata",
                schema: "Routing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    RouteId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metadata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Metadata_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "Routing",
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Metadata_RouteId",
                schema: "Routing",
                table: "Metadata",
                column: "RouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stops_Routes_RouteId",
                schema: "Routing",
                table: "Stops",
                column: "RouteId",
                principalSchema: "Routing",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stops_Routes_RouteId",
                schema: "Routing",
                table: "Stops");

            migrationBuilder.DropTable(
                name: "Locations",
                schema: "Routing");

            migrationBuilder.DropTable(
                name: "Metadata",
                schema: "Routing");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stops",
                schema: "Routing",
                table: "Stops");

            migrationBuilder.DropColumn(
                name: "DispatchTime",
                schema: "Routing",
                table: "Routes");

            migrationBuilder.RenameTable(
                name: "Routes",
                schema: "Routing",
                newName: "Routes");

            migrationBuilder.RenameTable(
                name: "Stops",
                schema: "Routing",
                newName: "RouteStops");

            migrationBuilder.RenameColumn(
                name: "TimeWindowEnd",
                table: "RouteStops",
                newName: "WindowEnd");

            migrationBuilder.RenameColumn(
                name: "TimeWindowBegin",
                table: "RouteStops",
                newName: "WindowStart");

            migrationBuilder.RenameIndex(
                name: "IX_Stops_RouteId",
                table: "RouteStops",
                newName: "IX_RouteStops_RouteId");

            migrationBuilder.AlterColumn<Guid>(
                name: "RouteId",
                table: "RouteStops",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "DwellTime",
                table: "RouteStops",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                oldClrType: typeof(TimeSpan),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "RouteStops",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RouteStops",
                table: "RouteStops",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RouteStops_Routes_RouteId",
                table: "RouteStops",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
