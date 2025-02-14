using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RouteManager.Migrations
{
    /// <inheritdoc />
    public partial class AddConverters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "TimeWindowEnd",
                schema: "Routing",
                table: "Stops",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "TimeWindowBegin",
                schema: "Routing",
                table: "Stops",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<long>(
                name: "DwellTime",
                schema: "Routing",
                table: "Stops",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "DispatchTime",
                schema: "Routing",
                table: "Routes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "TimeWindowEnd",
                schema: "Routing",
                table: "Stops",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "TimeWindowBegin",
                schema: "Routing",
                table: "Stops",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "DwellTime",
                schema: "Routing",
                table: "Stops",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DispatchTime",
                schema: "Routing",
                table: "Routes",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER");
        }
    }
}
