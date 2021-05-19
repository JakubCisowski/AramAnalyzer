using System.Collections.Generic;

namespace AramAnalyzer.Code
{
	public class MatchReport
	{
		public string PlayerTeam { get; set; }

		public List<(string Name, double Winrate, string DamageDealt, string DamageReceived)> BlueTeamChampions { get; set; }
		public List<(string Name, double Winrate, string DamageDealt, string DamageReceived)> RedTeamChampions { get; set; }
		public List<(string Amount, string GroupName, string Points)> BlueTeamTeamcomp { get; set; }
		public List<(string Amount, string GroupName, string Points)> RedTeamTeamcomp { get; set; }

		public string BlueAverageWinrate { get; set; }
		public string RedAverageWinrate { get; set; }
		public double BlueTotalTeamcompPoints { get; set; }
		public double BlueTotalChampionPoints { get; set; }
		public double RedTotalTeamcompPoints { get; set; }
		public double RedTotalChampionPoints { get; set; }
	}
}