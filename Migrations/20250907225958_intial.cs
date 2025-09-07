using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Primera.Migrations
{
    /// <inheritdoc />
    public partial class intial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehiculos_Clientes_ClienteId_Cliente",
                table: "Vehiculos");

            migrationBuilder.DropTable(
                name: "TipoVehiculoVehiculo");

            migrationBuilder.DropColumn(
                name: "Tipo_Vehiculo",
                table: "Vehiculos");

            migrationBuilder.RenameColumn(
                name: "ClienteId_Cliente",
                table: "Vehiculos",
                newName: "Id_Tipo");

            migrationBuilder.RenameIndex(
                name: "IX_Vehiculos_ClienteId_Cliente",
                table: "Vehiculos",
                newName: "IX_Vehiculos_Id_Tipo");

            migrationBuilder.AlterColumn<string>(
                name: "Marca",
                table: "Vehiculos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_Id_Cliente",
                table: "Vehiculos",
                column: "Id_Cliente");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehiculos_Clientes_Id_Cliente",
                table: "Vehiculos",
                column: "Id_Cliente",
                principalTable: "Clientes",
                principalColumn: "Id_Cliente",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehiculos_TipoVehiculos_Id_Tipo",
                table: "Vehiculos",
                column: "Id_Tipo",
                principalTable: "TipoVehiculos",
                principalColumn: "Id_Tipo",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehiculos_Clientes_Id_Cliente",
                table: "Vehiculos");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehiculos_TipoVehiculos_Id_Tipo",
                table: "Vehiculos");

            migrationBuilder.DropIndex(
                name: "IX_Vehiculos_Id_Cliente",
                table: "Vehiculos");

            migrationBuilder.RenameColumn(
                name: "Id_Tipo",
                table: "Vehiculos",
                newName: "ClienteId_Cliente");

            migrationBuilder.RenameIndex(
                name: "IX_Vehiculos_Id_Tipo",
                table: "Vehiculos",
                newName: "IX_Vehiculos_ClienteId_Cliente");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Vehiculos_Clientes_ClienteId_Cliente",
                table: "Vehiculos",
                column: "ClienteId_Cliente",
                principalTable: "Clientes",
                principalColumn: "Id_Cliente",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
