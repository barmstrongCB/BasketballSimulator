using System;
using System.Collections.Generic;

namespace NBASimulator.Models;

public partial class Team
{
    public int Id { get; set; }

    public string TeamName { get; set; } = null!;

    public int? YearOrigin { get; set; }

    public bool? NeedCalc { get; set; }
}
