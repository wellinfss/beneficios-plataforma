using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeneficiosPlataforma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizacaoHierarquica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GruposEconomicos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CnpjRaiz = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Responsavel = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GruposEconomicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estipulantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    RazaoSocial = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NomeFantasia = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    Endereco_Logradouro = table.Column<string>(type: "text", nullable: false),
                    Endereco_Numero = table.Column<string>(type: "text", nullable: false),
                    Endereco_Complemento = table.Column<string>(type: "text", nullable: true),
                    Endereco_Bairro = table.Column<string>(type: "text", nullable: false),
                    Endereco_Cidade = table.Column<string>(type: "text", nullable: false),
                    Endereco_Uf = table.Column<string>(type: "text", nullable: false),
                    Endereco_Cep = table.Column<string>(type: "text", nullable: false),
                    Telefone_Numero = table.Column<string>(type: "text", nullable: false),
                    Email_Endereco = table.Column<string>(type: "text", nullable: false),
                    GrupoEconomicoId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estipulantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estipulantes_GruposEconomicos_GrupoEconomicoId",
                        column: x => x.GrupoEconomicoId,
                        principalTable: "GruposEconomicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Subestipulantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    RazaoSocial = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NomeFantasia = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    Endereco_Logradouro = table.Column<string>(type: "text", nullable: true),
                    Endereco_Numero = table.Column<string>(type: "text", nullable: true),
                    Endereco_Complemento = table.Column<string>(type: "text", nullable: true),
                    Endereco_Bairro = table.Column<string>(type: "text", nullable: true),
                    Endereco_Cidade = table.Column<string>(type: "text", nullable: true),
                    Endereco_Uf = table.Column<string>(type: "text", nullable: true),
                    Endereco_Cep = table.Column<string>(type: "text", nullable: true),
                    Telefone_Numero = table.Column<string>(type: "text", nullable: true),
                    Email_Endereco = table.Column<string>(type: "text", nullable: true),
                    EstipulanteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subestipulantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subestipulantes_Estipulantes_EstipulanteId",
                        column: x => x.EstipulanteId,
                        principalTable: "Estipulantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Estipulantes_GrupoEconomicoId",
                table: "Estipulantes",
                column: "GrupoEconomicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Estipulantes_TenantId_GrupoEconomicoId",
                table: "Estipulantes",
                columns: new[] { "TenantId", "GrupoEconomicoId" });

            migrationBuilder.CreateIndex(
                name: "IX_Estipulantes_TenantId_Cnpj_Unique",
                table: "Estipulantes",
                columns: new[] { "TenantId", "Cnpj" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Estipulantes_TenantId_IsDeleted",
                table: "Estipulantes",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Estipulantes_TenantId_Status",
                table: "Estipulantes",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_GruposEconomicos_TenantId_IsDeleted",
                table: "GruposEconomicos",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_GruposEconomicos_TenantId_Status",
                table: "GruposEconomicos",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_GruposEconomicos_TenantId_CnpjRaiz_Unique",
                table: "GruposEconomicos",
                columns: new[] { "TenantId", "CnpjRaiz" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Subestipulantes_EstipulanteId",
                table: "Subestipulantes",
                column: "EstipulanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Subestipulantes_TenantId_EstipulanteId",
                table: "Subestipulantes",
                columns: new[] { "TenantId", "EstipulanteId" });

            migrationBuilder.CreateIndex(
                name: "IX_Subestipulantes_TenantId_IsDeleted",
                table: "Subestipulantes",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Subestipulantes_TenantId_Status",
                table: "Subestipulantes",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Subestipulantes_TenantId_Cnpj_Unique",
                table: "Subestipulantes",
                columns: new[] { "TenantId", "Cnpj" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subestipulantes");

            migrationBuilder.DropTable(
                name: "Estipulantes");

            migrationBuilder.DropTable(
                name: "GruposEconomicos");
        }
    }
}
