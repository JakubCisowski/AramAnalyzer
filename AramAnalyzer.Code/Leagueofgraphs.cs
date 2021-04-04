using HtmlAgilityPack;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace AramAnalyzer.Code
{
	public static class Leagueofgraphs
	{
		private static string _URL;

		public static string URL
		{
			get
			{
				return _URL;
			}

			set
			{
				_URL = value;
			}
		}

		public static double GetWinrate(string championName)
		{
			// Adjust champion name (only letters).
			string championNameFixed = Regex.Replace(championName, "[^a-zA-Z]", "").ToLower();

			// Set stats page URL.
			URL = $@"https://www.leagueofgraphs.com/champions/builds/{championNameFixed}/iron/aram";

			// Get this champion winrate
			var web = new HtmlWeb();

			// HTML document
			var doc = web.Load(URL);
			
			// Select node with winrate.
			var node = doc.DocumentNode
				.SelectNodes("/html/body/div[2]/div[3]/div[3]/div[1]/div[2]/div/div[1]/a[1]/div/div/div/div[1]/div[2]/div/div[1]")
				?.FirstOrDefault();

			// If node wasn't found, check another Xpath.
			if(node == null)
			{
				node = doc.DocumentNode
				.SelectNodes("/html/body/div[2]/div[3]/div[3]/div[2]/div[2]/div/div[1]/a[1]/div/div/div/div[1]/div[2]/div/div[1]")
				?.FirstOrDefault();
			}
			
			string winrateString = node.InnerText.Substring(10, 4);

			double winrate = double.Parse(winrateString, CultureInfo.InvariantCulture);

			return winrate;
		}
	}
}