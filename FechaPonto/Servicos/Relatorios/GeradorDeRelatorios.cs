using FechaPonto.Models;
using FechaPonto.Servicos.Abstracoes;
using FechaPonto.Servicos.Utils;

namespace FechaPonto.Servicos.Relatorios
{
	public class GeradorDeRelatorios : IGeradorDeRelatorios
	{
		private readonly Formatadores _formatadores;
		public GeradorDeRelatorios()
		{
			_formatadores= new Formatadores();
		}
		public virtual async Task<Ponto> ObterRelatorioSetor(IEnumerable<PontoFuncionario> pontoFuncionarios)
		{
			Ponto pontoSetor = new Ponto();
			pontoSetor.Funcionarios = new List<Funcionario>();
			Funcionario? funcionario = new Funcionario();
			var codFuncionario = pontoFuncionarios.OrderBy(x => x.Id).ToList();
			int codAtual = codFuncionario[0].Id;
			foreach (PontoFuncionario pontoFuncionario in codFuncionario)
			{
				if (codAtual != pontoFuncionario.Id)
				{
					pontoSetor.Funcionarios.Add(funcionario);
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
				TimeSpan horasExtraouDebito = _formatadores.ObterHorasExtraOuDebito(pontoFuncionario.Entrada, pontoFuncionario.Saida, pontoFuncionario.Almoco);
				if (horasExtraouDebito.TotalHours > 0)
				{
					funcionario.HorasExtras += horasExtraouDebito;
				}
				else if(horasExtraouDebito.TotalHours < 0)
				{
					funcionario.HorasDebito += horasExtraouDebito;
				}
				if (_formatadores.ChecarDiaDaSemana(pontoFuncionario.Data))
				{
					funcionario.DiasExtra++;
				}
			}

			pontoSetor.Funcionarios.Add(funcionario);
			return pontoSetor;
		}

		
	}
}
