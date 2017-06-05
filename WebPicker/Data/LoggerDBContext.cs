using Microsoft.EntityFrameworkCore;
using WebPicker.Models;

namespace WebPicker.Data
{
    public class LoggingDbContext : DbContext
    {
        public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options)
        {
        }

        public DbSet<Log> Log { get; set; }
    }
}