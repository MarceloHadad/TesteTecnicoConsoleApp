using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TesteTecnicoConsoleApp.Entities;
using System.Linq;
using System.Data;

namespace TesteTecnicoConsoleApp.Services
{
    class IOService
    {
        public List<Produto> LerProdutos(string caminho)
        {
            try
            {
                List<Produto> listProdutos = new List<Produto>();

                string[] produtos = File.ReadAllLines(caminho);

                foreach (string produto in produtos)
                {
                    Produto p = new Produto();

                    var prodAtual = produto.Split(";");
                    p.CodProd = int.Parse(prodAtual[0]);
                    p.QtdEstoque = int.Parse(prodAtual[1]);
                    p.QtdMin = int.Parse(prodAtual[2]);
                    listProdutos.Add(p);
                }
                return listProdutos;
            }
            catch (IOException e)
            {
                Console.WriteLine("Erro no PRODUTOS.txt!");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<Venda> LerVendas(string caminho)
        {
            try
            {
                List<Venda> listVendas = new List<Venda>();

                string[] vendas = File.ReadAllLines(caminho);

                foreach (string venda in vendas)
                {
                    Venda v = new Venda();

                    var vendaAtual = venda.Split(";");
                    v.CodProd = int.Parse(vendaAtual[0]);
                    v.QtdVendida = int.Parse(vendaAtual[1]);
                    v.SituacaoVenda = int.Parse(vendaAtual[2]);
                    v.CanalVenda = int.Parse(vendaAtual[3]);
                    listVendas.Add(v);
                }
                return listVendas;
            }
            catch (IOException e)
            {
                Console.WriteLine("Erro no VENDAS.txt!");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public void ValidaTransferencia(CentroOperacional co)
        {
            List<Transferencia> listT = new List<Transferencia>();
            foreach (Produto p in co.Produtos)
            {
                Transferencia t = new Transferencia();

                t.CodProd = p.CodProd;
                t.QtdEstoque = p.QtdEstoque;
                t.QtdMin = p.QtdMin;
                t.QtdVendas = co.Vendas.Where(v => v.CodProd == p.CodProd).Where(v => v.SituacaoVenda == 100 || v.SituacaoVenda == 102).Sum(v => v.QtdVendida);
                t.EstoquePosVendas = p.QtdEstoque - t.QtdVendas;

                if (p.QtdMin - t.EstoquePosVendas < 0)
                {
                    t.Necessario = 0;
                }

                else
                {
                    t.Necessario = p.QtdMin - t.EstoquePosVendas;
                }

                if (t.Necessario > 1 && t.Necessario < 10)
                {
                    t.Transferir = 10;
                }

                else
                {
                    if (t.Necessario < 0)
                    {
                        t.Transferir = 0;
                    }

                    else
                    {
                        t.Transferir = t.Necessario;
                    }
                }
                listT.Add(t);
            }

            try
            {
                var caminho = @"C:\repos\TesteTecnico\TesteHadad" + @"\transfere.txt";
                File.Delete(caminho);

                using (StreamWriter writer = File.CreateText(caminho))
                {
                    DataTable dt = new DataTable();

                    dt.Columns.Add("Produtos");
                    dt.Columns.Add("QtCO");
                    dt.Columns.Add("QtMin");
                    dt.Columns.Add("QtVendas");
                    dt.Columns.Add("Estq.após");
                    dt.Columns.Add("Necess.");
                    dt.Columns.Add("Transf. de");

                    foreach (Transferencia transfer in listT)
                    {
                        dt.Rows.Add(transfer.CodProd.ToString().PadRight(10),
                            transfer.QtdEstoque.ToString().PadRight(10),
                            transfer.QtdMin.ToString().PadRight(10),
                            transfer.QtdVendas.ToString().PadRight(10),
                            transfer.EstoquePosVendas.ToString().PadRight(10),
                            transfer.Necessario.ToString().PadRight(10),
                            transfer.Transferir.ToString().PadRight(10));
                    }

                    writer.WriteLine("Necessidade de Transferência Armazém para CO");
                    writer.WriteLine();

                    foreach (DataColumn col in dt.Columns)
                    {
                        writer.Write(col.ColumnName.PadRight(10));
                    }
                    writer.WriteLine();
                    writer.WriteLine("\t\t\t\t\t\t\t\t\t\t   Vendas\t\t\t Arm p/ CO");

                    foreach (DataRow dr in dt.Rows)
                    {
                        writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}",
                            dr["Produtos"],
                            dr["QtCO"],
                            dr["QtMin"],
                            dr["QtVendas"],
                            dr["Estq.após"],
                            dr["Necess."],
                            dr["Transf. de"]); ;
                    }



                    //foreach(DataRow row in table.Rows)
                    //{
                    //    writer.WriteLine(row.ItemArray);
                    //}
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Erro no transfere.txt");
                Console.WriteLine(e.Message);
            }
        }

        public void ValidaDivergencia(CentroOperacional co)
        {
            try
            {
                var caminho = @"C:\repos\TesteTecnico\TesteHadad" + @"\DIVERGENCIAS.txt";
                File.Delete(caminho);

                using (StreamWriter writer = File.CreateText(caminho))
                {
                    int linha = 1;
                    foreach (var v in co.Vendas)
                    {
                        var checkVendas = co.Produtos.Any(p => p.CodProd == v.CodProd);

                        if (!checkVendas)
                        {
                            writer.WriteLine("Linha {0} – Código de Produto não encontrado {1}", linha, v.CodProd);
                        }

                        else
                        {
                            if (v.SituacaoVenda == 135)
                            {
                                writer.WriteLine("Linha {0} – Venda cancelada", linha);
                            }

                            else
                            {
                                if (v.SituacaoVenda == 190)
                                {
                                    writer.WriteLine("Linha {0} – Venda não finalizada", linha);
                                }

                                else
                                {
                                    if (v.SituacaoVenda == 999)
                                    {
                                        writer.WriteLine("Linha {0} – Erro desconhecido. Acionar equipe de TI", linha);
                                    }
                                }
                            }
                        }

                        linha++;
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Erro no DIVERGENCIAS.txt");
                Console.WriteLine(e.Message);
            }
        }

        public void ValidaCanais(CentroOperacional co)
        {
            try
            {
                var caminho = @"C:\repos\TesteTecnico\TesteHadad" + @"\TOTCANAIS.txt";
                File.Delete(caminho);

                using (StreamWriter writer = File.CreateText(caminho))
                {
                    writer.WriteLine("Quantidades de Vendas por canal");
                    writer.WriteLine();

                    DataTable dt = new DataTable();

                    dt.Columns.Add("Canal");
                    dt.Columns.Add("QtVendas");

                    foreach (DataColumn col in dt.Columns)
                    {
                        writer.Write(col.ColumnName.PadRight(25));
                    }
                    writer.WriteLine();

                    var item1 = co.Vendas.Where(v => v.SituacaoVenda == 100 || v.SituacaoVenda == 102).Where(v => v.CanalVenda == 1).Sum(v => v.QtdVendida);
                    var item2 = co.Vendas.Where(v => v.SituacaoVenda == 100 || v.SituacaoVenda == 102).Where(v => v.CanalVenda == 2).Sum(v => v.QtdVendida);
                    var item3 = co.Vendas.Where(v => v.SituacaoVenda == 100 || v.SituacaoVenda == 102).Where(v => v.CanalVenda == 3).Sum(v => v.QtdVendida);
                    var item4 = co.Vendas.Where(v => v.SituacaoVenda == 100 || v.SituacaoVenda == 102).Where(v => v.CanalVenda == 4).Sum(v => v.QtdVendida);

                    dt.Rows.Add("1 - Representantes     ", item1.ToString().PadLeft(10));
                    dt.Rows.Add("2 - Website            ", item2.ToString().PadLeft(10));
                    dt.Rows.Add("3 - App móvel Android  ", item3.ToString().PadLeft(10));
                    dt.Rows.Add("4 - App móvel iPhone   ", item4.ToString().PadLeft(10));

                    foreach (DataRow dr in dt.Rows)
                    {
                        writer.WriteLine("{0}{1}", dr["Canal"], dr["QtVendas"]);
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Erro no DIVERGENCIAS.txt");
                Console.WriteLine(e.Message);
            }
        }
    }
}

