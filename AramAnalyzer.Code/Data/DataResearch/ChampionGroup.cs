using System.Collections.Generic;

namespace AramAnalyzer.Code.Data.DataResearch
{
	public class ChampionGroup
	{
		public string GroupName { get; set; }
		public List<string> ChampionNames { get; set; }
		public List<int> Points { get; set; }

		public ChampionGroup(string groupName)
		{
			ChampionNames = new List<string>();
			Points = new List<int>();
			GroupName = groupName;
		}
	}
}
