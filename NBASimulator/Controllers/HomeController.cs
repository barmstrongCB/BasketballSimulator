using Microsoft.AspNetCore.Mvc;
using NBASimulator.Models;
using System.Diagnostics;
using System.Linq.Expressions;

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
            //CreateSeason();
            //for (int i = 1; i < 3; i++)
            //{
            RunGame();
            //}
            //Records();
            //SumStats();
            //WinPct();
            //RealWinPct();
            //   CalibrateStats();
            // CalcAstBonus();
            return View();
        }

        public void CalibrateStats()
        {

            List<Player> players = _context.Players.Where(x => x.TeamId == 2 || x.TeamId == 4).ToList();
            foreach (Player p in players)
            {
                bool exist = _context.SeasonStats.Any(x => x.PlayerId == p.Id);
                if (exist)
                {
                    double fg2pct = (double)p.Fg2pct * 3;
                    double fg3pct = (double)p.Fg3pct * 3;
                    double ppg = (double)p.Ppg * 3;
                    double apg = (double)p.Apg * 3;
                    double rpg = (double)p.Rpg * 3;
                    double blk = (double)p.Blk * 3;
                    double tov = (double)p.Tov * 3;
                    double stl = (double)p.Stl * 3;
                    double pa2 = (double)p.Pa2 * 3;
                    double pa3 = (double)p.Pa3 * 3;
                    double win = (double)p.WinPct * 3;

                    SeasonStat seasonStat = _context.SeasonStats.SingleOrDefault(x => x.PlayerId == p.Id);
                    fg2pct += (seasonStat.Sm2pg / seasonStat.Sa2pg);
                    fg3pct += (seasonStat.Sm3pg / seasonStat.Sa3pg);
                    ppg += seasonStat.Ppg;
                    apg += seasonStat.Apg;
                    rpg += seasonStat.Rpg;
                    blk += seasonStat.Bpg;
                    tov += seasonStat.Tpg;
                    stl += seasonStat.Spg;
                    pa2 += seasonStat.Sa2pg;
                    pa3 += seasonStat.Sa3pg;
                    win += (double)seasonStat.WinPct;
                    try
                    {
                        if (fg2pct > 0)
                            p.Fg2pct = Math.Round(fg2pct / 4, 3);
                        if (fg3pct > 0)
                            p.Fg3pct = Math.Round(fg3pct / 4, 3);
                        if (ppg > 0)
                            p.Ppg = Math.Round(ppg / 4, 1);
                        if (rpg > 0)
                            p.Rpg = Math.Round(rpg / 4, 1);
                        if (apg > 0)
                            p.Apg = Math.Round(apg / 4, 1);
                        if (tov > 0)
                            p.Tov = Math.Round(tov / 4, 1);
                        if (blk > 0)
                            p.Blk = Math.Round(blk / 4, 1);
                        if (stl > 0)
                            p.Stl = Math.Round(stl / 4, 1);
                        if (pa2 > 0)
                            p.Pa2 = Math.Round(pa2 / 4, 1);
                        if (pa3 > 0)
                            p.Pa3 = Math.Round(pa3 / 4, 1);
                        if (win > 0)
                            p.WinPct = Math.Round(win / 4, 3);

                        _context.Update(p);
                        _context.SaveChanges();
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(p.Id + " " + ex.Message);
                    }
                }
            }

        }


        public void WinPct()
        {
            List<Player> players = _context.Players.Where(x => x.TeamId == 3 || x.TeamId == 6).ToList();
            foreach (Player player in players)
            {
                Team team = _context.Teams.SingleOrDefault(x => x.Id == player.TeamId);
                double winPct = Convert.ToDouble(team.Win) / Convert.ToDouble(team.Win + team.Loss);
                SeasonStat playerStat = _context.SeasonStats.SingleOrDefault(x => x.PlayerId == player.Id);
                playerStat.WinPct = Math.Round(winPct, 3);
                _context.Update(playerStat);
                _context.SaveChanges();
            }
        }
        public void RealWinPct()
        {
            List<Player> players = _context.Players.Where(x => x.TeamId == 2 || x.TeamId == 4).ToList();
            foreach (Player player in players)
            {
                double sumW = 0;
                double sumL = 0;

                List<Statline> statlines = _context.Statlines.Where(x => x.PlayerId == player.Id).ToList();
                foreach (Statline statline in statlines)
                {
                    bool existsOne = _context.Games.Any(x => x.Id == statline.GameId && x.TeamOneId == statline.TeamId);
                    if (existsOne)
                    {
                        Game game = _context.Games.SingleOrDefault(x => x.Id == statline.GameId && x.TeamOneId == statline.TeamId);
                        if ((bool)game.TeamOneW)
                            sumW++;
                        else
                            sumL++;
                    }
                    else
                    {
                        Game game2 = _context.Games.SingleOrDefault(x => x.Id == statline.GameId && x.TeamTwoId == statline.TeamId);
                        if ((bool)game2.TeamTwoW)
                            sumW++;
                        else
                            sumL++;
                    }
                }

                SeasonStat seasonStat = _context.SeasonStats.SingleOrDefault(x => x.PlayerId == player.Id);
                seasonStat.WinPct = Math.Round(sumW / (sumW + sumL), 3);
                _context.Update(seasonStat);
                _context.SaveChanges();
            }
        }

        public void SumStats()
        {
            List<Player> players = _context.Players.Where(x => x.TeamId == 2 || x.TeamId == 4).ToList();
            foreach (Player player in players)
            {
                SeasonStat newEntry = new SeasonStat();

                double sumPts = 0;
                double sumAst = 0;
                double sumReb = 0;
                double sumBlk = 0;
                double sumStl = 0;
                double sumTov = 0;
                double sumSm2 = 0;
                double sumSa2 = 0;
                double sumSm3 = 0;
                double sumSa3 = 0;
                double gameCount = 0;

                List<Statline> statlines = _context.Statlines.Where(x => x.PlayerId == player.Id).ToList();

                foreach (Statline statline in statlines)
                {
                    sumPts += statline.Pts;
                    sumAst += statline.Ast;
                    sumReb += statline.Reb;
                    sumBlk += statline.Blk;
                    sumStl += statline.Stl;
                    sumTov += statline.Tov;
                    sumSm2 += statline.Sm2;
                    sumSa2 += statline.Sa2;
                    sumSm3 += statline.Sm3;
                    sumSa3 += statline.Sa3;
                    gameCount++;
                }

                if (gameCount != 0)
                {
                    newEntry.PlayerId = player.Id;
                    newEntry.Season = 1;
                    newEntry.RegularSeason = true;
                    newEntry.Pts = Convert.ToInt32(sumPts);
                    newEntry.Ast = Convert.ToInt32(sumAst);
                    newEntry.Reb = Convert.ToInt32(sumReb);
                    newEntry.Blk = Convert.ToInt32(sumBlk);
                    newEntry.Stl = Convert.ToInt32(sumStl);
                    newEntry.Tov = Convert.ToInt32(sumTov);
                    newEntry.Sm2 = Convert.ToInt32(sumSm2);
                    newEntry.Sa2 = Convert.ToInt32(sumSa2);
                    newEntry.Sm3 = Convert.ToInt32(sumSm3);
                    newEntry.Sa3 = Convert.ToInt32(sumSa3);
                    newEntry.Ppg = Math.Round(sumPts / gameCount, 1);
                    newEntry.Apg = Math.Round(sumAst / gameCount, 1);
                    newEntry.Rpg = Math.Round(sumReb / gameCount, 1);
                    newEntry.Spg = Math.Round(sumStl / gameCount, 1);
                    newEntry.Bpg = Math.Round(sumBlk / gameCount, 1);
                    newEntry.Tpg = Math.Round(sumTov / gameCount, 1);
                    newEntry.Sm2pg = Math.Round(sumSm2 / gameCount, 1);
                    newEntry.Sa2pg = Math.Round(sumSa2 / gameCount, 1);
                    newEntry.Sm3pg = Math.Round(sumSm3 / gameCount, 1);
                    newEntry.Sa3pg = Math.Round(sumSa3 / gameCount, 1);
                    newEntry.Games = Convert.ToInt32(gameCount);

                    _context.Add(newEntry);
                    _context.SaveChanges();
                }
            }
        }

        public void CalcAstBonus()
        {
            List<Player> allPlayers = _context.Players.ToList();
            foreach (Player player in allPlayers)
            {
                // in this, 4.45 is the sum average of all active players APG. .3 is the scale we're bringing them closer to 1. 
                double sumAvg = 4.45;
                double scale = 0.3;
                player.AstBonus = (sumAvg + (scale * (player.Apg - sumAvg))) / sumAvg;
                _context.Update(player);
                _context.SaveChanges();
            }
        }

        public void Records()
        {
            List<Game> games = _context.Games.Where(x => x.TeamOnePts != null).ToList();
            foreach (Game game in games)
            {
                Team teamOne = _context.Teams.SingleOrDefault(x => x.Id == game.TeamOneId);
                Team teamTwo = _context.Teams.SingleOrDefault(x => x.Id == game.TeamTwoId);
                if ((bool)game.TeamOneW)
                {
                    teamOne.Win = teamOne.Win + 1;
                    teamTwo.Loss = teamTwo.Loss + 1;
                }
                else
                {
                    teamTwo.Win = teamTwo.Win + 1;
                    teamOne.Loss = teamOne.Loss + 1;
                }
                _context.SaveChanges();
            }
        }
        public void CreateSeason()
        {
            for (int i = 1; i < 10; i++)
            {
                for (int j = 1; j < 10; j++)
                {
                    if (i != j)
                    {
                        List<Game> countGame = _context.Games.Where(x => x.TeamOneId == i && x.TeamTwoId == j).ToList();
                        int kCount = 3 - countGame.Count();
                        for (int k = 0; k < kCount; k++)
                        {
                            Game game = new Game()
                            {
                                TeamOneId = i,
                                TeamTwoId = j
                            };
                            _context.Games.Add(game);
                            _context.SaveChanges();
                        }
                    }
                }
            }
        }

        public void RunGame()
        {
            // HARDCODED. Will be replaced w/ form input
            List<Game> countGame = _context.Games.Where(x => x.TeamOnePts == null).ToList();
            Random rndm = new();
            int rndmRes = rndm.Next(0, countGame.Count);
            Game curGame = countGame[rndmRes];

            int teamOneId = curGame.TeamOneId;
            int teamTwoId = curGame.TeamTwoId;

            GameLogic gameLogic = new GameLogic(_context);

            Team t1 = _context.Teams.SingleOrDefault(x => x.Id == teamOneId);

            if (t1.NeedCalc == true)
                gameLogic.CalcStats(t1.Id);

            Team t2 = _context.Teams.SingleOrDefault(x => x.Id == teamTwoId);

            if (t2.NeedCalc == true)
                gameLogic.CalcStats(t2.Id);

            List<Player> playerList = gameLogic.GetAllPlayers(t1.Id, t2.Id);

            gameLogic.GameLoop(playerList, curGame);
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