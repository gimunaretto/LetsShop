using LetsShop.Model;
using LetsShop.Repository;
using LetsShop.Repository.Model;
using LetsShop.Services;
using Microsoft.AspNetCore.Authorization;
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
        [Route("listarProdutos")]
        [AllowAnonymous]
        public IActionResult Get([FromServices] DataBase dataBase)
        {
            var result = dataBase.Produto.Include(x => x.NomeProduto).Select(x => x);

            if (result.Any())
                return Ok(result);
            else
                return StatusCode(204, string.Empty);
        }

        [HttpPost]
        [Route("funcionario/criarProduto")]
        [Authorize(Roles = "funcionario")]
        public IActionResult Post([FromBody] Produto produto, [FromServices] DataBase dataBase)
        {
            var produtoExiste = dataBase.Produto.Where(x => x.NomeProduto == produto.NomeProduto);
            if (produtoExiste.Any())
            {
                return StatusCode(409, "Já existe um produto com esse nome na loja.");
            }
            else
            {
                dataBase.Add(produto);
                dataBase.SaveChanges();

                return Ok();
            }
        }

        [HttpPatch]
        [Route("funcionario/update/{id}")]
        [Authorize(Roles = "funcionario")]
        public IActionResult Patch([FromRoute] int id, [FromBody] Produto produto, [FromServices] DataBase dataBase)
        {
            if (string.IsNullOrWhiteSpace(produto.NomeProduto))
                return StatusCode(400, $"Parâmetro {nameof(produto.NomeProduto)} não preenchido.");

            var produtoDb = dataBase.Produto.Where(x => x.Id == id).FirstOrDefault();

            if (produtoDb == null)
                return StatusCode(404, $"Produto não existe.");

            produtoDb.NomeProduto = produto.NomeProduto;
            produtoDb.Preco = produto.Preco;

            dataBase.Update(produtoDb);

            dataBase.SaveChanges();

            return Ok();
        }

        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public IActionResult GetProductById([FromRoute] int id, [FromServices] DataBase dataBase)
        {
            var produtoExiste = dataBase.Produto.Where(x => x.Id == id);

            try
            {
                if (produtoExiste.Any())
                {
                    return Ok(produtoExiste.FirstOrDefault());
                }
                else
                {
                    return StatusCode(404, "Produto não existe.");
                }
            }
            catch
            {
                return StatusCode(500, "Erro.");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetProductByName([FromQuery] string nomeProduto, [FromServices] DataBase dataBase)
        {
            var produtoExiste = dataBase.Produto.Where(x => x.NomeProduto == nomeProduto);

            try
            {
                if (produtoExiste.Any())
                {
                    return Ok(produtoExiste.FirstOrDefault());
                }
                else
                {
                    return StatusCode(404, "Produto não cadastrado.");
                }
            }
            catch
            {
                return StatusCode(500, "Erro.");
            }
        }

        [HttpDelete]
        [Route("funcionario/delete/{id}")]
        [Authorize(Roles = "funcionario")]
        public IActionResult Delete([FromRoute] int id, [FromServices] DataBase dataBase)
        {
            var productToRemove = dataBase.Produto.Where(x => x.Id == id);

            dataBase.Produto.RemoveRange(productToRemove);

            dataBase.SaveChanges();

            return Ok();
        }
    }
}