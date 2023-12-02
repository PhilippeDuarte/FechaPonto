using System.Xml.Schema;

namespace FechaPonto.Models
{
	public class Funcionario
	{
		public int Id { get; set; }
		public string Nome { get; set; }
		public double TotalReceber { get; set; } = 0;
		public TimeSpan HorasExtras { get; set; } = TimeSpan.Zero;
		public TimeSpan HorasDebito { get; set; } = TimeSpan.Zero;
		public int DiasFalta { get; set; } = 0;
		public int DiasExtra { get; set; } = 0;
		public int DiasTrabalhados { get; set; } = 0;
	}
}
