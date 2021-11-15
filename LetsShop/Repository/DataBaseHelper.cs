using LetsShop.Model;
using LetsShop.Repository.Model;
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

            modelBuilder.Entity<Carrinho>()
             .HasKey(r => new { r.Id, r.CarrinhoId, });

            modelBuilder.Entity<Carrinho>()
           .HasMany(c => c.CarrinhoItem);

            modelBuilder.Entity<CarrinhoItem>()
               .HasKey(r => new { r.Id, r.ProdutoId, });

            modelBuilder.Entity<CarrinhoItem>()
                .HasMany(c => c.Produto);
        }

        public static void SeedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>().HasData(new Produto[] {
                new Produto(){
                     Id = 1,
                     NomeProduto = "Teste",
                     Preco = 2.50
                }
            });
        }
    }
}