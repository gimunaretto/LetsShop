using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsShop.Model
{
    public class CarrinhoItem
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public double? TotalProduto { get; set; }
        public virtual ICollection<Produto> Produto { get; set; }
    }
}