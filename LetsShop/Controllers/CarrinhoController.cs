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
    [Route("api/Produto/{Produtoid}")]
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
        public IActionResult Post([FromRoute] int produtoId, [FromBody] CarrinhoItem carrinhoItem, [FromBody] int qtd, [FromServices] DataBase dataBase)
        {
            if (!dataBase.Produto.Where(x => x.Id == produtoId).Any())
                return StatusCode(404, $"Produto id {produtoId} does not exist");

            if (dataBase.CarrinhoItem.Where(x => x.ProdutoId == produtoId).Any())
                return StatusCode(404, $"Produto id {produtoId} already exist in the cart");

            var produto = dataBase.Produto.Where(x => x.Id == produtoId).FirstOrDefault();

            carrinhoItem.ProdutoId = produtoId;
            carrinhoItem.Produto.Add(produto);

            foreach (var preco in carrinhoItem.Produto)
            {
                for (int i = 0; i < qtd; i++)
                {
                    carrinhoItem.TotalProduto += preco.Preco;
                }
            }

            carrinhoItem.Quantidade = qtd;

            dataBase.CarrinhoItem.Add(carrinhoItem);

            dataBase.SaveChanges();

            return Ok();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch([FromRoute] int id, [FromBody] CarrinhoItem carrinhoItem, [FromServices] DataBase dataBase)
        {
            if (carrinhoItem.Quantidade == null)
                return StatusCode(400, $"Missing Parameter {nameof(carrinhoItem.Quantidade)}");

            var carrinhoItemDb = dataBase.CarrinhoItem.Where(x => x.Id == id).FirstOrDefault();

            if (carrinhoItemDb == null)
                return StatusCode(404, $"CarrinhoItem id {id} does not exist");

            carrinhoItemDb.Quantidade = carrinhoItem.Quantidade;

            dataBase.Update(carrinhoItemDb);

            dataBase.SaveChanges();

            return Ok();
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