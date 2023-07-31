using Microsoft.AspNetCore.Mvc;
using NBASimulator.Models;
using System.Diagnostics;

namespace NBASimulator.Controllers
{
    public class HomeController : Controller
    {
//private readonly ILogger<HomeController> _logger;

        private readonly NbasimulatorContext _context;
        public HomeController(NbasimulatorContext context/*, ILogger<HomeController> logger*/)
        {
            _context = context;
            //_logger = logger;
        }
        public IActionResult Index()
        {
            RunGame();

            return View();
        }

        public void RunGame()
        {
            // HARDCODED. Will be replaced w/ form input
            int teamOneId = 2;
            int teamTwoId = 1;
            GameLogic gameLogic = new GameLogic(_context);

            Team t1 = _context.Teams.SingleOrDefault(x => x.Id == teamOneId);

            if (t1.NeedCalc == true)
                gameLogic.CalcStats(t1.Id);

            Team t2 = _context.Teams.SingleOrDefault(x => x.Id == teamTwoId);

            if (t2.NeedCalc == true)
                gameLogic.CalcStats(t2.Id);

            List<Player> playerList = gameLogic.GetAllPlayers(t1.Id,t2.Id);

            gameLogic.GameLoop(playerList);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}