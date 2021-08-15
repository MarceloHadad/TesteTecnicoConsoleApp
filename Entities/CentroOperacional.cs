using System;
using System.Collections.Generic;
using System.Text;

namespace TesteTecnicoConsoleApp.Entities
{
    class CentroOperacional
    {
        public List<Produto> Produtos { get; set; }
        public List<Venda> Vendas { get; set; }
        public List<Transferencia> Transferencias { get; set; }
    }
}
