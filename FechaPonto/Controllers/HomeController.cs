using FechaPonto.Models;
using FechaPonto.Servicos.Abstracoes;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Diagnostics;

namespace FechaPonto.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ILeitorDeArquivos _leitorDeArquivos;
		private readonly IGeradorDeRelatorios _geradorDeRelatorios;

		public HomeController(ILogger<HomeController> logger, ILeitorDeArquivos leitorDeArquivos, IGeradorDeRelatorios geradorDeRelatorios)
		{
			_logger = logger;
			_leitorDeArquivos = leitorDeArquivos;
			_geradorDeRelatorios = geradorDeRelatorios;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult About()
		{
			return View();
		}
        /// <summary>
        /// Obtém o caminho para os arquivos de ponto no diretório definido pelo usuário.
        /// Chama os métodos para formatar o arquivo de ponto e para gerar o relatório final.
        /// Retorna a versão final do relatório de ponto, como será enviado para o front-end.
        /// </summary>
        /// <param name="pontoDepartamentos"></param>
        /// <returns>Lista de pontos de funcionarios separados por setor</returns>
        /// <exception cref="InvalidCastException">Erro na conversão dos arquivos.</exception>
        /// <remarks>
        ///  Exemplo de retorno:
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
        [HttpGet]
		public async Task<IActionResult> ObterPontoPorSetor(string caminho)
		{
			List<PontoDepartamento> listaPontoDepartamento = new List<PontoDepartamento>();
			try
			{
				//tenta ler todos os arquivos no diretório enviado pelo usuário.
                var files = Directory.EnumerateFiles(caminho, "*.csv");
                if (!files.Any())
                {
                    return NotFound("Nenhum arquivo encontrando dentro da pasta selecionada.");
                }
				//Envia os arquivos para serem lidos e formatados pelo sistema.
                var tasks = files.Select(async file =>
                {
                    PontoDepartamento pontoDepartamento = new PontoDepartamento();
                    pontoDepartamento.PontoFuncionarios = new List<PontoFuncionario>();
                    pontoDepartamento.NomeArquivo = file;
                    var ponto = await _leitorDeArquivos.ObterTodosOsPontosPorSetor(file);
                    foreach (var _ponto in ponto)
                    {
                        pontoDepartamento.PontoFuncionarios.Add(_ponto);
                    }

                    listaPontoDepartamento.Add(pontoDepartamento);
                });

                await Task.WhenAll(tasks);
                var relatorio = _geradorDeRelatorios.ObterRelatorioCompleto(listaPontoDepartamento);
                return Ok(relatorio);
            } 
			catch (Exception ex)
			{
				return BadRequest("Houve um erro ao tentar ler o arquivo de Ponto, verifique o caminho da pasta e os arquivos a serem lidos e tente novamente.");
			}
		}
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
		
	}
}