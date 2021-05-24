using AramAnalyzer.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace AramAnalyzer.Website.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		public IActionResult Analysis(string name, string region, string platinumWinrates)
		{
			bool platWinrates = platinumWinrates == "on" ? true : false;
			AramAnalyzer.Code.MatchReport model;

			try
			{
				model = AramAnalyzer.Code.Analyzer.GetMatchReport(name, region, platWinrates);
			}
			catch (Exception e)
			{
				ViewData["ErrorMessage"] = e.Message;

				return View("SearchError");
			}

			return View(model);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}