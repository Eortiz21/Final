using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Primera.Migrations
{
    /// <inheritdoc />
    public partial class inni : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TipoVehiculos_Tarifas_TarifaId_Tarifa",
                table: "TipoVehiculos");

            migrationBuilder.DropIndex(
                name: "IX_TipoVehiculos_TarifaId_Tarifa",
                table: "TipoVehiculos");

            migrationBuilder.DropColumn(
                name: "Id_Tarifa",
                table: "TipoVehiculos");

            migrationBuilder.RenameColumn(
                name: "TarifaId_Tarifa",
                table: "TipoVehiculos",
                newName: "Ejes");

            migrationBuilder.AddColumn<string>(
                name: "Marca",
                table: "TipoVehiculos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tamano",
                table: "TipoVehiculos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Marca",
                table: "TipoVehiculos");

            migrationBuilder.DropColumn(
                name: "Tamano",
                table: "TipoVehiculos");

            migrationBuilder.RenameColumn(
                name: "Ejes",
                table: "TipoVehiculos",
                newName: "TarifaId_Tarifa");

            migrationBuilder.AddColumn<int>(
                name: "Id_Tarifa",
                table: "TipoVehiculos",
                type: "int",
                maxLength: 20,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TipoVehiculos_TarifaId_Tarifa",
                table: "TipoVehiculos",
                column: "TarifaId_Tarifa");

            migrationBuilder.AddForeignKey(
                name: "FK_TipoVehiculos_Tarifas_TarifaId_Tarifa",
                table: "TipoVehiculos",
                column: "TarifaId_Tarifa",
                principalTable: "Tarifas",
                principalColumn: "Id_Tarifa",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
