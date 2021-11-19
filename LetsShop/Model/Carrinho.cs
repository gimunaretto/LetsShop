using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsShop.Model
{
    public class Carrinho
    {
        public int Id { get; set; }
        public int CarrinhoId { get; set; }
        public double? Total { get; set; }
        public virtual List<CarrinhoItem> CarrinhoItem { get; set; }
    }
}