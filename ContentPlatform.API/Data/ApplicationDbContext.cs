using ContentPlatform.API.Models;
using Microsoft.EntityFrameworkCore;

class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }

    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<Content> Contents { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("public");

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);
                
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(255);
                
            entity.Property(e => e.IsPublic)
                .IsRequired()
                .HasDefaultValue(false);
                
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .IsRequired();
                
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .IsRequired();
                
            entity.Property(e => e.DeletedAt)
                .IsRequired(false);

            // Add index on UserId for better query performance
            entity.HasIndex(e => e.UserId);
        });

        modelBuilder.Entity<Content>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.ProjectId)
                .IsRequired();
                
            entity.Property(e => e.Data)
                .IsRequired()
                .HasMaxLength(1024);
                
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(255);
                
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .IsRequired();
                
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .IsRequired();
                
            entity.Property(e => e.DeletedAt)
                .IsRequired(false);

            // Define relationship between Content and Project
            entity.HasOne<Project>()
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add composite index for common queries
            entity.HasIndex(e => new { e.ProjectId, e.UserId });
        });
    }
}