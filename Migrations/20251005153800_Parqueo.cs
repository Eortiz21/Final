using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Primera.Migrations
{
    /// <inheritdoc />
    public partial class Parqueo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_EspacioEstacionamientos_Id_Espacio",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Vehiculos_NoPlaca",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_Id_Espacio",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_NoPlaca",
                table: "Tickets");

            migrationBuilder.AddColumn<int>(
                name: "EspacioEstacionamientoId_Espacio",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VehiculoNoPlaca",
                table: "Tickets",
                type: "nvarchar(20)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_EspacioEstacionamientoId_Espacio",
                table: "Tickets",
                column: "EspacioEstacionamientoId_Espacio");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_VehiculoNoPlaca",
                table: "Tickets",
                column: "VehiculoNoPlaca");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_EspacioEstacionamientos_EspacioEstacionamientoId_Espacio",
                table: "Tickets",
                column: "EspacioEstacionamientoId_Espacio",
                principalTable: "EspacioEstacionamientos",
                principalColumn: "Id_Espacio",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Vehiculos_VehiculoNoPlaca",
                table: "Tickets",
                column: "VehiculoNoPlaca",
                principalTable: "Vehiculos",
                principalColumn: "NoPlaca",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_EspacioEstacionamientos_EspacioEstacionamientoId_Espacio",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Vehiculos_VehiculoNoPlaca",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_EspacioEstacionamientoId_Espacio",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_VehiculoNoPlaca",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "EspacioEstacionamientoId_Espacio",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "VehiculoNoPlaca",
                table: "Tickets");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Id_Espacio",
                table: "Tickets",
                column: "Id_Espacio");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_NoPlaca",
                table: "Tickets",
                column: "NoPlaca");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_EspacioEstacionamientos_Id_Espacio",
                table: "Tickets",
                column: "Id_Espacio",
                principalTable: "EspacioEstacionamientos",
                principalColumn: "Id_Espacio",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Vehiculos_NoPlaca",
                table: "Tickets",
                column: "NoPlaca",
                principalTable: "Vehiculos",
                principalColumn: "NoPlaca",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
