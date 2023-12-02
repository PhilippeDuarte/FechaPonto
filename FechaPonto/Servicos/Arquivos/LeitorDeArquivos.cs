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
			_utilitarios= new Formatadores();
		}
		private async Task<IEnumerable<PontoFuncionario>> RealizaLeitura(string caminhoDoArquivoASerLido)
		{
			if (string.IsNullOrEmpty(caminhoDoArquivoASerLido))
			{
				return null;
			}
			List<PontoFuncionario> listaPonto = new List<PontoFuncionario>();
			using StreamReader sr = new StreamReader(caminhoDoArquivoASerLido);
			while (!sr.EndOfStream)
			{
				string? linha = await sr.ReadLineAsync();
				if(linha is not null)
				{
					string[] propriedades = linha.Split(';');
					if (propriedades[0] != "Código")
					{
						PontoFuncionario funcionario = new PontoFuncionario();
						funcionario.Id = int.Parse(propriedades[0]);
						funcionario.Nome = propriedades[1];
						funcionario.ValorHora = _utilitarios.FormataDinheiro(propriedades[2]);
						funcionario.Data = Convert.ToDateTime(propriedades[3]);
						funcionario.Entrada = Convert.ToDateTime(propriedades[4]).TimeOfDay;
						funcionario.Saida = Convert.ToDateTime(propriedades[5]).TimeOfDay;
						funcionario.Almoco = _utilitarios.FormataDiferenca(propriedades[6]);
						listaPonto.Add(funcionario);
					}
				}
			}
			return listaPonto;
		}
		
		public async Task<IEnumerable<PontoFuncionario>> ObterTodosOsPontosPorSetor(string caminho)
		{
			var files = Directory.EnumerateFiles(caminho, "*.csv");
			
			var bag = new ConcurrentBag<PontoFuncionario>();
			var tasks = files.Select(async file =>
			{
				var response = await RealizaLeitura(file);
				foreach(var item in response)
				{
					bag.Add(item);
				}
				
			});
			
			await Task.WhenAll(tasks);
			return bag.ToList();
		}

	}
}
