namespace BeneficiosPlataforma.Infrastructure.Persistence;

using Domain.Catalogo;
using Domain.Common;
using Domain.Interfaces;
using Domain.OrganizacaoHierarquica;
using Domain.Tenants;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using MultiTenancy;

public class AppDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<MasterDataRegistry> MasterDataRegistry { get; set; }
    public DbSet<GrupoEconomico> GruposEconomicos { get; set; }
    public DbSet<Estipulante> Estipulantes { get; set; }
    public DbSet<Subestipulante> Subestipulantes { get; set; }
    public DbSet<Operadora> Operadoras { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Plano> Planos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ApplyQueryFilters(modelBuilder);
        ConfigureEntities(modelBuilder);
    }

    private void ApplyQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tenantProperty = entityType.FindProperty(nameof(ITenantEntity.TenantId));
            var isDeletedProperty = entityType.FindProperty(nameof(BaseEntity.IsDeleted));

            if (tenantProperty != null)
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType);
                var tenantProp = System.Linq.Expressions.Expression.Property(parameter, nameof(ITenantEntity.TenantId));
                var tenantConstant = System.Linq.Expressions.Expression.Constant(_tenantContext.TenantId);
                var tenantEqual = System.Linq.Expressions.Expression.Equal(tenantProp, tenantConstant);

                System.Linq.Expressions.Expression finalFilter = tenantEqual;

                if (isDeletedProperty != null)
                {
                    var isDeletedProp = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                    var isDeletedConstant = System.Linq.Expressions.Expression.Constant(false);
                    var isDeletedEqual = System.Linq.Expressions.Expression.Equal(isDeletedProp, isDeletedConstant);
                    finalFilter = System.Linq.Expressions.Expression.AndAlso(tenantEqual, isDeletedEqual);
                }

                var lambda = System.Linq.Expressions.Expression.Lambda(finalFilter, parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    private void ConfigureEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(255);
            b.Property(x => x.Slug).IsRequired().HasMaxLength(100);
            b.Property(x => x.Status).IsRequired().HasMaxLength(50);
            b.HasIndex(x => x.Slug).IsUnique();
        });

        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Email).IsRequired().HasMaxLength(255);
            b.Property(x => x.PasswordHash).IsRequired();
            b.Property(x => x.Name).IsRequired().HasMaxLength(255);
            b.Property(x => x.Status).IsRequired().HasMaxLength(50);
            b.HasIndex(x => new { x.TenantId, x.Email }).IsUnique();
        });

        modelBuilder.Entity<Role>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(100);
            b.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
        });

        modelBuilder.Entity<Permission>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Code).IsRequired().HasMaxLength(100);
            b.Property(x => x.Name).IsRequired().HasMaxLength(255);
            b.HasIndex(x => x.Code).IsUnique();
        });

        modelBuilder.Entity<UserRole>(b =>
        {
            b.HasKey(x => new { x.UserId, x.RoleId });
            b.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne<Role>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RolePermission>(b =>
        {
            b.HasKey(x => new { x.RoleId, x.PermissionId });
            b.HasOne<Role>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne<Permission>().WithMany().HasForeignKey(x => x.PermissionId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OutboxMessage>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.EventType).IsRequired();
            b.Property(x => x.Payload).IsRequired();
            b.Property(x => x.Status).IsRequired().HasMaxLength(50);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.CreatedAt);
        });

        modelBuilder.Entity<AuditLog>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.EntityName).IsRequired().HasMaxLength(255);
            b.Property(x => x.Action).IsRequired().HasMaxLength(50);
            b.Property(x => x.OldValue).HasColumnType("jsonb");
            b.Property(x => x.NewValue).HasColumnType("jsonb");
            b.HasIndex(x => x.CreatedAt);
        });

        modelBuilder.Entity<MasterDataRegistry>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Domain).IsRequired().HasMaxLength(100);
            b.Property(x => x.MdmStatus).IsRequired().HasMaxLength(50);
            b.HasIndex(x => x.GlobalId);
            b.HasIndex(x => new { x.TenantId, x.LocalId });
        });

        modelBuilder.Entity<GrupoEconomico>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.Nome).IsRequired().HasMaxLength(255);
            b.Property(x => x.Responsavel).IsRequired().HasMaxLength(255);
            b.Property(x => x.Status).IsRequired().HasMaxLength(50);
            b.Property(x => x.IsDeleted).IsRequired();
            b.OwnsOne(x => x.CnpjRaiz, cnpj =>
            {
                cnpj.Property(c => c.Value).HasMaxLength(8).HasColumnName("CnpjRaiz");
            });
            b.HasIndex(x => new { x.TenantId, x.IsDeleted });
            b.HasIndex(x => new { x.TenantId, x.Status });
            b.HasIndex(x => new { x.TenantId, x.CnpjRaiz }).IsUnique().HasFilter("\"IsDeleted\" = false");
        });

        modelBuilder.Entity<Estipulante>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.RazaoSocial).IsRequired().HasMaxLength(255);
            b.Property(x => x.NomeFantasia).HasMaxLength(255);
            b.Property(x => x.GrupoEconomicoId);
            b.Property(x => x.Status).IsRequired().HasMaxLength(50);
            b.Property(x => x.IsDeleted).IsRequired();
            b.OwnsOne(x => x.Cnpj, cnpj =>
            {
                cnpj.Property(c => c.Value).HasMaxLength(14).HasColumnName("Cnpj");
            });
            b.OwnsOne(x => x.Endereco);
            b.OwnsOne(x => x.Telefone);
            b.OwnsOne(x => x.Email);
            b.HasOne<GrupoEconomico>()
                .WithMany()
                .HasForeignKey(x => x.GrupoEconomicoId)
                .OnDelete(DeleteBehavior.SetNull);
            b.HasIndex(x => new { x.TenantId, x.IsDeleted });
            b.HasIndex(x => new { x.TenantId, x.Status });
            b.HasIndex(x => new { x.TenantId, x.GrupoEconomicoId });
            b.HasIndex(x => new { x.TenantId, x.Cnpj }).IsUnique().HasFilter("\"IsDeleted\" = false");
        });

        modelBuilder.Entity<Subestipulante>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.RazaoSocial).IsRequired().HasMaxLength(255);
            b.Property(x => x.NomeFantasia).HasMaxLength(255);
            b.Property(x => x.EstipulanteId).IsRequired();
            b.Property(x => x.Status).IsRequired().HasMaxLength(50);
            b.Property(x => x.IsDeleted).IsRequired();
            b.OwnsOne(x => x.Cnpj, cnpj =>
            {
                cnpj.Property(c => c.Value).HasMaxLength(14).HasColumnName("Cnpj");
            });
            b.OwnsOne(x => x.Endereco);
            b.OwnsOne(x => x.Telefone);
            b.OwnsOne(x => x.Email);
            b.HasOne<Estipulante>()
                .WithMany()
                .HasForeignKey(x => x.EstipulanteId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(x => new { x.TenantId, x.IsDeleted });
            b.HasIndex(x => new { x.TenantId, x.Status });
            b.HasIndex(x => new { x.TenantId, x.EstipulanteId });
            b.HasIndex(x => new { x.TenantId, x.Cnpj }).IsUnique().HasFilter("\"IsDeleted\" = false");
        });

        modelBuilder.Entity<Operadora>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.RazaoSocial).IsRequired().HasMaxLength(255);
            b.Property(x => x.Tipo).IsRequired().HasMaxLength(50);
            b.Property(x => x.Status).IsRequired().HasMaxLength(50);
            b.Property(x => x.EndpointIntegracao).HasMaxLength(500);
            b.Property(x => x.FormatoIntegracao).HasMaxLength(100);
            b.Property(x => x.IsDeleted).IsRequired();
            b.OwnsOne(x => x.Cnpj, cnpj =>
            {
                cnpj.Property(c => c.Value).HasMaxLength(14).HasColumnName("Cnpj");
            });
            b.OwnsOne(x => x.RegistroAns, registroAns =>
            {
                registroAns.Property(c => c.Value).HasMaxLength(6).HasColumnName("RegistroAns");
            });
            b.HasIndex(x => new { x.TenantId, x.IsDeleted });
            b.HasIndex(x => new { x.TenantId, x.Status });
            b.HasIndex(x => new { x.TenantId, x.Cnpj }).IsUnique().HasFilter("\"IsDeleted\" = false");
        });

        modelBuilder.Entity<Produto>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.Nome).IsRequired().HasMaxLength(255);
            b.Property(x => x.OperadoraId).IsRequired();
            b.Property(x => x.TipoBeneficio).IsRequired().HasMaxLength(50);
            b.Property(x => x.Modalidade).IsRequired().HasMaxLength(50);
            b.Property(x => x.RegistroAnsProduto).HasMaxLength(20);
            b.Property(x => x.Status).IsRequired().HasMaxLength(50);
            b.Property(x => x.IsDeleted).IsRequired();
            b.HasOne<Operadora>()
                .WithMany()
                .HasForeignKey(x => x.OperadoraId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(x => new { x.TenantId, x.IsDeleted });
            b.HasIndex(x => new { x.TenantId, x.Status });
            b.HasIndex(x => new { x.TenantId, x.OperadoraId });
            b.HasIndex(x => new { x.TenantId, x.OperadoraId, x.Nome }).IsUnique().HasFilter("\"IsDeleted\" = false");
        });

        modelBuilder.Entity<Plano>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.Nome).IsRequired().HasMaxLength(255);
            b.Property(x => x.ProdutoId).IsRequired();
            b.Property(x => x.Cobertura).HasMaxLength(255);
            b.Property(x => x.AbrangenciaGeografica).HasMaxLength(255);
            b.Property(x => x.TipoAcomodacao).HasMaxLength(50);
            b.Property(x => x.ValorReferencia).HasColumnType("numeric(10,2)");
            b.Property(x => x.Status).IsRequired().HasMaxLength(50);
            b.Property(x => x.IsDeleted).IsRequired();
            b.HasOne<Produto>()
                .WithMany()
                .HasForeignKey(x => x.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(x => new { x.TenantId, x.IsDeleted });
            b.HasIndex(x => new { x.TenantId, x.Status });
            b.HasIndex(x => new { x.TenantId, x.ProdutoId });
            b.HasIndex(x => new { x.TenantId, x.ProdutoId, x.Nome }).IsUnique().HasFilter("\"IsDeleted\" = false");
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                entry.Entity.UpdateTimestamp();
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

public class User : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Status { get; set; } = "ACTIVE";
}

public class Role : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = null!;
}

public class Permission : BaseEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}

public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}

public class RolePermission
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}

public class AuditLog : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid? UserId { get; set; }
    public string EntityName { get; set; } = null!;
    public string Action { get; set; } = null!;
    public Guid EntityId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}

public class MasterDataRegistry : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid GlobalId { get; set; }
    public string Domain { get; set; } = null!;
    public Guid LocalId { get; set; }
    public string MdmStatus { get; set; } = null!;
    public DateTime? LastSyncAt { get; set; }
}
