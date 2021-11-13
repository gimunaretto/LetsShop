using LetsShop.Model;
using LetsShop.Repository;
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
    public class ProdutoController : Controller
    {
        [HttpGet]
        public IActionResult Get([FromServices] DataBase dataBase)
        {
            var result = dataBase.Produto.Include(x => x.NomeProduto).Select(x => x);

            if (result.Any())
                return Ok(result);
            else
                return StatusCode(204, string.Empty);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Produto produto, [FromServices] DataBase dataBase)
        {
            dataBase.Add(produto);

            dataBase.SaveChanges();

            return Ok();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch([FromRoute] int id, [FromBody] Produto produto, [FromServices] DataBase dataBase)
        {
            if (string.IsNullOrWhiteSpace(produto.NomeProduto))
                return StatusCode(400, $"Missing Parameter {nameof(produto.NomeProduto)}");

            var produtoDb = dataBase.Produto.Where(x => x.Id == id).FirstOrDefault();

            if (produtoDb == null)
                return StatusCode(404, $"Produto id {id} does not exist");

            produtoDb.NomeProduto = produto.NomeProduto;

            dataBase.Update(produtoDb);

            dataBase.SaveChanges();

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id, [FromServices] DataBase dataBase)
        {
            var peopleToRemove = dataBase.Produto.Where(x => x.Id == id);

            dataBase.Produto.RemoveRange(peopleToRemove);

            dataBase.SaveChanges();

            return Ok();
        }
    }
}