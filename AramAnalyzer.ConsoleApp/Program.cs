using System;

namespace AramAnalyzer
{
	internal class Program
	{
		private static void Main()
		{
			/*
			// Research section.

			// Gather data.
			// 50 games/minute -> 3000/hour -> 72000/day -> 504000/week.
			// About 20-40% of gathered games tend to be repeated.
			Code.Data.DataResearch.Research.GatherData(RiotSharp.Misc.Region.Eune, 36000);

			Code.Data.DataResearch.Research.DeleteRepeatedRows();

			//Generate data report.
			Code.Data.DataResearch.Research.GetChampionGroupsWinrates();
			*/

			// Game analyzer section.

			Console.WriteLine("Welcome to AramAnalyzer!\n");

			string nickname;
			string server;
			string platinumWinrates;

			// Ask for nickname and region until it's correct.
			do
			{
				Console.WriteLine("Please enter your League of Legends nickname:");
				nickname = Console.ReadLine();

				Console.WriteLine("Please enter your League of Legends server (eune/euw):");
				server = Console.ReadLine();

				Console.WriteLine("Do you want to check Platinum+ winrates? [y/n] (Default is Iron+)");
				platinumWinrates = Console.ReadLine();

				// Select which winrates to display.
				if (platinumWinrates == "y")
				{
					Code.Leagueofgraphs.PlatinumWinrates = true;
				}
				else
				{
					Code.Leagueofgraphs.PlatinumWinrates = false;
				}
			}
			while (Code.Riot.GetCurrentGame(nickname, server) == null);

			// Analyze current game.
			Code.Analyzer.Analyze();
		}
	}
}