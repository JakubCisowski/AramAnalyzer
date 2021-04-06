using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace AramAnalyzer.Code.Data.DataResearch
{
	public static class Research
	{
		public static List<ChampionGroup> ChampionGroups { get; set; }

		static Research()
		{
			ChampionGroups = new List<ChampionGroup>();
		}

		public static void LoadChampionGroups()
		{
			// Loads champion groups from ChampionGroupsResearch.json into list of champion groups (champion group = list of strings/champions)

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

		public static void LoadChampions(JObject data, string championGroupName)
		{
			// Load single champion group to static list.
			
			var championGroupArray = (JArray)data[championGroupName];
			var championGroup = new ChampionGroup(championGroupName);

			for (var i = 0; i < championGroupArray.Count; i++)
			{
				championGroup.ChampionNames.Add(championGroupArray[i].ToString());
			}

			ChampionGroups.Add(championGroup);
		}
	}
}