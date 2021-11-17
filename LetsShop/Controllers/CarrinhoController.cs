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
    [Route("api/Produto/{ProdutoId}")]
    [ApiController]
    public class CarrinhoController : Controller
    {
        [HttpGet("CarrinhoItem")]
        public IActionResult Get([FromRoute] int produtoId, [FromServices] DataBase dataBase)
        {
            var result = dataBase.CarrinhoItem.Include(x => x.Produto).Where(x => x.ProdutoId == produtoId);

            if (!result.Any())
                return StatusCode(204, string.Empty);

            return Ok(result);
        }

        [HttpPost("CarrinhoItem")]
        public IActionResult Post([FromRoute] int produtoId, [FromBody] CarrinhoItem carrinhoItem, [FromServices] DataBase dataBase)
        {
            if (!dataBase.Produto.Where(x => x.Id == produtoId).Any())
                return StatusCode(404, $"Produto id {produtoId} does not exist");

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

                dataBase.CarrinhoItem.Update(itemCarrinho);

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

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int carrinhoItemid, [FromServices] DataBase dataBase)
        {
            var carrinhoItemRemove = dataBase.CarrinhoItem.Where(x => x.Id == carrinhoItemid);

            dataBase.CarrinhoItem.RemoveRange(carrinhoItemRemove);

            dataBase.SaveChanges();

            return Ok();
        }
    }
}