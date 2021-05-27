using RiotSharp;
using RiotSharp.Endpoints.SpectatorEndpoint;
using RiotSharp.Endpoints.SummonerEndpoint;
using RiotSharp.Misc;
using System;
using System.Linq;

namespace AramAnalyzer.Code
{
	public static class Riot
	{
		private static RiotApi _api;
		private static CurrentGame currentGame;

		public static CurrentGame CurrentGame
		{
			get
			{
				return currentGame;
			}
			set
			{
				CurrentGame = value;
			}
		}

		public static bool IsPlayerBlueTeam { get; set; }

		static Riot()
		{
			// Get API key from resources.
			_api = RiotApi.GetDevelopmentInstance(Properties.Resources.Key);
		}

		public static CurrentGame GetCurrentGame(string summonerName, string regionString)
		{
			// Uppercase first letter of region input (to match Riot enum values).
			regionString = regionString.ToLower();
			regionString = regionString.First().ToString().ToUpper() + regionString.Substring(1);

			// Select region.
			Region region = new Region();
			try
			{
				region = (Region)Enum.Parse(typeof(Region), regionString);
			}
			catch (Exception)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"There is no such region as {regionString}!\n");
				Console.ResetColor();
				return null;
			}

			// Find summoner.
			Summoner summoner;
			string summonerId = "";
			try
			{
				summoner = _api.Summoner.GetSummonerByNameAsync(region, summonerName).Result;
				summonerId = summoner.Id;
			}
			catch (Exception)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"There is no summoner named '{summonerName}' on {regionString} server!\n");
				Console.ResetColor();
				return null;
			}

			// Find current game.
			CurrentGame game;
			try
			{
				game = _api.Spectator.GetCurrentGameAsync(region, summonerId).Result;
			}
			catch (Exception)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"'{summonerName}' [{regionString}] isn't currently in game!\n");
				Console.ResetColor();
				return null;
			}

			// Check if current game is an ARAM game.
			if (game.GameMode != GameMode.Aram)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"'{summonerName}' [{regionString}] is not in an ARAM game!\n");
				Console.ResetColor();
				return null;
			}

			// Check if the given player is on the Blue or Red team. (100 - blue)
			IsPlayerBlueTeam = game.Participants.FirstOrDefault(x => x.SummonerId == summonerId).TeamId == 100;

			// Everything is OK, save the game for future analysis.
			currentGame = game;
			return game;
		}

		public static CurrentGame GetCurrentGameWebsite(string summonerName, string regionString)
		{
			// When user refreshes the page while analysis is open (we can't catch form resubmission in that case).
			if (string.IsNullOrEmpty(summonerName) || string.IsNullOrEmpty(regionString))
			{
				throw new Exception($"Please search for a new analysis!\n");
			}

			// Uppercase first letter of region input (to match Riot enum values).
			regionString = regionString.ToLower();
			regionString = regionString.First().ToString().ToUpper() + regionString.Substring(1);

			// Select region.
			Region region = (Region)Enum.Parse(typeof(Region), regionString);

			// Find summoner.
			Summoner summoner;
			string summonerId = "";
			try
			{
				summoner = _api.Summoner.GetSummonerByNameAsync(region, summonerName).Result;
				summonerId = summoner.Id;
			}
			catch (Exception)
			{
				throw new Exception($"There is no summoner named '{summonerName}' on {regionString} server!\n");
			}

			// Find current game.
			CurrentGame game;
			try
			{
				game = _api.Spectator.GetCurrentGameAsync(region, summonerId).Result;
			}
			catch (Exception)
			{
				throw new Exception($"'{summonerName}' [{regionString}] isn't currently in game!\n");
			}

			// Check if current game is an ARAM game.
			if (game.GameMode != GameMode.Aram)
			{
				throw new Exception($"'{summonerName}' [{regionString}] is not in an ARAM game!\n");
			}

			// Check if the given player is on the Blue or Red team. (100 - blue)
			IsPlayerBlueTeam = game.Participants.FirstOrDefault(x => x.SummonerId == summonerId).TeamId == 100;

			// Everything is OK, save the game for future analysis.
			currentGame = game;
			return game;
		}
	}
}