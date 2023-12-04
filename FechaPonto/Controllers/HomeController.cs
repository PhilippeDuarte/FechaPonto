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

		public async Task<IActionResult> Index()
		{
			return View();
		}

		public IActionResult About()
		{
			return View();
		}
		[HttpGet]
		public async Task<IActionResult> ObterPontoPorSetor(string caminho)
		{
			List<PontoDepartamento> listaPontoDepartamento = new List<PontoDepartamento>();
			try
			{
                var files = Directory.EnumerateFiles(caminho, "*.csv");
                if (!files.Any())
                {
                    return NotFound("Nenhum arquivo encontrando dentro da pasta selecionada.");
                }
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