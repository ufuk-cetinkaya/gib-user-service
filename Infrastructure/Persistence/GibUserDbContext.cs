using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class GibUserDbContext : DbContext
{
    public GibUserDbContext(DbContextOptions<GibUserDbContext> options)
        : base(options)
    {
    }

    public DbSet<GibUser> GibUsers => Set<GibUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GibUser>(entity =>
        {
            entity.ToTable("GibUser");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Identifier)
                  .IsRequired()
                  .HasMaxLength(11);

            entity.Property(x => x.Title)
                  .IsRequired()
                  .HasMaxLength(256);

            entity.Property(x => x.UserType)
                  .IsRequired()
                  .HasMaxLength(4)
                  .HasConversion<string>();

            entity.Property(x => x.AccountType)
                  .IsRequired()
                  .HasMaxLength(16)
                  .HasConversion<string>();
            
            entity.Property(x => x.FirstCreationTime)
                  .IsRequired()
                  .HasMaxLength(32);

            entity.Property(x => x.DocumentType)
                  .IsRequired()
                  .HasMaxLength(16)
                  .HasConversion<string>();

            entity.Property(x => x.Unit)
                  .IsRequired()
                  .HasMaxLength(2)
                  .HasConversion<string>();

            entity.Property(x => x.Alias)
                  .IsRequired()
                  .HasMaxLength(128);

            entity.Property(x => x.AliasCreationTime)
                  .IsRequired()
                  .HasMaxLength(32);

            entity.Property(x => x.AliasDeletionTime)
                  .HasMaxLength(32);

            entity.Property(x => x.IsActive)
                  .IsRequired();
        });
    }
}