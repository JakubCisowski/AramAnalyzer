namespace AramAnalyzer
{
	internal class Program
	{
		private static void Main()
		{
			Code.Data.DataResearch.Research.LoadChampionGroups();
			Code.Data.DataResearch.Research.GatherData("kaisa diff", "EpicNoob666", 250);

			Code.Data.DataResearch.Research.GetChampionGroupsWinrates();

			/*Console.WriteLine("Welcome to AramAnalyzer!\n");

			string nickname;
			string server;

			// Ask for nickname and region until it's correct.
			do
			{
				Console.WriteLine("Please enter your League of Legends nickname:");
				nickname = Console.ReadLine();

				Console.WriteLine("Please enter your League of Legends server (eune/euw):");
				server = Console.ReadLine();
			}
			while (Code.Riot.GetCurrentGame(nickname, server) == null);

			// Analyze current game.
			Code.Analyzer.Analyze();*/
		}
	}
}