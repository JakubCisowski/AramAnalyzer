using System;

namespace AramAnalyzer.Code
{
	public static class Analyzer
	{
		public static void Analyze()
		{
			AnalyzeWinrates();
		}

		public static void AnalyzeWinrates()
		{
			// for each champion show its winrate in console.
			Console.WriteLine("BLUE TEAM");
			foreach (var participant in Riot.CurrentGame.Participants)
			{
				if (participant.TeamId == 100)
				{
					string championName = Ddragon.GetChampionName(participant.ChampionId);
					Console.WriteLine($"{championName}\t\t-\t{Leagueofgraphs.GetWinrate(championName)}");
				}
			}
			Console.WriteLine();

			Console.WriteLine("RED TEAM");
			foreach (var participant in Riot.CurrentGame.Participants)
			{
				if (participant.TeamId == 200)
				{
					string championName = Ddragon.GetChampionName(participant.ChampionId);
					Console.WriteLine($"{championName}\t\t-\t{Leagueofgraphs.GetWinrate(championName)}");
				}
			}
			Console.WriteLine();
		}
	}
}