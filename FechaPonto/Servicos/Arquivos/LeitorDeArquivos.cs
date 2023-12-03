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
						if (propriedades[0] != "Código")
						{
							PontoFuncionario funcionario = new PontoFuncionario();
							funcionario.Id = int.Parse(propriedades[0]);
							funcionario.Nome = propriedades[1];
							funcionario.ValorHora = await _utilitarios.FormataDinheiro(propriedades[2]);
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
