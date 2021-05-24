using System.Collections.Generic;
using System.Text;

namespace AramAnalyzer.Code.Data.DataResearch
{
	public class ChampionGroup
	{
		public string GroupName { get; set; }
		public string DisplayName { get; set; }
		public List<string> ChampionNames { get; set; }
		public List<int> Points { get; set; }

		public ChampionGroup(string groupName)
		{
			ChampionNames = new List<string>();
			Points = new List<int>();
			GroupName = groupName;

			if (GroupName.ToUpper() == GroupName)
			{
				// If the name is only capital characters (like ADC), do nothing.
				DisplayName = GroupName;
			}
			else
			{
				// Rework the name, eg. BattleCaster -> Battle Caster
				var builder = new StringBuilder();
				char previousChar = char.MinValue;
			
				foreach (char c in GroupName)
				{
					if (char.IsUpper(c))
					{
						if (builder.Length != 0 && previousChar != ' ')
						{
							builder.Append(' ');
						}   
					}
			
					builder.Append(c);
					previousChar = c;
				}
			
				DisplayName = builder.ToString();
			}
		}
	}
}
