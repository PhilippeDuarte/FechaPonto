using FechaPonto.Models;
using FechaPonto.Servicos.Abstracoes;
using Microsoft.AspNetCore.Mvc;
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
			var pontoFuncionarios= await _leitorDeArquivos.ObterTodosOsPontosPorSetor("C:\\Ponto");			
			var relatorio = await _geradorDeRelatorios.ObterRelatorioSetor(pontoFuncionarios);
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
		
	}
}