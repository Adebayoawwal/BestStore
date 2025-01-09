using Crud.Models;
using Microsoft.EntityFrameworkCore;
namespace Crud.Services
{
    public class ApplicationDbCotnext : DbContext
    {
        public ApplicationDbCotnext(DbContextOptions options) :base(options) 
        {
            
        }
        public DbSet<Product> Products { get; set; }
    }
}
