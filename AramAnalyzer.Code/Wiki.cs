using HtmlAgilityPack;
using System.Linq;

namespace AramAnalyzer.Code
{
	public static class Wiki
	{
		public static HtmlNode AramChangesTable { get; set; }

		static Wiki()
		{
			// Save table for future use.
			var url = @"https://leagueoflegends.fandom.com/wiki/ARAM";
			var web = new HtmlWeb();
			var doc = web.Load(url);

			// Select table.
			AramChangesTable = doc.DocumentNode
				.SelectNodes("/html[1]/body[1]/div[3]/div[2]/div[1]/div[1]/article[1]/div[1]/div[2]/div[1]/div[1]/div[3]/div[1]")
				?.FirstOrDefault();
			// To check xpath:
			// doc.DocumentNode.SelectNodes("//div[@id ='x']")
			// That's how to find xpath of outer div, then add /div[1]
		}

		public static string GetChampionBuffs(string championName)
		{
			// Check full name (nunu -> nunu & willump).
			championName = Ddragon.GetChampionFullName(championName);

			// When searching champion in table, replace:
			// & with &amp;
			// ' with &#39;
			championName = championName.Replace("&", "&amp;");
			championName = championName.Replace("\'", "&#39;");
			// in full champion name

			// Select <td> element with champion name.
			var tdWithChampionName = AramChangesTable.SelectNodes($"//td[@data-sort-value ='{championName}']")?.FirstOrDefault();

			// If champion wasn't found in table, it means it hasn't been nerfed/buffed.
			if (tdWithChampionName is null)
			{
				return "     ";
			}

			// Next siblings of this element are buffs/nerfs.
			string damageDealt = tdWithChampionName.NextSibling.NextSibling.InnerHtml;
			string damageReceived = tdWithChampionName.NextSibling.NextSibling.NextSibling.NextSibling.InnerHtml;

			// Return formatted value
			string buffs = $"\t\t{damageDealt}\t\t{damageReceived}\t";
			return buffs;
		}

		public static (string, string) GetChampionBuffsPair(string championName)
		{
			// Check full name (nunu -> nunu & willump).
			championName = Ddragon.GetChampionFullName(championName);

			// When searching champion in table, replace:
			// & with &amp;
			// ' with &#39;
			championName = championName.Replace("&", "&amp;");
			championName = championName.Replace("\'", "&#39;");
			// in full champion name

			// Select <td> element with champion name.
			var tdWithChampionName = AramChangesTable.SelectNodes($"//td[@data-sort-value ='{championName}']")?.FirstOrDefault();

			// If champion wasn't found in table, it means it hasn't been nerfed/buffed.
			if (tdWithChampionName is null)
			{
				return ("", "");
			}

			// Next siblings of this element are buffs/nerfs.
			string damageDealt = tdWithChampionName.NextSibling.NextSibling.InnerHtml;
			string damageReceived = tdWithChampionName.NextSibling.NextSibling.NextSibling.NextSibling.InnerHtml;

			return (damageDealt, damageReceived);
		}
	}
}