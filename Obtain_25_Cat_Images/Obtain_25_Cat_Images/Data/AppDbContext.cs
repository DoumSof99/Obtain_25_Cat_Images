using Microsoft.EntityFrameworkCore;
using Obtain_25_Cat_Images.Models.Entities;

namespace Obtain_25_Cat_Images.Data {
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
        public DbSet<CatEntity> Cats { get; set; }
        public DbSet<TagEntity> Tags { get; set; }
        public DbSet<CatTagEntity> CatTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<CatTagEntity>()
                .HasKey(ct => new { ct.CatId, ct.TagId });

            modelBuilder.Entity<CatTagEntity>()
                .HasOne(ct => ct.Cat)
                .WithMany(c => c.CatTags)
                .HasForeignKey(ct => ct.CatId);

            modelBuilder.Entity<CatTagEntity>()
                .HasOne(ct => ct.Tag)
                .WithMany(t => t.CatTags)
                .HasForeignKey(ct => ct.TagId);
        }
    }
}
