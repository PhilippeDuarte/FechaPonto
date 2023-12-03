using System.ComponentModel.DataAnnotations.Schema;

namespace FechaPonto.Models
{
	public class PontoFuncionario
	{
		public int Id { get; set; }
		public string Nome { get; set; }
		[Column(TypeName = "decimal(18, 2)")]
		public double ValorHora { get; set; }
		public DateTime Data { get; set; }
		public TimeSpan Entrada { get; set; }
		public TimeSpan Saida { get; set; }
		public TimeSpan Almoco { get; set; }
	}
}
