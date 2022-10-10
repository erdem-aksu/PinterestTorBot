using Microsoft.EntityFrameworkCore;
using PinterestTorBot.Models;

namespace PinterestTorBot.EntityFramework
{
    public class AppDbContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        { 
            optionsBuilder.UseSqlServer(@"Server=localhost; Database=ArticleDb; User=sa; Password=EaErx6m6;");
        }
    }
}