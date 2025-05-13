using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ProjectSpetses.Models.Entities;

namespace ProjectSpetses.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        //define tables
        public DbSet<User> Users { get; set; }
        public DbSet<Stats> Stats { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Content> Content { get; set; }
        public DbSet<Quiz_item> Quiz_items { get; set; }
        public DbSet<Blank_item> Blank_items { get; set; }
        public DbSet<Match_item> Match_items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //define keys

            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<Stats>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Section>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Category>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Content>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Quiz_item>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Blank_item>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Match_item>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Stats>().HasOne(s => s.User)
                .WithOne(s => s.Stats)
                .HasForeignKey<Stats>(s => s.Id);

            modelBuilder.Entity<Category>().HasOne(c => c.Section)
                .WithMany()
                .HasForeignKey(c => c.SectionId);

            modelBuilder.Entity<Content>().HasOne(c => c.Category)
                .WithMany()
                .HasForeignKey(c => c.CategoryId);

            modelBuilder.Entity<Quiz_item>().HasOne(c => c.Content)
                .WithMany()
                .HasForeignKey(c => c.ContentId);

            modelBuilder.Entity<Blank_item>().HasOne(c => c.Content)
                .WithMany()
                .HasForeignKey(c => c.ContentId);

            modelBuilder.Entity<Match_item>().HasOne(c => c.Content)
                .WithMany()
                .HasForeignKey(c => c.ContentId);

            //these work, trust me bro
            modelBuilder.Entity<Quiz_item>()
                .Property(q => q.Answers)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null));

            modelBuilder.Entity<Blank_item>()
                .Property(q => q.Answers)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null));

            base.OnModelCreating(modelBuilder);
        }
    }
}
