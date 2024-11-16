using api.Helpers;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data.DB;

public class InitDb : DbContext
{
    
    public virtual DbSet<Users> Users { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(Settings.ConnectionString, MariaDbServerVersion.LatestSupportedServerVersion);
        }
    }
}