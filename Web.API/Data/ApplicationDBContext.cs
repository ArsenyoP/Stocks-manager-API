using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web.API.Models;

namespace Web.API.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions) :
            base(dbContextOptions)
        {

        }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Portfolio>(x => x.HasKey(p => new { p.AppUserId, p.StockId }));

            builder.Entity<Portfolio>()
                .HasOne(u => u.AppUser)
                .WithMany(u => u.Portfolios)
                .HasForeignKey(p => p.AppUserId);

            builder.Entity<Portfolio>()
                .HasOne(u => u.Stock)
                .WithMany(u => u.Portfolios)
                .HasForeignKey(p => p.StockId);


            //indexes for comments with included properties Title, Content, AppUserId
            builder.Entity<Comment>()
                .HasIndex(c => new { c.StockID, c.CreatedOn })
                .IsDescending(false, true)
                .IncludeProperties(c => new { c.Title, c.Content, c.AppUserId })
                .HasDatabaseName("IX_Comments_StockID_CreatedOn_Covering");

            builder.Entity<Portfolio>()
                .HasIndex(c => new { c.AppUserId })
                .HasDatabaseName("IX_Portfolio_AppUserId");

            builder.Entity<Stock>()
                .HasIndex(s => s.Symbol)
                .IsUnique();

            builder.Entity<Stock>()
                .HasIndex(s => s.CompanyName);



            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER"
                },
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
