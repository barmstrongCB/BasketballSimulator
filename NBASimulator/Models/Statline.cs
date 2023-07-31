using System;
using System.Collections.Generic;

namespace NBASimulator.Models;

public partial class Statline
{
    public int Id { get; set; }

    public int GameId { get; set; }

    public int PlayerId { get; set; }

    public int TeamId { get; set; }

    public int Pts { get; set; }

    public int Ast { get; set; }

    public int Reb { get; set; }

    public int Stl { get; set; }

    public int Blk { get; set; }

    public int Tov { get; set; }

    public int Sm2 { get; set; }

    public int Sa2 { get; set; }

    public int Sm3 { get; set; }

    public int Sa3 { get; set; }
}
