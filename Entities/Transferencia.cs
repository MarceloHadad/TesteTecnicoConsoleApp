using System;
using System.Collections.Generic;
using System.Text;

namespace TesteTecnicoConsoleApp.Entities
{
    class Transferencia
    {
        public int CodProd { get; set; }
        public int QtdEstoque { get; set; }
        public int QtdMin { get; set; }
        public int QtdVendas { get; set; }
        public int EstoquePosVendas { get; set; }
        public int Necessario { get; set; }
        public int Transferir { get; set; }
    }
}
