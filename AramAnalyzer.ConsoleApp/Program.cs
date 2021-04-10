namespace AramAnalyzer
{
	internal class Program
	{
		private static void Main()
		{
			// Research section.

			// Load champion groups.
			Code.Data.DataResearch.Research.LoadChampionGroups();

			// Gather data.
			// 50 games/minute -> 3000/hour -> 72000/day -> 504000/week.
			// About 20-40% of gathered games tend to be repeated.
			Code.Data.DataResearch.Research.GatherData(RiotSharp.Misc.Region.Eune, "alooy", "rizeniik", 8000);

			// Generate data report.
			Code.Data.DataResearch.Research.GetChampionGroupsWinrates();

			// Game analyzer section.

			//Console.WriteLine("Welcome to AramAnalyzer!\n");

			//string nickname;
			//string server;

			//// Ask for nickname and region until it's correct.
			//do
			//{
			//	Console.WriteLine("Please enter your League of Legends nickname:");
			//	nickname = Console.ReadLine();

			//	Console.WriteLine("Please enter your League of Legends server (eune/euw):");
			//	server = Console.ReadLine();
			//}
			//while (Code.Riot.GetCurrentGame(nickname, server) == null);

			//// Analyze current game.
			//Code.Analyzer.Analyze();
		}
	}
}