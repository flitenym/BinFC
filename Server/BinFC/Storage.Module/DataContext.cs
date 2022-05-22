using Microsoft.EntityFrameworkCore;
using Storage.Module.Entities;

namespace Storage.Module
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<UserInfo> UsersInfo { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Settings> Settings { get; set; }

        public DbSet<FuturesData> FuturesData { get; set; }
        public DbSet<SpotData> SpotData { get; set; }
        public DbSet<FuturesScale> FuturesScale { get; set; }
        public DbSet<SpotScale> SpotScale { get; set; }
        public DbSet<PayHistory> PayHistory { get; set; }
        public DbSet<Unique> Unique { get; set; }
    }
}