using System;
using System.Collections.Generic;

namespace NBASimulator.Models;

public partial class Pick
{
    public int Id { get; set; }

    public int Season { get; set; }

    public int TeamId { get; set; }

    public int RoundNum { get; set; }

    public int? PickNum { get; set; }

    public int? TradedToTeamId { get; set; }
}
