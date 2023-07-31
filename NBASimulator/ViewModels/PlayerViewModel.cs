namespace NBASimulator.ViewModels
{
    public class PlayerViewModel
    {
        public PlayerViewModel()
        {

        }
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public int? YearOrigin { get; set; }

        public double? Fg2pct { get; set; }

        public double? Fg3pct { get; set; }

        public double? Ppg { get; set; }

        public double? Apg { get; set; }

        public double? Rpg { get; set; }

        public double? Stl { get; set; }

        public double? Blk { get; set; }

        public double? Tov { get; set; }

        public double? Pa2 { get; set; }

        public double? Pa3 { get; set; }
        public double? WinPct { get; set; }
        public double? PlayLikely { get; set; }
        public double? RbdLikely{ get; set; }
        public double? BlkLikely{ get; set;}
        public double? StlLikely{ get; set;}
    }
}
