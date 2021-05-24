using System;
using System.Collections.Generic;
using System.Linq;

namespace AramAnalyzer.Code
{
	public static class Analyzer
	{
		public static MatchReport Report { get; set; }

		public static Dictionary<string, double> BlueTeamChampions { get; set; }
		public static Dictionary<string, double> RedTeamChampions { get; set; }

		public static int BlueChampionPoints { get; set; }
		public static int RedChampionPoints { get; set; }
		public static int BlueTeamcompPoints { get; set; }
		public static int RedTeamcompPoints { get; set; }

		static Analyzer()
		{
			BlueTeamChampions = new Dictionary<string, double>();
			RedTeamChampions = new Dictionary<string, double>();
		}

		public static void Analyze()
		{
			Report = new MatchReport();

			AnalyzeWinrates();
			AnalyzeTeamcomps();
			DisplaySummary();
		}

		public static MatchReport GetMatchReport(string name, string region, bool platinumWinrates)
		{
			// Select which winrates to display.
			if (platinumWinrates)
			{
				Code.Leagueofgraphs.PlatinumWinrates = true;
			}
			else
			{
				Code.Leagueofgraphs.PlatinumWinrates = false;
			}

			try
			{
				_ = Code.Riot.GetCurrentGameWebsite(name, region);
			}
			catch (Exception)
			{
				throw;
			}

			Report = new MatchReport();

			var BlueTeamChampions = new List<(string Name, double Winrate, string DamageDealt, string DamageReceived)>();
			var RedTeamChampions = new List<(string Name, double Winrate, string DamageDealt, string DamageReceived)>();
			double BlueAverageWinrate;
			double RedAverageWinrate;
			double BlueTotalChampionPoints;
			double RedTotalChampionPoints;

			var BlueTeamTeamcomp = new List<(string Amount, string GroupName, string Points)>();
			var RedTeamTeamcomp = new List<(string Amount, string GroupName, string Points)>();
			string PlayerTeam;
			double BlueTotalTeamcompPoints = 0;
			double RedTotalTeamcompPoints = 0;

			#region CHAMPIONS

			// BLUE TEAM
			foreach (var participant in Riot.CurrentGame.Participants)
			{
				if (participant.TeamId == 100)
				{
					string championName = Ddragon.GetChampionName(participant.ChampionId);
					double championWinrate = Leagueofgraphs.GetWinrate(championName);

					(string, string) championBuffs = Wiki.GetChampionBuffsPair(championName);

					string damageDealt = championBuffs.Item1;
					string damageReceived = championBuffs.Item2;

					BlueTeamChampions.Add((Ddragon.GetChampionFullName(championName), championWinrate, damageDealt, damageReceived));
				}
			}

			BlueTotalChampionPoints = (Math.Round(((Math.Round(BlueTeamChampions.Select(x => x.Winrate).Average(), 1) - 50) * 100)));
			BlueAverageWinrate = Math.Round(BlueTeamChampions.Select(x => x.Winrate).Average(), 1);

			// RED TEAM
			foreach (var participant in Riot.CurrentGame.Participants)
			{
				if (participant.TeamId == 200)
				{
					string championName = Ddragon.GetChampionName(participant.ChampionId);
					double championWinrate = Leagueofgraphs.GetWinrate(championName);

					(string, string) championBuffs = Wiki.GetChampionBuffsPair(championName);

					string damageDealt = championBuffs.Item1;
					string damageReceived = championBuffs.Item2;

					RedTeamChampions.Add((Ddragon.GetChampionFullName(championName), championWinrate, damageDealt, damageReceived));
				}
			}
			RedTotalChampionPoints = (Math.Round(((Math.Round(RedTeamChampions.Select(x => x.Winrate).Average(), 1) - 50) * 100)));
			RedAverageWinrate = Math.Round(RedTeamChampions.Select(x => x.Winrate).Average(), 1);

			#endregion CHAMPIONS

			#region TEAMCOMPS

			// Load champion groups.
			Data.DataResearch.Research.LoadChampionGroups();

			// BLUE TEAM
			for (int i = 0; i < Data.DataResearch.Research.ChampionGroups.Count; i++)
			{
				var championGroupMembers = BlueTeamChampions.Select(x => x.Name).ToList().Intersect(Data.DataResearch.Research.ChampionGroups[i].Item1.ChampionNames);
				var points = Data.DataResearch.Research.ChampionGroups[i].Item1.Points[championGroupMembers.Count()];

				BlueTeamTeamcomp.Add((championGroupMembers.Count().ToString(), Data.DataResearch.Research.ChampionGroups[i].Item1.DisplayName, points.ToString()));

				BlueTotalTeamcompPoints += points;
			}

			// RED TEAM
			for (int i = 0; i < Data.DataResearch.Research.ChampionGroups.Count; i++)
			{
				var championGroupMembers = RedTeamChampions.Select(x => x.Name).ToList().Intersect(Data.DataResearch.Research.ChampionGroups[i].Item1.ChampionNames);
				var points = Data.DataResearch.Research.ChampionGroups[i].Item1.Points[championGroupMembers.Count()];

				RedTeamTeamcomp.Add((championGroupMembers.Count().ToString(), Data.DataResearch.Research.ChampionGroups[i].Item1.DisplayName, points.ToString()));

				RedTotalTeamcompPoints += points;
			}

			#endregion TEAMCOMPS

			#region SUMMARY

			if (Riot.IsPlayerBlueTeam)
			{
				PlayerTeam = "BLUE";
			}
			else
			{
				PlayerTeam = "RED";
			}

			#endregion SUMMARY

			Report.BlueTeamChampions = BlueTeamChampions;
			Report.RedTeamChampions = RedTeamChampions;
			Report.BlueTeamTeamcomp = BlueTeamTeamcomp;
			Report.RedTeamTeamcomp = RedTeamTeamcomp;
			Report.BlueAverageWinrate = BlueAverageWinrate;
			Report.RedAverageWinrate = RedAverageWinrate;
			Report.BlueTotalTeamcompPoints = BlueTotalTeamcompPoints;
			Report.RedTotalTeamcompPoints = RedTotalTeamcompPoints;
			Report.BlueTotalChampionPoints = BlueTotalChampionPoints;
			Report.RedTotalChampionPoints = RedTotalChampionPoints;
			Report.PlayerTeam = PlayerTeam;
			Report.PlayerName = name;

			return Report;
		}

		public static void AnalyzeWinrates()
		{
			// For each champion show its winrate in console.

			// BLUE TEAM

			Console.WriteLine();
			Console.WriteLine("------------------------------------------------------------------------------------------------");
			Console.BackgroundColor = ConsoleColor.Blue;
			Console.WriteLine(" BLUE TEAM \t\t WINRATE \t DAMAGE DEALT \t DAMAGE RECEIVED ");
			Console.ResetColor();
			foreach (var participant in Riot.CurrentGame.Participants)
			{
				if (participant.TeamId == 100)
				{
					string championName = Ddragon.GetChampionName(participant.ChampionId);
					double championWinrate = Leagueofgraphs.GetWinrate(championName);

					Console.Write($"{championName}{(championName.Length > 7 ? "\t" : "\t\t")}\t");

					// Select winrate text color.

					if (championWinrate >= 52)
					{
						Console.ForegroundColor = ConsoleColor.Green;
					}
					else if (championWinrate >= 50)
					{
						Console.ForegroundColor = ConsoleColor.Cyan;
					}
					else if (championWinrate >= 48)
					{
						Console.ForegroundColor = ConsoleColor.Magenta;
					}
					else
					{
						Console.ForegroundColor = ConsoleColor.Red;
					}

					Console.Write($"{championWinrate}");
					Console.ResetColor();

					string championBuffs = Wiki.GetChampionBuffs(championName);

					if (championBuffs[2] == '+')
					{
						// Champion is buffed.
						Console.ForegroundColor = ConsoleColor.Green;
					}
					else if (championBuffs[2] == '-')
					{
						// Champion is nerfed.
						Console.ForegroundColor = ConsoleColor.Red;
					}
					else if (championBuffs[4] == '-')
					{
						// Champion is buffed (only damage received).
						Console.ForegroundColor = ConsoleColor.Green;
					}
					else
					{
						// Either no changes, or only damage received nerf.
						Console.ForegroundColor = ConsoleColor.Red;
					}

					Console.WriteLine(championBuffs);
					Console.ResetColor();

					// Add champion with winrate to BlueTeam dictionary.
					BlueTeamChampions.Add(championName, championWinrate);
				}
			}
			Console.BackgroundColor = ConsoleColor.Blue;
			Console.WriteLine($" AVERAGE WINRATE: \t{Math.Round(BlueTeamChampions.Values.Average(), 1)}");
			Console.ResetColor();
			Console.WriteLine();

			BlueChampionPoints = (int)(Math.Round(((Math.Round(BlueTeamChampions.Values.Average(), 1) - 50) * 100)));

			// RED TEAM

			Console.BackgroundColor = ConsoleColor.Red;
			Console.WriteLine(" RED TEAM \t\t WINRATE \t DAMAGE DEALT \t DAMAGE RECEIVED ");
			Console.ResetColor();
			foreach (var participant in Riot.CurrentGame.Participants)
			{
				if (participant.TeamId == 200)
				{
					string championName = Ddragon.GetChampionName(participant.ChampionId);
					double championWinrate = Leagueofgraphs.GetWinrate(championName);

					Console.Write($"{championName}{(championName.Length > 7 ? "\t" : "\t\t")}\t");

					// Select winrate text color.

					if (championWinrate >= 52)
					{
						Console.ForegroundColor = ConsoleColor.Green;
					}
					else if (championWinrate >= 50)
					{
						Console.ForegroundColor = ConsoleColor.Cyan;
					}
					else if (championWinrate >= 48)
					{
						Console.ForegroundColor = ConsoleColor.Magenta;
					}
					else
					{
						Console.ForegroundColor = ConsoleColor.Red;
					}

					Console.Write($"{championWinrate}");
					Console.ResetColor();

					string championBuffs = Wiki.GetChampionBuffs(championName);

					if (championBuffs[2] == '+')
					{
						// Champion is buffed.
						Console.ForegroundColor = ConsoleColor.Green;
					}
					else if (championBuffs[2] == '-')
					{
						// Champion is nerfed.
						Console.ForegroundColor = ConsoleColor.Red;
					}
					else if (championBuffs[4] == '-')
					{
						// Champion is buffed (only damage received).
						Console.ForegroundColor = ConsoleColor.Green;
					}
					else
					{
						// Either no changes, or only damage received nerf.
						Console.ForegroundColor = ConsoleColor.Red;
					}

					Console.WriteLine(championBuffs);
					Console.ResetColor();

					// Add champion with winrate to RedTeam dictionary.
					RedTeamChampions.Add(championName, championWinrate);
				}
			}
			Console.BackgroundColor = ConsoleColor.Red;
			Console.WriteLine($" AVERAGE WINRATE: \t{Math.Round(RedTeamChampions.Values.Average(), 1)}");
			Console.ResetColor();

			RedChampionPoints = (int)(Math.Round(((Math.Round(RedTeamChampions.Values.Average(), 1) - 50) * 100)));

			Console.WriteLine("------------------------------------------------------------------------------------------------");
			Console.WriteLine();
		}

		public static void AnalyzeTeamcomps()
		{
			// Load champion groups.
			Data.DataResearch.Research.LoadChampionGroups();

			// BLUE TEAM

			Console.BackgroundColor = ConsoleColor.Blue;
			Console.WriteLine(" BLUE TEAM \t\t\t\t POINTS ");
			Console.ResetColor();

			for (int i = 0; i < Data.DataResearch.Research.ChampionGroups.Count; i++)
			{
				var championGroupMembers = BlueTeamChampions.Keys.ToList().Intersect(Data.DataResearch.Research.ChampionGroups[i].Item1.ChampionNames);
				var points = Data.DataResearch.Research.ChampionGroups[i].Item1.Points[championGroupMembers.Count()];

				Console.Write($"{championGroupMembers.Count()}x  {Data.DataResearch.Research.ChampionGroups[i].Item1.GroupName} {(Data.DataResearch.Research.ChampionGroups[i].Item1.GroupName.Length > 10 ? "\t\t\t" : "\t\t\t\t")}");
				Console.ForegroundColor = points > 0 ? ConsoleColor.Green : ConsoleColor.Red;
				if (points > 0)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine($"+{points}");
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"{points}");
				}
				Console.ResetColor();

				BlueTeamcompPoints += points;
			}

			Console.BackgroundColor = ConsoleColor.Blue;
			Console.WriteLine($" TOTAL TEAMCOMP POINTS: \t\t{BlueTeamcompPoints}");
			Console.ResetColor();
			Console.WriteLine();

			// RED TEAM

			Console.BackgroundColor = ConsoleColor.Red;
			Console.WriteLine(" RED TEAM \t\t\t\t POINTS ");
			Console.ResetColor();

			for (int i = 0; i < Data.DataResearch.Research.ChampionGroups.Count; i++)
			{
				var championGroupMembers = RedTeamChampions.Keys.ToList().Intersect(Data.DataResearch.Research.ChampionGroups[i].Item1.ChampionNames);
				var points = Data.DataResearch.Research.ChampionGroups[i].Item1.Points[championGroupMembers.Count()];

				Console.Write($"{championGroupMembers.Count()}x  {Data.DataResearch.Research.ChampionGroups[i].Item1.GroupName} {(Data.DataResearch.Research.ChampionGroups[i].Item1.GroupName.Length > 10 ? "\t\t\t" : "\t\t\t\t")}");
				if (points > 0)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine($"+{points}");
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"{points}");
				}
				Console.ResetColor();

				RedTeamcompPoints += points;
			}

			Console.BackgroundColor = ConsoleColor.Red;
			Console.WriteLine($" TOTAL TEAMCOMP POINTS: \t\t{RedTeamcompPoints}");
			Console.ResetColor();

			Console.WriteLine("------------------------------------------------------------------------------------------------");
			Console.WriteLine();
		}

		public static void DisplaySummary()
		{
			Console.WriteLine($"SUMMARY:\n");

			Console.Write("Player is on the ");

			if (Riot.IsPlayerBlueTeam)
			{
				Console.BackgroundColor = ConsoleColor.Blue;
				Console.Write("BLUE");
			}
			else
			{
				Console.BackgroundColor = ConsoleColor.Red;
				Console.Write("RED");
			}

			Console.ResetColor();
			Console.WriteLine(" Team.\n");

			// 	INDIVIDUAL CHAMPIONS

			Console.WriteLine($"Blue team individual champion points: {BlueChampionPoints}");
			Console.WriteLine($"Blue team comp points: {BlueTeamcompPoints}\n");

			// 	TEAMCOMPS

			Console.WriteLine($"Red team individual champion points: {RedChampionPoints}");
			Console.WriteLine($"Red team comp points: {RedTeamcompPoints}\n\n");

			// 	INDIVIDUAL CHAMPIONS

			Console.Write("Individual champions -> \t");

			int difference = BlueChampionPoints - RedChampionPoints;

			if (difference > 0)
			{
				Console.BackgroundColor = ConsoleColor.Blue;
				Console.Write($"BLUE");
				Console.ResetColor();

				Console.Write($" team champions are better by {difference} points. \n");
			}
			else if (difference < 0)
			{
				Console.BackgroundColor = ConsoleColor.Red;
				Console.Write($"RED");
				Console.ResetColor();

				Console.Write($" team champions are better by {-1 * difference} points. \n");
			}
			else
			{
				Console.Write("Both teams are even. \n");
			}

			// 	TEAMCOMPS

			Console.Write("Teamcomp -> \t\t\t");

			difference = BlueTeamcompPoints - RedTeamcompPoints;

			if (difference > 0)
			{
				Console.BackgroundColor = ConsoleColor.Blue;
				Console.Write($"BLUE");
				Console.ResetColor();

				Console.Write($" teamcomp is better by {difference} points. \n");
			}
			else if (difference < 0)
			{
				Console.BackgroundColor = ConsoleColor.Red;
				Console.Write($"RED");
				Console.ResetColor();

				Console.Write($" teamcomp is better by {-1 * difference} points. \n");
			}
			else
			{
				Console.Write("Both teams are even. \n");
			}

			Console.WriteLine();
			Console.WriteLine($"[100 points equal 1% in winrate]");

			Console.WriteLine("------------------------------------------------------------------------------------------------");
			Console.WriteLine();
		}
	}
}