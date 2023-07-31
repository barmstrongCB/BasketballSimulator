using System;
using System.Collections.Generic;

namespace NBASimulator.Models;

public partial class Game
{
    public int Id { get; set; }

    public int TeamOneId { get; set; }

    public int TeamTwoId { get; set; }
}
