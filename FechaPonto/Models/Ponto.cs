using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace FechaPonto.Models
{
	public class Ponto
	{
		public string Departamento { get; set; }
		public string MesVigente { get; set; }
		public int AnoVigencia { get; set; }
		[Column(TypeName = "decimal(18, 2)")]
		public double TotalPagar { get; set; }
		[Column(TypeName = "decimal(18, 2)")]
		public double TotalDescontos { get; set; }
		[Column(TypeName = "decimal(18, 2)")]
		public double TotalExtras { get; set; }
		public List<Funcionario>? Funcionarios { get; set; } 
	}
}
