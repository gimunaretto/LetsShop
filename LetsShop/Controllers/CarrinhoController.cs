using LetsShop.Model;
using LetsShop.Repository;
using LetsShop.Repository.Model;
using LetsShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarrinhoController : Controller
    {
        [HttpGet]
        [Route("produtosCarrinho")]
        [AllowAnonymous]
        public IActionResult Get([FromServices] DataBase dataBase)
        {
            var result = dataBase.CarrinhoItem.Include(x => x.Produto).Select(x => x);

            if (result.Any())
                return Ok(result);
            else
                return StatusCode(204, string.Empty);
        }

        [HttpPost]
        [Route("adicionarCarrinho")]
        [AllowAnonymous]
        public IActionResult Post([FromRoute] int produtoId, [FromBody] CarrinhoItem carrinhoItem, [FromServices] DataBase dataBase)
        {
            if (!dataBase.Produto.Where(x => x.Id == produtoId).Any())
                return StatusCode(404, "Produto não existe.");

            if (dataBase.CarrinhoItem.Where(x => x.ProdutoId == produtoId).Any())
            {
                var itemCarrinho = dataBase.CarrinhoItem.Where(x => x.ProdutoId == produtoId).FirstOrDefault();

                foreach (var preco in itemCarrinho.Produto)
                {
                    for (int i = 0; i < carrinhoItem.Quantidade; i++)
                    {
                        itemCarrinho.TotalProduto += preco.Preco;
                    }
                }

                itemCarrinho.Quantidade += carrinhoItem.Quantidade;

                dataBase.CarrinhoItem.UpdateRange(itemCarrinho);

                dataBase.SaveChanges();

                return Ok();
            }
            else
            {
                var produto = dataBase.Produto.Where(x => x.Id == produtoId).FirstOrDefault();

                carrinhoItem.ProdutoId = produtoId;
                carrinhoItem.Produto.Add(produto);

                foreach (var preco in carrinhoItem.Produto)
                {
                    for (int i = 0; i < carrinhoItem.Quantidade; i++)
                    {
                        carrinhoItem.TotalProduto += preco.Preco;
                    }
                }

                dataBase.CarrinhoItem.Add(carrinhoItem);
                dataBase.SaveChanges();

                return Ok();
            }
        }

        [HttpDelete]
        [Route("delete/{id}")]
        [AllowAnonymous]
        public IActionResult Delete([FromRoute] int id, [FromServices] DataBase dataBase)
        {
            if (!dataBase.CarrinhoItem.Where(x => x.Id == id).Any())
                return StatusCode(404, "$Item de id {carrinhoItemId} não existe no carrinho.");

            var carrinhoItemRemove = dataBase.CarrinhoItem.Where(x => x.Id == id);

            dataBase.CarrinhoItem.RemoveRange(carrinhoItemRemove);

            dataBase.SaveChanges();

            return Ok();
        }

        [HttpDelete]
        [Route("empty")]
        [AllowAnonymous]
        public IActionResult EmptyCart([FromServices] DataBase dataBase)
        {
            try
            {
                if (dataBase.CarrinhoItem.Any())
                {
                    var carrinhoItens = dataBase.CarrinhoItem.Include(x => x.Produto).Select(x => x);
                    foreach (var item in carrinhoItens)
                    {
                        dataBase.CarrinhoItem.RemoveRange(item);
                        dataBase.SaveChanges();
                    }
                    return Ok("Seu carrinho foi esvaziado.");
                }
                else
                {
                    return Ok("Seu carrinho está vazio.");
                }
            }
            catch
            {
                return StatusCode(500, "Erro.");
            }
        }

        [HttpGet]
        [Route("cliente/checkout")]
        [Authorize(Roles = "cliente")]
        public IActionResult Checkout([FromServices] DataBase dataBase)
        {
            Carrinho checkout = new();

            var carrinhoItens = dataBase.CarrinhoItem.ToList();

            checkout.CarrinhoItem = carrinhoItens;

            double total = 0;

            foreach (var produto in carrinhoItens)
            {
                total += produto.TotalProduto;
            }

            checkout.Total = total;

            return Ok(checkout);
        }
    }
}