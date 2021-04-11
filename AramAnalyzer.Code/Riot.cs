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

			// Everything is OK, save the game for future analysis.
			currentGame = game;
			return game;
		}
	}
}