using FechaPonto.Models;
using FechaPonto.Servicos.Abstracoes;
using FechaPonto.Servicos.Utils;
using System.Collections.Concurrent;

namespace FechaPonto.Servicos.Arquivos
{
    public class LeitorDeArquivos : ILeitorDeArquivos
    {
        private readonly Formatadores _utilitarios;
        public LeitorDeArquivos()
        {
            _utilitarios = new Formatadores();
        }
        /// <summary>
        /// Monta a lista de ponto batido por cada funcionário, quebra a linha que é uma string em várias variáveis com seus respectivos tipos.
        /// </summary>
        /// <param name="caminhoDoArquivoASerLido"></param>
        /// <returns>
        /// Exemplo de retorno:
        /// {
        ///		"Id": 1,
        ///		"Nome": "Philippe Duarte",
        ///		"ValorHora": 100.00,
        ///		"Data": "01/12/2023 00:00:00"
        ///		"Entrada": 8:00:00",
        ///		"Entrada": "18:00:00",
        ///		"Almoco": "01:00:00"	
        /// },
        /// {
        ///		"Id": 1,
        ///		"Nome": "Philippe Duarte",
        ///		"ValorHora": 100.00,
        ///		"Data": "02/12/2023 00:00:00"
        ///		"Entrada": 8:02:00",
        ///		"Entrada": "18:39:00",
        ///		"Almoco": "01:15:00"	
        /// },
        /// </returns>
        /// <exception cref="InvalidCastException">Erro na conversão dos arquivos.</exception>
        private async Task<IEnumerable<PontoFuncionario>> RealizaLeitura(string caminhoDoArquivoASerLido)
        {
            if (string.IsNullOrEmpty(caminhoDoArquivoASerLido))
            {
                return null;
            }
            try
            {
                List<PontoFuncionario> listaPonto = new List<PontoFuncionario>();
                using StreamReader sr = new StreamReader(caminhoDoArquivoASerLido);
                while (!sr.EndOfStream)
                {
                    string? linha = await sr.ReadLineAsync();
                    if (linha is not null)
                    {
                        string[] propriedades = linha.Split(';');
                        //Checa se a linha é cabeçalho
                        if (int.TryParse(propriedades[0], out int value))
                        {
                            PontoFuncionario funcionario = new PontoFuncionario();
                            funcionario.Id = int.Parse(propriedades[0]);
                            funcionario.Nome = propriedades[1];
                            funcionario.ValorHora = _utilitarios.FormataDinheiro(propriedades[2]);
                            funcionario.Data = Convert.ToDateTime(propriedades[3]);
                            funcionario.Entrada = Convert.ToDateTime(propriedades[4]).TimeOfDay;
                            funcionario.Saida = Convert.ToDateTime(propriedades[5]).TimeOfDay;
                            funcionario.Almoco = _utilitarios.FormataDiferencaAlmoco(propriedades[6]);
                            listaPonto.Add(funcionario);
                        }
                    }
                }
                return listaPonto;
            }
            catch (Exception ex)
            {
                throw new InvalidCastException("Conversão de valor inválido na lista: " + ex);
            }
        }
        /// <summary>
        /// Retorna todos os pontos de funcionários de um setor expecífico
        /// </summary>
        /// <param name="caminho">Endereço do compoutador local onde estão os arquivos de ponto</param>
        /// <returns>Lista do objeto PontoFuncionario</returns>
        public async Task<IEnumerable<PontoFuncionario>> ObterTodosOsPontosPorSetor(string caminho)
        {
            var bag = new ConcurrentBag<PontoFuncionario>();

            var response = await RealizaLeitura(caminho);
            foreach (var item in response)
            {
                bag.Add(item);
            }

            return bag.ToList();
        }

    }
}
