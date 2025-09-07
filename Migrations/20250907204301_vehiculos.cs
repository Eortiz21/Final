using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Primera.Migrations
{
    /// <inheritdoc />
    public partial class vehiculos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TipoVehiculos_Vehiculos_VehiculoNoPlaca",
                table: "TipoVehiculos");

            migrationBuilder.DropIndex(
                name: "IX_TipoVehiculos_VehiculoNoPlaca",
                table: "TipoVehiculos");

            migrationBuilder.DropColumn(
                name: "NoPlaca",
                table: "TipoVehiculos");

            migrationBuilder.DropColumn(
                name: "VehiculoNoPlaca",
                table: "TipoVehiculos");

            migrationBuilder.AlterColumn<string>(
                name: "Marca",
                table: "Vehiculos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Tipo_Vehiculo",
                table: "Vehiculos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TipoVehiculoVehiculo",
                columns: table => new
                {
                    TiposVehiculoId_Tipo = table.Column<int>(type: "int", nullable: false),
                    VehiculosNoPlaca = table.Column<string>(type: "nvarchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoVehiculoVehiculo", x => new { x.TiposVehiculoId_Tipo, x.VehiculosNoPlaca });
                    table.ForeignKey(
                        name: "FK_TipoVehiculoVehiculo_TipoVehiculos_TiposVehiculoId_Tipo",
                        column: x => x.TiposVehiculoId_Tipo,
                        principalTable: "TipoVehiculos",
                        principalColumn: "Id_Tipo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TipoVehiculoVehiculo_Vehiculos_VehiculosNoPlaca",
                        column: x => x.VehiculosNoPlaca,
                        principalTable: "Vehiculos",
                        principalColumn: "NoPlaca",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TipoVehiculoVehiculo_VehiculosNoPlaca",
                table: "TipoVehiculoVehiculo",
                column: "VehiculosNoPlaca");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TipoVehiculoVehiculo");

            migrationBuilder.DropColumn(
                name: "Tipo_Vehiculo",
                table: "Vehiculos");

            migrationBuilder.AlterColumn<string>(
                name: "Marca",
                table: "Vehiculos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "NoPlaca",
                table: "TipoVehiculos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VehiculoNoPlaca",
                table: "TipoVehiculos",
                type: "nvarchar(20)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TipoVehiculos_VehiculoNoPlaca",
                table: "TipoVehiculos",
                column: "VehiculoNoPlaca");

            migrationBuilder.AddForeignKey(
                name: "FK_TipoVehiculos_Vehiculos_VehiculoNoPlaca",
                table: "TipoVehiculos",
                column: "VehiculoNoPlaca",
                principalTable: "Vehiculos",
                principalColumn: "NoPlaca",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
