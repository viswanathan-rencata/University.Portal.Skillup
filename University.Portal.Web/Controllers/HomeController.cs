using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using University.Portal.Data.Interface;
using University.Portal.Web.Models;

namespace University.Portal.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IUnitOfWork _unitOfWork;
		public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
		{
			_logger = logger;
			_unitOfWork = unitOfWork;
        }

		public IActionResult Index()
		{	
			var list = _unitOfWork.UserRepository.GetAll();
            return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error(int statusCode, string message)
		{
            return View(new ErrorViewModel 
			{ 
				RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
				StatusCode = statusCode,
				Message = message				
			});
		}
	}
}