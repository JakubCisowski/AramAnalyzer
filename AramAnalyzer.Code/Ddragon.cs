using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace AramAnalyzer.Code
{
	public static class Ddragon
	{
		public static Dictionary<long, (string Id, string Name)> ChampionPairs { get; set; }
		public static string GameVersion { get; set; }
		public static string ChampionsJson { get; set; }

		static Ddragon()
		{
			using var client = new WebClient();
			
			// Load game version.
			string versionUrl = @"https://ddragon.leagueoflegends.com/api/versions.json";
			string jsonString = client.DownloadString(versionUrl);
			GameVersion = (JArray.Parse(jsonString))[0].ToString();

			// Load JSON with champion ids and names.
			string championsUrl = $@"http://ddragon.leagueoflegends.com/cdn/{GameVersion}/data/en_US/champion.json";
			ChampionsJson = client.DownloadString(championsUrl);

			ChampionPairs = new Dictionary<long, (string Id, string Name)>();

			// Load champion names and Ids from the file
			JObject championData = JObject.Parse(ChampionsJson);
			JObject rss = JObject.Parse(championData["data"].ToString());

			foreach (var item in rss.Children().Children())
			{
				var key = (long)item["key"];
				var id = ((string)item["id"], (string)item["name"]);

				ChampionPairs.Add(key, id);
			}
		}

		public static string GetChampionName(long key)
		{
			return ChampionPairs[key].Id;
		}

		public static string GetChampionFullName(string championName)
		{
			return ChampionPairs.Values.FirstOrDefault(x=>x.Id == championName).Name;
		}
	}
}