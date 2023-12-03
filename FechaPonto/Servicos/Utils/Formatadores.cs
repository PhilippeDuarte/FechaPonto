namespace FechaPonto.Servicos.Utils
{
	public class Formatadores
	{
		public double FormataDinheiro(string valor)
		{
			string dinheiro = valor.Split()[1];
			return Convert.ToDouble(dinheiro);
		}
		public TimeSpan FormataDiferenca(string valor)
		{
			string[] tempo = valor.Split('-');
			return Convert.ToDateTime(tempo[1]).TimeOfDay - Convert.ToDateTime(tempo[0]).TimeOfDay;
		}
		public double ObterTotalAReceberDia(TimeSpan horaInicial, TimeSpan horaFinal, TimeSpan almoco, double valorHora)
		{
			double totalAReceberDia = ObterHorasDia(horaInicial, horaFinal, almoco).TotalHours * valorHora;
			return totalAReceberDia;
		}
		public TimeSpan ObterHorasExtraOuDebito(TimeSpan horaInicial, TimeSpan horaFinal, TimeSpan almoco, DateTime data)
		{

			TimeSpan horasExtraOuDebito = new TimeSpan();

			//Verifica se houve falta durante a semana e realiza o desconto
			if (horaFinal == horaInicial && !ChecarDiaDaSemana(data))
			{
				return horasExtraOuDebito.Add(new TimeSpan(-9, 0, 0));
			}
			horasExtraOuDebito = ObterHorasDia(horaInicial, horaFinal, almoco);
			
			//Se o trabalho for feito no final de semana é considerado o dia todo como extra, do contrário subtrai as horas normais.
			return ChecarDiaDaSemana(data) ? horasExtraOuDebito : horasExtraOuDebito.Add(new TimeSpan(-9, 0, 0));
		}
		public bool ChecarDiaDaSemana(DateTime data)
		{			
			if ((data.DayOfWeek == DayOfWeek.Sunday) || (data.DayOfWeek == DayOfWeek.Saturday))
			{
				return true;
			}
			return false;
		}
		private TimeSpan ObterHorasDia(TimeSpan horaInicial, TimeSpan horaFinal, TimeSpan almoco)
		{
			TimeSpan somaHoras = horaFinal - horaInicial - almoco;
			return somaHoras;
		}
		
	}
}
