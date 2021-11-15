using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace GameLib.rsc
{
    public class ProgramDbContext : DbContext
    {
        private readonly string ConnectionString =
            @"Data Source=rsc\Dictionary.db";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }


        public DbSet<Word> Words { get; set; }
        public DbSet<Player.ServerListView> Servers { get; set; }
        public DbSet<Player> Players { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Word>().HasKey(c => c.Id);
            modelBuilder.Entity<Word>().Property(c => c.Language)
                .HasConversion<int>();

            modelBuilder.Entity<Word>().ToTable("Words");

            var count = 0;
            foreach (var line in GetLinesAsync(
                File.ReadAllLines(@"rsc\fr.dic")))
            {
                if (line.Length >= 5 && line.Length <= 10 && count <= 10000)
                {
                    count++;
                    modelBuilder.Entity<Word>().HasData(new Word()
                    {
                        Id = count,
                        Language = Language.FR,
                        Text = line
                    });
                }
            }

            modelBuilder.Entity<Player>().HasKey(c => c.PlayerId);
            modelBuilder.Entity<Player>()
                .HasMany<Player.ServerListView>(b => b.Severs)
                .WithOne(b => b.Player);
            modelBuilder.Entity<Player>().Navigation(b => b.Severs)
                .UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);
            modelBuilder.Entity<Player>().ToTable("Players");


            modelBuilder.Entity<Player.ServerListView>().HasKey(c => c.Id);
            modelBuilder.Entity<Player.ServerListView>().ToTable("Servers");
        }

        private IEnumerable<string> GetLinesAsync(string[] lines)
        {
            foreach (var line in lines) yield return line;
        }
    }
}