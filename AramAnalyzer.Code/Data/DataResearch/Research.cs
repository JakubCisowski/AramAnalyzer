using Newtonsoft.Json.Linq;
using RiotSharp;
using RiotSharp.Endpoints.MatchEndpoint;
using RiotSharp.Endpoints.SummonerEndpoint;
using RiotSharp.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AramAnalyzer.Code.Data.DataResearch
{
	public static class Research
	{
		public static List<(ChampionGroup, List<(int Wins, int Losses)>)> ChampionGroups { get; set; }
		private static RiotApi _api;
		private static Random _rng;

		// File paths.
		private static string DataPath = @"C:\Programowanie\Fun\AramAnalyzer\AramAnalyzer.ConsoleApp\bin\Debug\net5.0\Data\DataResearch\Games.csv";

		private static string ReportPath = @"C:\Programowanie\Fun\AramAnalyzer\AramAnalyzer.ConsoleApp\bin\Debug\net5.0\Data\DataResearch\DataReport.txt";

		private static string NicknamesPath = @"C:\Programowanie\Fun\AramAnalyzer\AramAnalyzer.ConsoleApp\bin\Debug\net5.0\Data\DataResearch\Nicknames.txt";

		// Request limits.
		private const int LimitPerSecond = 20;

		private const int LimitPerTwoMinutes = 100;

		static Research()
		{
			ChampionGroups = new List<(ChampionGroup, List<(int Wins, int Losses)>)>();

			_rng = new Random();

			// Get API key from resources.
			_api = RiotApi.GetDevelopmentInstance(Properties.Resources.Key);
		}

		// Loads champion groups from ChampionGroupsResearch.json into list of champion groups (champion group = list of strings/champions)
		public static void LoadChampionGroups()
		{
			ChampionGroups.Clear();

			var path = Path.Combine(Environment.CurrentDirectory, @"Data\DataResearch\", "ChampionGroupsResearch.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));

			LoadChampions(parsedObject, "ADC");
			LoadChampions(parsedObject, "Assassin");
			LoadChampions(parsedObject, "BattleCaster");
			LoadChampions(parsedObject, "BurstCaster");
			LoadChampions(parsedObject, "PokeCaster");
			LoadChampions(parsedObject, "BruiserDiver");
			LoadChampions(parsedObject, "BruiserJuggernaut");
			LoadChampions(parsedObject, "ProtectorTank");
			LoadChampions(parsedObject, "EngageTank");
			LoadChampions(parsedObject, "Catcher");
			LoadChampions(parsedObject, "Healer");
		}

		// Load single champion group to static list.
		public static void LoadChampions(JObject data, string championGroupName)
		{
			var championGroupArray = (JArray)data[championGroupName];
			var championGroup = new ChampionGroup(championGroupName);

			for (var i = 0; i < championGroupArray.Count; i++)
			{
				championGroup.ChampionNames.Add(championGroupArray[i].ToString());
			}

			var winLossPairs = new List<(int Wins, int Losses)>()
			{
				(0,0),
				(0,0),
				(0,0),
				(0,0),
				(0,0),
				(0,0)
			};

			ChampionGroups.Add((championGroup, winLossPairs));
		}

		public static void GatherData(Region region, int gameLimit)
		{
			Region researchRegion = region;

			// Currently analyzed nickname.
			string currentName = "";

			int gamesCounter = 0;

			// How many account do we gather data from.
			const int accountsAmount = 100000;

			// How many games do we search per account.
			const int accountGamesLimit = 500;

			// Gather data.

			using (StreamWriter writer = new StreamWriter(DataPath, true))
			{
				for (int i = 0; i < accountsAmount; i++)
				{
					// Randomize nickname.
					if (currentName == "")
					{
						currentName = GetRandomNickname();
					}

					// Find summoner.
					Summoner summoner;
					string summonerId = "";
					var matchHistory = new MatchList();
					try
					{
						summoner = _api.Summoner.GetSummonerByNameAsync(researchRegion, currentName).Result;

						summonerId = summoner.AccountId;

						// Save his match history.
						matchHistory = _api.Match.GetMatchListAsync(researchRegion, summonerId, queues: new List<int>() { 450 }).Result;
					}
					catch (Exception)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine($"There is no summoner named '{currentName}' on {researchRegion.ToString()} server!\n");
						Console.ResetColor();

						// Continue (this will randomize nickname once again).
						currentName = "";
						continue;
					}

					// Check if he has played aram games.
					if (matchHistory.Matches.Count > 0)
					{
						// If yes, gather data from his games.
						foreach (var matchReference in matchHistory.Matches.Take(accountGamesLimit))
						{
							var match = new Match();
							try
							{
								match = _api.Match.GetMatchAsync(researchRegion, matchReference.GameId).Result;
							}
							catch
							{
								continue;
							}

							var blueParticipants = match.Participants.Where(x => x.TeamId == 100).ToList();
							var redParticipants = match.Participants.Where(x => x.TeamId == 200).ToList();

							try
							{
								// Save match data to .csv file.
								writer.WriteLine($"{Ddragon.GetChampionName(blueParticipants[0].ChampionId)},{Ddragon.GetChampionName(blueParticipants[1].ChampionId)},{Ddragon.GetChampionName(blueParticipants[2].ChampionId)},{Ddragon.GetChampionName(blueParticipants[3].ChampionId)},{Ddragon.GetChampionName(blueParticipants[4].ChampionId)},{Ddragon.GetChampionName(redParticipants[0].ChampionId)},{Ddragon.GetChampionName(redParticipants[1].ChampionId)},{Ddragon.GetChampionName(redParticipants[2].ChampionId)},{Ddragon.GetChampionName(redParticipants[3].ChampionId)},{Ddragon.GetChampionName(redParticipants[4].ChampionId)},{(blueParticipants[0].Stats.Winner ? "Blue" : "Red")}");
							}
							catch
							{
								continue;
							}

							// Display progress in console.
							Console.ForegroundColor = ConsoleColor.Blue;
							Console.WriteLine($"Game #{++gamesCounter} loaded.");
							Console.ResetColor();
							// Check if it reached game limit.
							if (gamesCounter >= gameLimit)
							{
								Console.WriteLine($"{gamesCounter} games loaded.");
								writer.Close();
								DeleteRepeatedRows();
								return;
							}

							// Get random name for next outer loop iteration;
							var randomName = match.ParticipantIdentities[_rng.Next(0, 10)].Player.SummonerName;
							currentName = randomName;
						}
					}
					else
					{
						// If not, continue (this will randomize nickname once again).
						currentName = "";
						continue;
					}
				}
			}

			// Remove duplicate games.
			DeleteRepeatedRows();
		}

		public static void DeleteRepeatedRows()
		{
			int gamesAmount = File.ReadLines(DataPath).Count() - 1;

			// Remove duplicate games from research data.
			string[] lines = File.ReadAllLines(DataPath);
			File.WriteAllLines(DataPath, lines.Distinct().Where(x => x.Count(y => y == ',') <= 10).ToArray());

			if(gamesAmount - File.ReadLines(DataPath).Count() + 1 != 1)
			{
				Console.WriteLine($"{gamesAmount - File.ReadLines(DataPath).Count() + 1} repeated games removed.");
			}
			else
			{
				Console.WriteLine($"{gamesAmount - File.ReadLines(DataPath).Count() + 1} repeated game removed.");
			}
		}

		// Checks winrates of champion groups
		public static void GetChampionGroupsWinrates()
		{
			// Load champion groups.
			LoadChampionGroups();

			Console.WriteLine($"Champion groups loaded.");

			using (var reader = new StreamReader(DataPath))
			{
				string line = "";

				// First, header line.
				reader.ReadLine();

				// For each game - check winrates.
				while ((line = reader.ReadLine()) != null)
				{
					// Split match data to lists of strings.
					var blueTeamChampions = new List<string>();
					var redTeamChampions = new List<string>();
					bool blueTeamWon = true;

					var splitLine = line.Split(',');

					blueTeamChampions.AddRange(splitLine.Take(5));
					redTeamChampions.AddRange(splitLine.Skip(5).Take(5));
					blueTeamWon = splitLine[10] == "Blue" ? true : false;

					// For each champion group - check winrates of every possible count (0-5).
					for (int i = 0; i < ChampionGroups.Count; i++)
					{
						// Save champion group members in current game.
						var blueChampionGroupMembers = blueTeamChampions.Intersect(ChampionGroups[i].Item1.ChampionNames);
						var redChampionGroupMembers = redTeamChampions.Intersect(ChampionGroups[i].Item1.ChampionNames);

						// Save game result.
						if (blueTeamWon)
						{
							ChampionGroups[i].Item2[blueChampionGroupMembers.Count()] = (ChampionGroups[i].Item2[blueChampionGroupMembers.Count()].Wins + 1, ChampionGroups[i].Item2[blueChampionGroupMembers.Count()].Losses);

							ChampionGroups[i].Item2[redChampionGroupMembers.Count()] = (ChampionGroups[i].Item2[redChampionGroupMembers.Count()].Wins, ChampionGroups[i].Item2[redChampionGroupMembers.Count()].Losses + 1);
						}
						else
						{
							ChampionGroups[i].Item2[blueChampionGroupMembers.Count()] = (ChampionGroups[i].Item2[blueChampionGroupMembers.Count()].Wins, ChampionGroups[i].Item2[blueChampionGroupMembers.Count()].Losses + 1);

							ChampionGroups[i].Item2[redChampionGroupMembers.Count()] = (ChampionGroups[i].Item2[redChampionGroupMembers.Count()].Wins + 1, ChampionGroups[i].Item2[redChampionGroupMembers.Count()].Losses);
						}
					}
				}
			}

			// Save data.
			using var writer = new StreamWriter(ReportPath);

			int gamesAmount = File.ReadLines(DataPath).Count() - 1;

			writer.WriteLine($"Total amount of analyzed games: {gamesAmount}");
			writer.WriteLine($"Sample size: {gamesAmount} * 2 = {gamesAmount * 2}");
			writer.WriteLine($"Because every analyzed game contains 2 teams");
			writer.WriteLine("-------------------");

			foreach (var championGroupPair in ChampionGroups)
			{
				writer.WriteLine($"{championGroupPair.Item1.GroupName} winrates:");

				for (int i = 0; i < 6; i++)
				{
					double winrate = Math.Round((double)championGroupPair.Item2[i].Wins / (championGroupPair.Item2[i].Wins + championGroupPair.Item2[i].Losses), 4);

					writer.WriteLine($"With {i} champions   -->   Win: {championGroupPair.Item2[i].Wins} \t|\t Loss: {championGroupPair.Item2[i].Losses} \t|\t Winrate: {(winrate > 0 ? winrate.ToString("P") : "-")}");
				}
				writer.WriteLine("-------------------");
			}

			Console.WriteLine($"Data report created.");
		}
		public static string GetRandomNickname()
		{
			using (var reader = new StreamReader(NicknamesPath))
			{
				List<string> nicknames = File.ReadAllLines(NicknamesPath).ToList();

				var randomNumber = _rng.Next(0, nicknames.Count() - 1);

				var randomNickname = nicknames[randomNumber];

				return randomNickname;
			}
		}
	}	
}