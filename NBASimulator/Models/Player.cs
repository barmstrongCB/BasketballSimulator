using System;
using System.Collections.Generic;

namespace NBASimulator.Models;

public partial class Player
{
    public int Id { get; set; }

    public int TeamId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public bool Sg { get; set; }

    public bool Sf { get; set; }

    public bool Pf { get; set; }

    public bool C { get; set; }

    public bool Pg { get; set; }

    public int? YearOrigin { get; set; }

    public double? Salary { get; set; }

    public int? FasalPaidBy { get; set; }

    public int? ContractLength { get; set; }

    public int FirstContractYear { get; set; }

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

    public double? RbdLikely { get; set; }

    public double? StlLikely { get; set; }

    public double? BlkLikely { get; set; }
}
