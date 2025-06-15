using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ProjectSpetses.Models;
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
        public DbSet<PointsEarned> PointsEarned { get; set; }

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

            modelBuilder.Entity<PointsEarned>()
                .HasKey(p => p.Id);


            modelBuilder.Entity<Stats>().HasOne(s => s.User)
                .WithOne(s => s.Stats)
                .HasForeignKey<Stats>(s => s.Id);

            modelBuilder.Entity<Category>().HasOne(c => c.Section)
                .WithMany()
                .HasForeignKey(c => c.SectionId);

            modelBuilder.Entity<Content>().HasOne(c => c.Category)
                .WithMany()
                .HasForeignKey(c => c.CategoryId);

            modelBuilder.Entity<Quiz_item>().HasOne(c => c.Section)
                .WithMany()
                .HasForeignKey(c => c.SectionId);

            modelBuilder.Entity<Blank_item>().HasOne(c => c.Section)
                .WithMany()
                .HasForeignKey(c => c.SectionId);

            modelBuilder.Entity<Match_item>().HasOne(c => c.Section)
                .WithMany()
                .HasForeignKey(c => c.SectionId);

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

            modelBuilder.Entity<Stats>()
                .Property(s => s.CorrectAnswers)
                .HasConversion(
                    v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => v == null ? new List<StatAnswerViewModel>() : JsonSerializer.Deserialize<List<StatAnswerViewModel>>(v, (JsonSerializerOptions?)null)!);

            modelBuilder.Entity<Stats>()
                .Property(s => s.WrongAnswers)
                .HasConversion(
                    v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => v == null ? new List<StatAnswerViewModel>() : JsonSerializer.Deserialize<List<StatAnswerViewModel>>(v, (JsonSerializerOptions?)null)!);

            modelBuilder.Entity<PointsEarned>().HasOne(p => p.User)
                .WithOne(p => p.PointsEarned)
                .HasForeignKey<PointsEarned>(s => s.Id);


            base.OnModelCreating(modelBuilder);
        }
    }
}
