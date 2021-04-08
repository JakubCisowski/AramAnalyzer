using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using RiotSharp;
using RiotSharp.Endpoints.SpectatorEndpoint;
using RiotSharp.Endpoints.SummonerEndpoint;
using RiotSharp.Misc;
using System.Linq;
using System.Threading;

namespace AramAnalyzer.Code.Data.DataResearch
{
	public static class Research
	{
		public static List<ChampionGroup> ChampionGroups { get; set; }
		private static RiotApi _api;
		private static Random _rng;

		// Request limits.
		const int LimitPerSecond = 20;
		const int LimitPerTwoMinutes = 100;

		static Research()
		{
			ChampionGroups = new List<ChampionGroup>();

			_rng = new Random();

			// Get API key from resources.
			_api = RiotApi.GetDevelopmentInstance(Properties.Resources.Key);
		}

		// Loads champion groups from ChampionGroupsResearch.json into list of champion groups (champion group = list of strings/champions)
		public static void LoadChampionGroups()
		{

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
			ChampionGroups.Clear();

			var championGroupArray = (JArray)data[championGroupName];
			var championGroup = new ChampionGroup(championGroupName);

			for (var i = 0; i < championGroupArray.Count; i++)
			{
				championGroup.ChampionNames.Add(championGroupArray[i].ToString());
			}

			ChampionGroups.Add(championGroup);
		}

		public static void EnforceRateLimits(int requestCounter)
		{
			if (requestCounter % LimitPerTwoMinutes == 0)
			{
				// Wait 2 minutes.
				Console.WriteLine("Waiting 2 minutes");
				Thread.Sleep(120000);
			}
			else if (requestCounter % LimitPerSecond == 0)
			{
				// Wait 1 second.
				Console.WriteLine("Waiting 1 second");
				Thread.Sleep(1000);
			}
		}

		public static void GatherData(string name)
		{
			const Region researchRegion = Region.Eune;
			
			int requestCounter = 0;

 			// How many account do we gather data from.
			const int accountsAmount = 2;

			// How many games do we search per account.
			const int accountGamesLimit = 3;

			// Default nickname in case of exception.
			const string defaultErrorNickname = "EpicNoob666";

			// Create file if it doesn't already exist.
			// // if (!File.Exists(@"Data\DataReseatch\Games.csv"))
			// // {
			// // 	using TextWriter initialWriter = new StreamWriter(@"Data\DataReseatch\Games.csv", true);

			// // 	// Value names.
			// // 	initialWriter.WriteLine("blueChampion1,blueChampion2,blueChampion3,blueChampion4,blueChampion5,redChampion1,redChampion2,redChampion3,redChampion4,redChampion5,winner");
				
			// // }

			// Gather data.

			using StreamWriter writer = new StreamWriter(@"C:\Programowanie\Fun\AramAnalyzer\AramAnalyzer.ConsoleApp\bin\Debug\net5.0\Data\DataResearch\Games.csv", true);

			for (int i = 0; i < accountsAmount; i++)
			{
				// Find summoner.
				Summoner summoner;
				string summonerId = "";
				try
				{
					summoner = _api.Summoner.GetSummonerByNameAsync(researchRegion, name).Result;
					EnforceRateLimits(++requestCounter);

					summonerId = summoner.AccountId;
				}
				catch (Exception)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"There is no summoner named '{name}' on {researchRegion.ToString()} server!\n");
					Console.ResetColor();

					// Continue with default nickname.
					name = defaultErrorNickname;
					continue;
				}

				// Save his match history.
				var matchHistory = _api.Match.GetMatchListAsync(researchRegion, summonerId, queues: new List<int>(){450}).Result;
				EnforceRateLimits(++requestCounter);

				// Check if he has played aram games.
				if(matchHistory.Matches.Count > 0)
				{
					// If yes, gather data from his games.
					foreach (var matchReference in matchHistory.Matches.Take(accountGamesLimit))
					{
						var match = _api.Match.GetMatchAsync(researchRegion, matchReference.GameId).Result;
						EnforceRateLimits(++requestCounter);

						var blueParticipants = match.Participants.Where(x => x.TeamId == 100).ToList();
						var redParticipants = match.Participants.Where(x => x.TeamId == 200).ToList();

						// Save match data to .csv file.
						writer.WriteLine($"{Ddragon.GetChampionName(blueParticipants[0].ChampionId)},{Ddragon.GetChampionName(blueParticipants[1].ChampionId)},{Ddragon.GetChampionName(blueParticipants[2].ChampionId)},{Ddragon.GetChampionName(blueParticipants[3].ChampionId)},{Ddragon.GetChampionName(blueParticipants[4].ChampionId)},{Ddragon.GetChampionName(redParticipants[0].ChampionId)},{Ddragon.GetChampionName(redParticipants[1].ChampionId)},{Ddragon.GetChampionName(redParticipants[2].ChampionId)},{Ddragon.GetChampionName(redParticipants[3].ChampionId)},{Ddragon.GetChampionName(redParticipants[4].ChampionId)},{(blueParticipants[0].Stats.Winner?"Blue":"Red")}");

						// Get random name for next outer loop iteration;
						var randomName = match.ParticipantIdentities[_rng.Next(0, 10)].Player.SummonerName;
						name = randomName;
					}
				}
				else
				{
					// If not, continue with default nickname.
					name =  defaultErrorNickname;
					continue;
				}
			}
		}

		public static void DeleteRepeatedRows()
		{

		}
	}
}