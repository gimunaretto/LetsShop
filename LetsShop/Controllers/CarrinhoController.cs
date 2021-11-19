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
            var result = dataBase.CarrinhoItem.Where(x => x.Produto != null).ToList();

            foreach(var item in result)
            {
                item.Produto = dataBase.Produto.Where(x => x.Id == item.ProdutoId).FirstOrDefault();
            }

            if (result.Any())
                return Ok(result);
            else
                return StatusCode(204, string.Empty);
        }

        [HttpPost]
        [Route("adicionarCarrinho/{id}")]
        [AllowAnonymous]
        public IActionResult Post([FromRoute] int id, [FromBody] CarrinhoItem carrinhoItem, [FromServices] DataBase dataBase)
        {
            if (!dataBase.Produto.Where(x => x.Id == id).Any())
                return StatusCode(404, "Produto não existe.");       

            var produto = dataBase.Produto.Where(x => x.Id == id).FirstOrDefault();
            var idCarrinho = dataBase.CarrinhoItem.Where(x => x.Id == carrinhoItem.Id).FirstOrDefault();

            if (dataBase.CarrinhoItem.Where(x => x.ProdutoId == id).Any())
            {
                var itemCarrinho = dataBase.CarrinhoItem.Where(x => x.ProdutoId == id).FirstOrDefault();

                for (int i = 0; i < carrinhoItem.Quantidade; i++)
                {
                    itemCarrinho.TotalProduto += produto.Preco;
                }

                itemCarrinho.Quantidade += carrinhoItem.Quantidade;

                itemCarrinho.Produto = produto;

                dataBase.CarrinhoItem.UpdateRange(itemCarrinho);

                dataBase.SaveChanges();

                return Ok();
            }
            else
            {
                carrinhoItem.ProdutoId = id;
                carrinhoItem.Produto = produto;
                double? total = 0.0;

                if (idCarrinho != null)
                {
                    carrinhoItem.Id++;
                }

                for (int i = 0; i < carrinhoItem.Quantidade; i++)
                {
                    total += produto.Preco;
                }

                carrinhoItem.TotalProduto = total;

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
                return StatusCode(404, $"Item de id {id} não existe no carrinho.");

            var carrinhoItemRemove = dataBase.CarrinhoItem.Where(x => x.Id == id).FirstOrDefault();

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
                    var carrinhoItens = dataBase.CarrinhoItem.Where(x => x.Produto != null).ToList();
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

            foreach (var item in checkout.CarrinhoItem)
            {
                item.Produto = dataBase.Produto.Where(x => x.Id == item.ProdutoId).FirstOrDefault();
            }

            double? total = 0;

            foreach (var produto in carrinhoItens)
            {
                total += produto.TotalProduto;
            }

            checkout.Total = total;

            return Ok(checkout);
        }
    }
}