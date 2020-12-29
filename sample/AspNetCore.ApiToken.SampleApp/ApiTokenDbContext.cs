using System;
using AspNetCore.ApiToken.SampleApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.ApiToken.SampleApp
{
    public class ApiTokenDbContext : DbContext
    {
        public ApiTokenDbContext(DbContextOptions<ApiTokenDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Entities.ApiToken> Token { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User { Id = 1, Name = "Allen", Password = "123456", Role = "Admin" });

            base.OnModelCreating(modelBuilder);
        }
    }
}