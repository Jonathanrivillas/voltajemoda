using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoltajeModa.Migrations
{
    /// <inheritdoc />
    public partial class AmpliarDireccionYFixCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Direcciones_DireccionId",
                table: "Pedidos");

            migrationBuilder.AddColumn<bool>(
                name: "EsPredeterminada",
                table: "Direcciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Etiqueta",
                table: "Direcciones",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Casa");

            migrationBuilder.AddColumn<string>(
                name: "NombreDestinatario",
                table: "Direcciones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Direcciones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Direcciones_DireccionId",
                table: "Pedidos",
                column: "DireccionId",
                principalTable: "Direcciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Direcciones_DireccionId",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "EsPredeterminada",
                table: "Direcciones");

            migrationBuilder.DropColumn(
                name: "Etiqueta",
                table: "Direcciones");

            migrationBuilder.DropColumn(
                name: "NombreDestinatario",
                table: "Direcciones");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Direcciones");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Direcciones_DireccionId",
                table: "Pedidos",
                column: "DireccionId",
                principalTable: "Direcciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
