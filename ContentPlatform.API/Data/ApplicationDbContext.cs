using ContentPlatform.API.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
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

            entity.HasIndex(e => e.UserId);
        });

        modelBuilder.Entity<Content>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.ProjectId)
                .IsRequired(false);
                
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

            entity.HasOne<Project>()
                .WithMany(p => p.Contents)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entity.HasIndex(e => new { e.ProjectId, e.UserId });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);
                
            entity.HasIndex(e => e.Email)
                .IsUnique();
        });
    }
}