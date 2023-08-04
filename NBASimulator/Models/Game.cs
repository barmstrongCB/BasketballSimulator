using System;
using System.Collections.Generic;

namespace NBASimulator.Models;

public partial class Game
{
    public int Id { get; set; }

    public int TeamOneId { get; set; }

    public int TeamTwoId { get; set; }

    public int? TeamOnePts { get; set; }

    public int? TeamTwoPts { get; set; }

    public bool? TeamOneW { get; set; }

    public bool? TeamTwoW { get; set; }

    public bool? Tie { get; set; }
}
