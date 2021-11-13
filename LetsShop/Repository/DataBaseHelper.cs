using LetsShop.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsShop.Repository
{
    internal static class DataBaseHelper
    {
        public static void ModelCreate(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>().HasKey(k => k.Id);
        }

        public static void SeedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>().HasData(new Produto[] {
                new Produto(){
                     Id = 1,
                      NomeProduto = "Teste",
                }
            });
        }
    }
}