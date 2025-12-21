using Microsoft.EntityFrameworkCore;
using AuditService.Models;

namespace AuditService.Data
{
    public class AuditDbContext : DbContext
    {
        public AuditDbContext(DbContextOptions<AuditDbContext> options)
            : base(options)
        {
        }

        public DbSet<AuditLog> AuditLogs { get; set; }
    }
}
