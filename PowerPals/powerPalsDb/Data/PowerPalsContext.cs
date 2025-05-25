using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using powerPalsDb.Models;

namespace powerPalsDb.Data;

public class PowerPalsContext: DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;
    public DbSet<Gym> Gyms { get; set; } = null!;
	public DbSet<GymGoer> GymGoers { get; set; } = null!;
	public DbSet<Friendship> Friendships { get; set; } = null!;
	public DbSet<Tag> Tags { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
		modelBuilder.Entity<Post>()
	        .Property(p => p.Date)
	        .HasDefaultValueSql("GETDATE()");

		modelBuilder.Entity<Review>()
	        .Property(p => p.CreatedAt)
	        .HasDefaultValueSql("GETDATE()");

		modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Gym)
            .WithMany(g => g.Reviews)
            .HasForeignKey(r => r.GymId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.User1)
            .WithMany(u => u.FriendshipsInitiated)
            .HasForeignKey(f => f.User1Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.User2)
            .WithMany(u => u.FriendshipsReceived)
            .HasForeignKey(f => f.User2Id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GymGoer>()
            .HasOne(gg => gg.User)
            .WithMany(u => u.GymGoers)
            .HasForeignKey(gg => gg.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GymGoer>()
            .HasOne(gg => gg.Gym)
            .WithMany(g => g.GymGoers)
            .HasForeignKey(gg => gg.GymId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("server=.;database=myDb2;trusted_connection=true;TrustServerCertificate=True");
    }
}
