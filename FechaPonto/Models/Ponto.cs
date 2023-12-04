using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace FechaPonto.Models
{
	public class Ponto
	{
		public string Departamento { get; set; }
		public string MesVigente { get; set; }
		public int AnoVigencia { get; set; }
		public double TotalPagar { get; set; }
		public double TotalDescontos { get; set; }
		public double TotalExtras { get; set; }
		public List<Funcionario>? Funcionarios { get; set; } 
	}
}
