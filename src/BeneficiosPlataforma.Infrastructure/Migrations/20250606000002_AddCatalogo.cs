using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeneficiosPlataforma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Operadoras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    RazaoSocial = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    RegistroAns = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EndpointIntegracao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FormatoIntegracao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CredenciaisEncriptadas = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operadoras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OperadoraId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoBeneficio = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Modalidade = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RegistroAnsProduto = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produtos_Operadoras_OperadoraId",
                        column: x => x.OperadoraId,
                        principalTable: "Operadoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Planos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Cobertura = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AbrangenciaGeografica = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TipoAcomodacao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ValorReferencia = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Planos_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Operadoras_TenantId_Cnpj",
                table: "Operadoras",
                columns: new[] { "TenantId", "Cnpj" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Operadoras_TenantId_IsDeleted",
                table: "Operadoras",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Operadoras_TenantId_Status",
                table: "Operadoras",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_TenantId_IsDeleted",
                table: "Produtos",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_TenantId_OperadoraId",
                table: "Produtos",
                columns: new[] { "TenantId", "OperadoraId" });

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_TenantId_OperadoraId_Nome",
                table: "Produtos",
                columns: new[] { "TenantId", "OperadoraId", "Nome" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_TenantId_Status",
                table: "Produtos",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Planos_TenantId_IsDeleted",
                table: "Planos",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Planos_TenantId_ProdutoId",
                table: "Planos",
                columns: new[] { "TenantId", "ProdutoId" });

            migrationBuilder.CreateIndex(
                name: "IX_Planos_TenantId_ProdutoId_Nome",
                table: "Planos",
                columns: new[] { "TenantId", "ProdutoId", "Nome" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Planos_TenantId_Status",
                table: "Planos",
                columns: new[] { "TenantId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Planos");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Operadoras");
        }
    }
}
