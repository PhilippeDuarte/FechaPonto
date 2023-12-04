using FechaPonto.Models;

namespace FechaPonto.Servicos.Abstracoes
{
	public interface IGeradorDeRelatorios
	{		
		IEnumerable<Ponto> ObterRelatorioCompleto(IEnumerable<PontoDepartamento> pontoDepartamentos);
	}
}
