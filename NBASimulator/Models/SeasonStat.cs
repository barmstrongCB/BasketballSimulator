using System;
using System.Collections.Generic;

namespace NBASimulator.Models;

public partial class SeasonStat
{
    public int Id { get; set; }

    public int PlayerId { get; set; }

    public int Season { get; set; }

    public bool RegularSeason { get; set; }

    public int Pts { get; set; }

    public int Reb { get; set; }

    public int Ast { get; set; }

    public int Stl { get; set; }

    public int Blk { get; set; }

    public int Tov { get; set; }

    public int Sm2 { get; set; }

    public int Sa2 { get; set; }

    public int Sm3 { get; set; }

    public int Sa3 { get; set; }

    public double? WinPct { get; set; }

    public double Ppg { get; set; }

    public double Rpg { get; set; }

    public double Apg { get; set; }

    public double Spg { get; set; }

    public double Bpg { get; set; }

    public double Tpg { get; set; }

    public double Sm2pg { get; set; }

    public double Sa2pg { get; set; }

    public double Sm3pg { get; set; }

    public double Sa3pg { get; set; }

    public int Games { get; set; }

    public double? SalaryReceived { get; set; }
}
