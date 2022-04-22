using Microsoft.EntityFrameworkCore;
using Storage.Module.Entities;
using System;

namespace Storage.Module
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<UserInfo> UserInfo { get; set; }
    }
}