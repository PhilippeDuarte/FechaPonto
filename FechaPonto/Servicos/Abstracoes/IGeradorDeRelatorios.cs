using FechaPonto.Models;

namespace FechaPonto.Servicos.Abstracoes
{
	public interface IGeradorDeRelatorios
	{		
		Task<IEnumerable<Ponto>> ObterRelatorioCompleto(IEnumerable<PontoDepartamento> pontoDepartamentos);
	}
}
