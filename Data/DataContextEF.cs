using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data;

public class DataContextEF : DbContext
{
    private readonly IConfiguration _config;

    public DataContextEF(IConfiguration config)
    {
        _config = config;
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserSalary> UserSalary { get; set; }
    public virtual DbSet<UserJobInfo> UserJobInfo { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // * need Microsoft.EntityFrameworkCore.SqlServer package for UseSqlServer method
            optionsBuilder
                .UseSqlServer(_config.GetConnectionString("DefaultConnection"),
                optionsBuilder => optionsBuilder.EnableRetryOnFailure());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // * setup logic to map schema to model
        // * need Microsoft.EntityFrameworkCore.Relational to use this method
        // * bundled separately for some reason
        modelBuilder.HasDefaultSchema("TutorialAppSchema");

        // * give proper mapping to table in SQL database - necessary because table name does not match model name
        // * also give unique id used as key
        modelBuilder.Entity<User>()
            .ToTable("Users", "TutorialAppSchema")
            .HasKey(user => user.UserId);

        modelBuilder.Entity<UserSalary>()
            .HasKey(user => user.UserId);

        modelBuilder.Entity<UserJobInfo>()
            .HasKey(user => user.UserId);
    }
}