using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NBASimulator.Models;

public partial class NbasimulatorContext : DbContext
{
    public NbasimulatorContext()
    {
    }

    public NbasimulatorContext(DbContextOptions<NbasimulatorContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Pick> Picks { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Statline> Statlines { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=BARMSTRONGLAP\\SQLEXPRESS01; Database=NBASimulator; Trusted_Connection=true; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("Game");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<Pick>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblPicks");

            entity.ToTable("Pick");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.TradedToTeamId).HasColumnName("TradedToTeamID");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblPlayers");

            entity.ToTable("Player");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Apg).HasColumnName("APG");
            entity.Property(e => e.Blk).HasColumnName("BLK");
            entity.Property(e => e.FasalPaidBy).HasColumnName("FASalPaidBy");
            entity.Property(e => e.Fg2pct).HasColumnName("FG2Pct");
            entity.Property(e => e.Fg3pct).HasColumnName("FG3Pct");
            entity.Property(e => e.FirstContractYear).HasDefaultValueSql("((1))");
            entity.Property(e => e.FirstName).HasMaxLength(20);
            entity.Property(e => e.LastName).HasMaxLength(20);
            entity.Property(e => e.Pa2).HasColumnName("PA2");
            entity.Property(e => e.Pa3).HasColumnName("PA3");
            entity.Property(e => e.Pf).HasColumnName("PF");
            entity.Property(e => e.Pg).HasColumnName("PG");
            entity.Property(e => e.Ppg).HasColumnName("PPG");
            entity.Property(e => e.Rpg).HasColumnName("RPG");
            entity.Property(e => e.Sf).HasColumnName("SF");
            entity.Property(e => e.Sg).HasColumnName("SG");
            entity.Property(e => e.Stl).HasColumnName("STL");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.Tov).HasColumnName("TOV");
        });

        modelBuilder.Entity<Statline>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Stat");

            entity.ToTable("Statline");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblTeams");

            entity.ToTable("Team");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.NeedCalc)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.TeamName).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
