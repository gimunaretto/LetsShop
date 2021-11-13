using LetsShop.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace LetsShop.Repository
{
    public class DataBase : DbContext
    {
        public DataBase([NotNullAttribute] DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ModelCreate();
            modelBuilder.SeedData();
        }

        public DbSet<Produto> Produto { get; set; }
    }
}