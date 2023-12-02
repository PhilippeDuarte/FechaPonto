using FechaPonto.Models;

namespace FechaPonto.Servicos.Abstracoes
{
	public interface IGeradorDeRelatorios
	{
		Task<Ponto> ObterRelatorioSetor(IEnumerable<PontoFuncionario> pontoFuncionarios);
	}
}
