using FechaPonto.Models;
using FechaPonto.Servicos.Abstracoes;
using FechaPonto.Servicos.Utils;

namespace FechaPonto.Servicos.Relatorios
{
	public class GeradorDeRelatorios : IGeradorDeRelatorios
	{
		private readonly Formatadores _formatadores;
		private double valorExtra;
		private double valorDebito;
		public GeradorDeRelatorios()
		{
			_formatadores= new Formatadores();
			valorExtra=0;
			valorDebito=0;
		}
		
		public async Task<IEnumerable<Ponto>> ObterRelatorioCompleto(IEnumerable<PontoDepartamento> pontoDepartamentos)
		{
			List<Ponto> result = new List<Ponto>();
			
			foreach (PontoDepartamento item in pontoDepartamentos)
			{
				Ponto ponto = new Ponto();
				ponto.Funcionarios = new List<Funcionario>();
				string[] valoresTitulo = item.NomeArquivo.Split('-');
				string[] titulo = valoresTitulo[0].Split('\\');
				ponto.Departamento = titulo[titulo.Length - 1];
				ponto.MesVigente= valoresTitulo[1];				
				ponto.AnoVigencia = int.Parse(valoresTitulo[2].Split('.')[0]);
				var funcionario = await ObterRelatorioSetor(item.PontoFuncionarios);
				ponto.Funcionarios.AddRange(funcionario);
				ponto.TotalPagar += funcionario.Sum(x => x.TotalReceber);
				ponto.TotalExtras = valorExtra;
				ponto.TotalDescontos = valorDebito;
				result.Add(ponto);
			}
			return result;
		}

		private async Task<List<Funcionario>> ObterRelatorioSetor(IEnumerable<PontoFuncionario> pontoFuncionarios)
		{
			List<Funcionario> pontoSetor = new List<Funcionario>();
			Funcionario? funcionario = new Funcionario();
			var codFuncionario = pontoFuncionarios.OrderBy(x => x.Id).ToList();
			int codAtual = codFuncionario[0].Id;
			foreach (PontoFuncionario pontoFuncionario in codFuncionario)
			{
				if (codAtual != pontoFuncionario.Id)
				{
					pontoSetor.Add(funcionario);
					funcionario = new Funcionario();
					codAtual = pontoFuncionario.Id;
				}
				if (funcionario.Id is 0)
				{
					funcionario.Id = pontoFuncionario.Id;
					funcionario.Nome = pontoFuncionario.Nome;
				}
				funcionario.DiasTrabalhados++;
				funcionario.TotalReceber += _formatadores.ObterTotalAReceberDia(pontoFuncionario.Entrada, pontoFuncionario.Saida, pontoFuncionario.Almoco, pontoFuncionario.ValorHora);
				TimeSpan horasExtraouDebito = _formatadores.ObterHorasExtraOuDebito(pontoFuncionario.Entrada, pontoFuncionario.Saida, pontoFuncionario.Almoco, pontoFuncionario.Data);
				if (horasExtraouDebito.TotalHours > 0)
				{
					funcionario.HorasExtras += horasExtraouDebito;
					valorExtra += horasExtraouDebito.TotalHours * pontoFuncionario.ValorHora;
				}
				else if(horasExtraouDebito.TotalHours < 0)
				{
					funcionario.HorasDebito += horasExtraouDebito;
					valorDebito += horasExtraouDebito.TotalHours * pontoFuncionario.ValorHora;
					if (horasExtraouDebito.TotalHours <= -9) funcionario.DiasFalta++;
				}
				if (_formatadores.ChecarDiaDaSemana(pontoFuncionario.Data))
				{
					funcionario.DiasExtra++;
				}
			}
			
			pontoSetor.Add(funcionario);
			return pontoSetor;
		}

		
	}
}
