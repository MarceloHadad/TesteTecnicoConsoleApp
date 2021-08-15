using System;
using TesteTecnicoConsoleApp.Entities;
using TesteTecnicoConsoleApp.Services;

namespace TesteTecnicoConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourcePath = @"C:\repos\TesteTecnico\TesteHadad";
            var prodPath = sourcePath + @"\PRODUTOS.TXT";
            var vendasPath = sourcePath + @"\VENDAS.TXT";

            IOService iOService = new IOService();
            CentroOperacional co = new CentroOperacional();
            co.Produtos = iOService.LerProdutos(prodPath);
            co.Vendas = iOService.LerVendas(vendasPath);

            iOService.ValidaTransferencia(co);
            iOService.ValidaDivergencia(co);
            iOService.ValidaCanais(co);
        }
    }
}
