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
        /// <summary>
        /// Retorna a versão final do relatório de ponto, como será enviado para o front-end.
        /// Cria a lista de departamentos e cada objeto possui a lista com os dados de cada funcionário de seu respectivo departamento.
        /// </summary>
        /// <param name="pontoDepartamentos"></param>
        /// <returns>Lista de pontos de funcionarios separados por setor</returns>
        /// <exception cref="InvalidCastException">Erro na conversão dos arquivos.</exception>
        /// <remarks>
		///  Exemplo de requisição:
		///  {
        ///		"departamento": "Financeiro",
        ///		"mesVigente": "Dezembro",
        ///		"anoVigencia": 2023,
        ///		"totalPagar": 22409.04,
        ///		"totalDescontos": -141.28,
        ///		"totalExtras": 355.87,
        ///		"funcionarios": [
        ///		    {
        ///		        "id": 3,
        ///		        "nome": "Pedro Lages",
        ///		        "totalReceber": 6488.92,
        ///		        "horasExtras": "10:58:00",
        ///		        "horasDebito": "00:00:00",
        ///		        "diasFalta": 0,
        ///		        "diasExtra": 1,
        ///		        "diasTrabalhados": 22
        ///		    },
        ///		    {
        ///		        "id": 4,
        ///		        "nome": "Lara Almeida",
        ///		        "totalReceber": 15920.12,
        ///		        "horasExtras": "00:00:00",
        ///		        "horasDebito": "-01:35:00",
        ///		        "diasFalta": 0,
        ///		        "diasExtra": 0,
        ///		        "diasTrabalhados": 20
        ///		    }
        ///		]
		///	}
		/// </remarks>
        public async Task<IEnumerable<Ponto>> ObterRelatorioCompleto(IEnumerable<PontoDepartamento> pontoDepartamentos)
		{
			List<Ponto> result = new List<Ponto>();
			try
			{
				foreach (PontoDepartamento item in pontoDepartamentos)
				{
					Ponto ponto = new Ponto();
					ponto.Funcionarios = new List<Funcionario>();
					string[] valoresTitulo = item.NomeArquivo.Split('-');
					string[] titulo = valoresTitulo[0].Split('\\');
					ponto.Departamento = titulo[titulo.Length - 1];
					ponto.MesVigente = valoresTitulo[1];
					ponto.AnoVigencia = int.Parse(valoresTitulo[2].Split('.')[0]);

					var funcionario = await ObterRelatorioSetor(item.PontoFuncionarios);

					ponto.Funcionarios.AddRange(funcionario);
					ponto.TotalPagar += Math.Round(funcionario.Sum(x => x.TotalReceber), 2);
					ponto.TotalExtras = Math.Round(valorExtra, 2);
					ponto.TotalDescontos = Math.Round(valorDebito, 2);
					result.Add(ponto);
				}
				return result;
			}
			catch (Exception ex)
			{
				throw new InvalidCastException("Conversão de valor inválido na lista: " + ex);
			}
		}
        /// <summary>
        /// Gera uma lista de funcionários calculando seu total de horas e valores a receber.
        /// </summary>
        /// <param name="pontoFuncionarios"></param>
        /// <returns>Lista do objeto Funcionario</returns>
        private async Task<IEnumerable<Funcionario>> ObterRelatorioSetor(IEnumerable<PontoFuncionario> pontoFuncionarios)
		{
			List<Funcionario> pontoSetor = new List<Funcionario>();
			Funcionario? funcionario = new Funcionario();
			var codFuncionario = pontoFuncionarios.OrderBy(x => x.Id).ToList();
			int codAtual = codFuncionario[0].Id;
			foreach (PontoFuncionario pontoFuncionario in codFuncionario)
			{
				//Com a lista ordenada, verifica se o usuário mudou, nesse caso adiciona o funcionário na lista anterior e instancia um novo funcionário
				if (codAtual != pontoFuncionario.Id)
				{
					funcionario.TotalReceber = Math.Round(funcionario.TotalReceber, 2);
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
				//se for final de semana adiciona um dia extra
				if (_formatadores.ChecarDiaDaSemana(pontoFuncionario.Data))
				{
					funcionario.DiasExtra++;
				}
			}
			
			funcionario.TotalReceber = Math.Round(funcionario.TotalReceber, 2);
			pontoSetor.Add(funcionario);
			return pontoSetor;
		}

		
	}
}
