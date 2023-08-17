using NBASimulator.Models;
using NBASimulator.ViewModels;
using System.Linq;
using System.Numerics;

namespace NBASimulator
{
    public class GameLogic
    {
        private readonly NbasimulatorContext _context;
        public GameLogic(NbasimulatorContext context)
        {
            _context = context;
        }
        public class TeamBoosts
        {
            public int TeamId { get; set; }
            public string BoostName { get; set; }
            public double BoostVal { get; set; }
        }
        public void CalcStats(int teamId)
        {
            //Teams will have a NeedCalc bool field on %db. 
            //This is turned on by events like: re-ordering priorities on players, change of roster, season initialization, etc...
            //Once this runs, NeedCalc is updated to 0.

            List<Player> playersTeam = _context.Players
                .Where(x => x.TeamId == teamId)
                .ToList();

            double playSum = 0;
            double rbdSum = 0;
            double blkSum = 0;
            double stlSum = 0;

            foreach (Player player in playersTeam)
            {
                playSum += (double)player.Pa2 + (double)player.Pa3 + (double)player.Apg;
                rbdSum += (double)player.Rpg;
                blkSum += (double)player.Blk;
                stlSum += (double)player.Stl;
            }

            foreach (Player player in playersTeam)
            {
                Player curPlayer = _context.Players.SingleOrDefault(b => b.Id == player.Id);

                curPlayer.PlayLikely = ((double)player.Pa2 + (double)player.Pa3 + (double)player.Apg) / playSum;
                curPlayer.RbdLikely = (double)player.Rpg / rbdSum;
                curPlayer.BlkLikely = (double)player.Blk / blkSum;
                curPlayer.StlLikely = (double)player.Stl / stlSum;

                _context.Players.Update(curPlayer);
                _context.SaveChanges();
            }
        }
        public List<Player> GetPlayers(int teamId)
        {
            List<Player> allPlayers = _context.Players
                .Where(x => x.TeamId == teamId)
                .OrderBy(x => x.LastName)
                .ToList();

            return allPlayers;
        }
        public List<Player> GetAllPlayers(int teamOne, int teamTwo)
        {
            List<Player> allPlayers = _context.Players
                .Where(x => x.TeamId == teamOne || x.TeamId == teamTwo)
                .OrderBy(x => x.TeamId).ThenBy(x => x.LastName)
                .ToList();

            return allPlayers;
        }

        public List<TeamBoosts> CalcTeamBoosts(int teamOne, int teamTwo)
        {
            // this could and should all be calculated then stored in DB. Currently it does it at every start of game.
            //With these summed values in DB, all you'd have to do would be to contrast Win/PPG Boost, and RPG boost.

            List<TeamBoosts> bothTeams = new();

            double teamOneWinPct = 0;
            double teamOnePPG = 0;
            double teamOneRbdTot = 0;
            double teamOneBlkTot = 0;
            double teamOneStlTot = 0;
            double teamTwoWinPct = 0;
            double teamTwoPPG = 0;
            double teamTwoRbdTot = 0;
            double teamTwoBlkTot = 0;
            double teamTwoStlTot = 0;

            List<Player> teamOnePlayers = _context.Players
                .Where(x => x.TeamId == teamOne)
                .ToList();

            foreach (Player player in teamOnePlayers)
            {
                teamOneWinPct += (double)player.WinPct;
                teamOnePPG += (double)player.Ppg;
                teamOneRbdTot += (double)player.Rpg;
                teamOneBlkTot += (double)player.Blk;
                teamOneStlTot += (double)player.Stl;
            }

            List<Player> teamTwoPlayers = _context.Players
                .Where(x => x.TeamId == teamTwo)
                .ToList();

            foreach (Player player in teamTwoPlayers)
            {
                teamTwoWinPct += (double)player.WinPct;
                teamTwoPPG += (double)player.Ppg;
                teamTwoRbdTot += (double)player.Rpg;
                teamTwoBlkTot += (double)player.Blk;
                teamTwoStlTot += (double)player.Stl;
            }

            double sumWin = teamOneWinPct + teamTwoWinPct;
            double sumPPG = teamOnePPG + teamTwoPPG;
            var winBoost = new TeamBoosts()
            {
                TeamId = teamOne,
                BoostName = "Win",
                BoostVal = ((teamOneWinPct + teamOnePPG) / (sumWin + sumPPG)) * 2
            };
            bothTeams.Add(winBoost);

            var win2Boost = new TeamBoosts()
            {
                TeamId = teamTwo,
                BoostName = "Win",
                BoostVal = 2 - (teamOneWinPct / sumWin) * 2
            };
            bothTeams.Add(win2Boost);

            var rbdBoost = new TeamBoosts()
            {
                TeamId = teamOne,
                BoostName = "Rbd",
                BoostVal = (teamOneRbdTot - teamTwoRbdTot) / 2
            };
            bothTeams.Add(rbdBoost);

            var rbd2Boost = new TeamBoosts()
            {
                TeamId = teamTwo,
                BoostName = "Rbd",
                BoostVal = (teamTwoRbdTot - teamOneRbdTot) / 2
            };
            bothTeams.Add(rbd2Boost);

            var aBlk = new TeamBoosts()
            {
                TeamId = teamOne,
                BoostName = "Blk",
                BoostVal = teamOneBlkTot
            };
            bothTeams.Add(aBlk);

            var aStl = new TeamBoosts()
            {
                TeamId = teamOne,
                BoostName = "Stl",
                BoostVal = teamOneStlTot
            };
            bothTeams.Add(aStl);

            var bBlk = new TeamBoosts()
            {
                TeamId = teamTwo,
                BoostName = "Blk",
                BoostVal = teamTwoBlkTot
            };
            bothTeams.Add(bBlk);

            var bStl = new TeamBoosts()
            {
                TeamId = teamTwo,
                BoostName = "Stl",
                BoostVal = teamTwoStlTot
            };
            bothTeams.Add(bStl);

            return bothTeams;
        }

        public int WhoGets(int teamId, string statType, int holdBall = 0)
        {
            Random rndm = new();

            List<Player> allPlayers = _context.Players
                .Where(x => x.TeamId == teamId)
                .OrderBy(x => Guid.NewGuid())
                .ToList();

            double sumCnt = 0;

            if (holdBall == 0)
            {
                Console.WriteLine("Hi!" + statType + teamId.ToString());
            }

            double res = rndm.NextDouble();

            foreach (Player player in allPlayers)
            {
                switch (statType)
                {
                    case "Rbd":
                        if (res < player.RbdLikely + sumCnt)
                            return player.Id;
                        else
                            sumCnt += (double)player.RbdLikely;
                        break;
                    case "Stl":
                        if (res < player.StlLikely + sumCnt)
                            return player.Id;
                        else
                            sumCnt += (double)player.StlLikely;
                        break;
                    case "Play":
                        if ((res < player.PlayLikely + sumCnt) && player.Id != holdBall)
                            return player.Id;
                        else
                            sumCnt += (double)player.PlayLikely;
                        break;
                    case "Blk":
                        if (res < player.BlkLikely + sumCnt)
                            return player.Id;
                        else
                            sumCnt += (double)player.BlkLikely;
                        break;
                }
            }
            Player defPlayer = allPlayers.FirstOrDefault();
            return defPlayer.Id;
        }

        public string gameResults { get; set; }
        public void GameLoop(List<Player> allPlayers, Game curGame)
        {
            //setting initial conditions

            Random rndm = new();
            double rndmRes = 0;

            curGame.TeamOnePts = 0;
            curGame.TeamTwoPts = 0;
            curGame.TeamOneW = false;
            curGame.TeamTwoW = false;
            curGame.Tie = false;
            _context.Update(curGame);
            _context.SaveChanges();

            List<Statline> masterStatline = new();

            foreach (Player player in allPlayers)
            {
                var newStatline = new Statline()
                {
                    PlayerId = player.Id,
                    GameId = curGame.Id,
                    TeamId = player.TeamId,
                    Pts = 0,
                    Ast = 0,
                    Reb = 0,
                    Stl = 0,
                    Blk = 0,
                    Tov = 0,
                    Sm2 = 0,
                    Sa2 = 0,
                    Sm3 = 0,
                    Sa3 = 0
                };

                _context.Add(newStatline);
                _context.SaveChanges();
                masterStatline.Add(newStatline);
            }


            int playCount = 0;
            int teamOnePointTotal = 0;
            int teamTwoPointTotal = 0;
            bool teamOneBall = true;
            int teamOneId = masterStatline[0].TeamId;
            Team teamOne = _context.Teams.SingleOrDefault(x => x.Id == teamOneId);
            int teamTwoId = masterStatline[5].TeamId;
            Team teamTwo = _context.Teams.SingleOrDefault(x => x.Id == teamTwoId);
            int hasBallTeamId = masterStatline[0].TeamId;
            int noBallTeamId = masterStatline[5].TeamId;
            bool noAssist = true;
            int pendAssist = 0;
            int holdBall = 0;
            bool playDone = false;
            double sumStealOne = 0;
            double sumStealTwo = 0;
            double sumBlkOne = 0;
            double sumBlkTwo = 0;
            int totalPlays = 190;
            int whoPassed = 0;

            foreach (Player sumP in allPlayers.Where(x => x.TeamId == teamOneId))
            {
                sumStealOne += (int)sumP.Stl;
                sumBlkOne += (int)sumP.Blk;
            }

            foreach (Player sumP in allPlayers.Where(x => x.TeamId == teamTwoId))
            {
                sumStealTwo += (int)sumP.Stl;
                sumBlkTwo += (int)sumP.Blk;
            }

            // starting game loop



            while (playCount < totalPlays || teamOnePointTotal == teamTwoPointTotal)
            {
                playDone = false;

                if (holdBall == 0)
                {
                    rndmRes = rndm.NextDouble();

                    holdBall = WhoGets(hasBallTeamId, "Play");
                }

                Console.WriteLine(GetPlayerName(holdBall) + " has the ball " + System.Environment.NewLine);

                rndmRes = rndm.Next(1, 101);

                if (teamOneBall == true)
                {
                    if (rndmRes < sumStealTwo)
                    {
                        holdBall = WhoGets(noBallTeamId, "Stl");
                        AddStat(holdBall, "Stl", curGame.Id, 1);
                        Console.WriteLine(GetPlayerName(holdBall) + " steals!" + System.Environment.NewLine);
                        teamOneBall = false;
                        playDone = true;
                        playCount++;
                        (hasBallTeamId, noBallTeamId) = (noBallTeamId, hasBallTeamId);
                    }
                }
                else
                {
                    if (rndmRes < sumStealOne)
                    {
                        holdBall = WhoGets(noBallTeamId, "Stl");
                        AddStat(holdBall, "Stl", curGame.Id, 1);
                        Console.WriteLine(GetPlayerName(holdBall) + " steals!" + System.Environment.NewLine);
                        teamOneBall = true;
                        playDone = true;
                        playCount++;
                        (hasBallTeamId, noBallTeamId) = (noBallTeamId, hasBallTeamId);
                    }
                }

                if (!playDone)
                {
                    bool ifTov = tovCheck(holdBall);
                    if (ifTov)
                    {
                        AddStat(holdBall, "Tov", curGame.Id, 1);
                        playDone = true;
                        playCount++;
                        holdBall = 0;
                        (hasBallTeamId, noBallTeamId) = (noBallTeamId, hasBallTeamId);
                        if (teamOneBall)
                            teamOneBall = false;
                        else
                            teamOneBall = true;
                    }
                }

                if (!playDone)
                {
                    rndmRes = rndm.Next(1, 101);

                    if (teamOneBall == true)
                    {
                        if (rndmRes < sumBlkTwo)
                        {
                            holdBall = WhoGets(noBallTeamId, "Blk");
                            AddStat(holdBall, "Blk", curGame.Id, 1);
                            Console.WriteLine(GetPlayerName(holdBall) + " blocks!" + System.Environment.NewLine);
                            teamOneBall = false;
                            playDone = true;
                            playCount++;
                            (hasBallTeamId, noBallTeamId) = (noBallTeamId, hasBallTeamId);
                        }
                    }
                    else
                    {
                        if (rndmRes < sumBlkOne)
                        {
                            holdBall = WhoGets(noBallTeamId, "Blk");
                            AddStat(holdBall, "Blk", curGame.Id, 1);
                            Console.WriteLine(GetPlayerName(holdBall) + " blocks!" + System.Environment.NewLine);
                            teamOneBall = true;
                            playDone = true;
                            playCount++;
                            (hasBallTeamId, noBallTeamId) = (noBallTeamId, hasBallTeamId);
                        }
                    }
                }

                if (!playDone)
                {
                    bool passBall = WillPass(holdBall);

                    if (passBall)
                    {
                        whoPassed = holdBall;
                        string strPass = GetPlayerName(holdBall);
                        holdBall = WhoGets(hasBallTeamId, "Play", holdBall);
                        Console.WriteLine(strPass + " passes to " + GetPlayerName(holdBall));
                        playDone = true;
                    }
                    else
                    {
                        List<int> shotMade = WillShoot(holdBall, teamOneId, teamTwoId, hasBallTeamId, whoPassed);
                        if (shotMade[0] > 0)
                        {
                            if (whoPassed != 0)
                                AddStat(whoPassed, "Ast", curGame.Id, 1);
                            if (shotMade[0] == 2)
                            {
                                AddStat(holdBall, "Sa2", curGame.Id, 1);
                                AddStat(holdBall, "Sm2", curGame.Id, 1);
                                AddStat(holdBall, "Pts", curGame.Id, 2);
                                Console.WriteLine(GetPlayerName(holdBall) + " makes the 2!");

                            }
                            else
                            {
                                AddStat(holdBall, "Sa3", curGame.Id, 1);
                                AddStat(holdBall, "Sm3", curGame.Id, 1);
                                AddStat(holdBall, "Pts", curGame.Id, 3);
                                Console.WriteLine(GetPlayerName(holdBall) + " makes the 3!");
                            }
                            playDone = true;
                            holdBall = 0;
                            whoPassed = 0;
                            playCount++;
                            (hasBallTeamId, noBallTeamId) = (noBallTeamId, hasBallTeamId);

                            if (teamOneBall)
                            {
                                teamOnePointTotal += shotMade[0];
                                teamOneBall = false;
                            }
                            else
                            {
                                teamTwoPointTotal += shotMade[0];
                                teamOneBall = true;
                            }
                            Console.WriteLine(teamOne.TeamName + " " + teamOnePointTotal.ToString() + ", " +
                                   "" + teamTwo.TeamName + " " + teamTwoPointTotal.ToString());
                        }
                        else
                        {
                            string shotMiss = "";
                            if (shotMade[1] == 2)
                            {
                                AddStat(holdBall, "Sa2", curGame.Id, 1);
                                shotMiss = "two";
                            }

                            else
                            {
                                AddStat(holdBall, "Sa3", curGame.Id, 1);
                                shotMiss = "three";
                            }
                            string forLog = (GetPlayerName(holdBall) + " misses the " + shotMiss);
                            bool defRbd = DefRebound(hasBallTeamId, noBallTeamId, teamOneId, teamTwoId);
                            if (defRbd)
                            {
                                holdBall = WhoGets(noBallTeamId, "Rbd");
                                AddStat(holdBall, "Reb", curGame.Id, 1);
                                forLog += ".  " + GetPlayerName(holdBall) + " gets the rebound.";
                                Player rebPass = _context.Players.SingleOrDefault(x => x.Id == holdBall);
                                if (rebPass.PlayLikely < .2)
                                {
                                    holdBall = WhoGets(noBallTeamId, "Play");
                                    forLog += " Passes to " + GetPlayerName(holdBall);
                                }
                                playDone = true;
                                playCount++;
                                (hasBallTeamId, noBallTeamId) = (noBallTeamId, hasBallTeamId);
                                if (teamOneBall)
                                    teamOneBall = false;
                                else
                                    teamOneBall = true;
                            }
                            else
                            {
                                holdBall = WhoGets(hasBallTeamId, "Rbd");
                                AddStat(holdBall, "Reb", curGame.Id, 1);
                                forLog += " " + GetPlayerName(holdBall) + " gets the rebound.";
                                Player rebPass = _context.Players.SingleOrDefault(x => x.Id == holdBall);
                                if (rebPass.PlayLikely < .2)
                                {
                                    holdBall = WhoGets(hasBallTeamId, "Play");
                                    forLog += " Passes to " + GetPlayerName(holdBall);
                                }
                            }
                            Console.WriteLine(forLog);
                        }
                    }
                }
            }
            curGame.TeamOnePts = teamOnePointTotal;
            curGame.TeamTwoPts = teamTwoPointTotal;
            Team tOne = _context.Teams.SingleOrDefault(x => x.Id == curGame.TeamOneId);
            Team tTwo = _context.Teams.SingleOrDefault(x => x.Id == curGame.TeamTwoId);

            if (teamOnePointTotal != teamTwoPointTotal)
            {
                if (teamOnePointTotal > teamTwoPointTotal)
                {
                    curGame.TeamOneW = true;
                    tOne.Win++;
                    tTwo.Loss++;
                }
                else
                {
                    curGame.TeamTwoW = true;
                    tTwo.Win++;
                    tOne.Loss++;
                }
            }
            else
                curGame.Tie = true;

            _context.Update(curGame);
            _context.Update(tTwo);
            _context.Update(tOne);
            _context.SaveChanges();

        }

        public bool tovCheck(int holdBall)
        {
            Random rndm = new();
            int rndmRes = rndm.Next(1, 101);

            Player player = _context.Players.SingleOrDefault(x => x.Id == holdBall);

            if (rndmRes < player.Tov)
                return true;
            else
                return false;
        }
        public void AddStat(int holdBall, string statName, int gameId, int valAdd)
        {
            Statline statline = _context.Statlines.SingleOrDefault
                (x => x.PlayerId == holdBall && x.GameId == gameId);
            switch (statName)
            {
                case "Ast":
                    statline.Ast = statline.Ast + valAdd;
                    break;
                case "Sa3":
                    statline.Sa3 = statline.Sa3 + valAdd;
                    break;
                case "Sa2":
                    statline.Sa2 = statline.Sa2 + valAdd;
                    break;
                case "Pts":
                    statline.Pts = statline.Pts + valAdd;
                    break;
                case "Blk":
                    statline.Blk = statline.Blk + valAdd;
                    break;
                case "Reb":
                    statline.Reb = statline.Reb + valAdd;
                    break;
                case "Sm2":
                    statline.Sm2 = statline.Sm2 + valAdd;
                    break;
                case "Sm3":
                    statline.Sm3 = statline.Sm3 + valAdd;
                    break;
                case "Stl":
                    statline.Stl = statline.Stl + valAdd;
                    break;
                case "Tov":
                    statline.Tov = statline.Tov + valAdd;
                    break;
            }
            _context.SaveChanges();
        }
        public bool DefRebound(int hasBallTeamId, int noBallTeamId, int teamOneId, int teamTwoId)
        {
            Random rndm = new();
            int rndmRes = rndm.Next(1, 101);

            List<TeamBoosts> bothTeams = CalcTeamBoosts(teamOneId, teamTwoId);

            var narrowTo = bothTeams.Where(x => x.TeamId == noBallTeamId && x.BoostName == "Rbd");

            TeamBoosts defValBoost = narrowTo.First();

            int defVal = Convert.ToInt32(defValBoost.BoostVal);

            if (rndmRes < 77 + defVal)
                return true;
            else
                return false;
        }

        public List<int> WillShoot(int holdBall, int teamOneId, int teamTwoId, int hasBallTeamId, int whoPassed)
        {
            Player playerTest = _context.Players.SingleOrDefault(b => b.Id == holdBall);
            double astBoost = 1;
            
            if(whoPassed != 0)
            {
                Player playerPassed = _context.Players.SingleOrDefault(b => b.Id == whoPassed);
                astBoost = (double)playerPassed.AstBonus;
            }

            List<TeamBoosts> bothTeams = CalcTeamBoosts(teamOneId, teamTwoId);

            var narrowTo = bothTeams.Where(x => x.TeamId == hasBallTeamId && x.BoostName == "Win");

            TeamBoosts shotBoost = narrowTo.First();

            double sumShootTest = (double)playerTest.Pa2 + (double)playerTest.Pa3;
            Random rndm = new();
            double rndmRes = rndm.Next(0, Convert.ToInt32(sumShootTest) + 1);

            if (rndmRes < (double)playerTest.Pa2)
            {
                Console.WriteLine(GetPlayerName(holdBall) + " shoots a 2...");
                rndmRes = rndm.NextDouble();
                double fg2Boost = (double)playerTest.Fg2pct * shotBoost.BoostVal * astBoost;
                if (rndmRes < fg2Boost)
                {
                    List<int> list = new List<int>() { 2, 2 };
                    return list;
                }
                else
                {
                    List<int> list = new List<int>() { 0, 2 };
                    return list;
                }
            }
            else
            {
                Console.WriteLine(GetPlayerName(holdBall) + " shoots a 3...");
                rndmRes = rndm.NextDouble();
                double fg3Boost = (double)playerTest.Fg3pct * shotBoost.BoostVal * astBoost;
                if (rndmRes < fg3Boost)
                {
                    List<int> list = new List<int>() { 3, 3 };
                    return list;
                }
                else
                {
                    List<int> list = new List<int>() { 0, 3 };
                    return list;
                }
            }
        }
        public bool WillPass(int holdBall)
        {
            Player playerTest = _context.Players.SingleOrDefault(b => b.Id == holdBall);

            double sumPassTest = (double)playerTest.Pa2 + (double)playerTest.Pa3 + ((double)playerTest.Apg * 5.0);
            Random rndm = new();
            double rndmRes = rndm.Next(0, Convert.ToInt32(sumPassTest) + 1);

            if (rndmRes < ((double)playerTest.Pa2 + (double)playerTest.Pa3))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public string GetPlayerName(int holdBall)
        {
            Player playerBall = _context.Players.SingleOrDefault(b => b.Id == holdBall);
            string fullName = playerBall.FirstName + " " + playerBall.LastName;
            return fullName;
        }
    }
}
