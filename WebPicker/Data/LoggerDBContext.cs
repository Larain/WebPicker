using Microsoft.EntityFrameworkCore;
using WebPicker.Models;

namespace WebPicker.Data
{
    public class LoggerDBContext : DbContext
    {
        public LoggerDBContext(DbContextOptions<LoggerDBContext> options) : base(options)
        {
        }

        public DbSet<Log> Log { get; set; }
    }
}