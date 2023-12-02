using FechaPonto.Models;

namespace FechaPonto.Servicos.Abstracoes
{
	public interface ILeitorDeArquivos
	{		
		Task<IEnumerable<PontoFuncionario>> ObterTodosOsPontosPorSetor (string caminhoArquivos);
	}
}
