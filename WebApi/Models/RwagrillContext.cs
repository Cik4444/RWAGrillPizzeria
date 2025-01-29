using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models;

public partial class RwagrillContext : DbContext
{
    public RwagrillContext()
    {
    }

    public RwagrillContext(DbContextOptions<RwagrillContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alergen> Alergens { get; set; }

    public virtual DbSet<Hrana> Hranas { get; set; }

    public virtual DbSet<HranaAlergen> HranaAlergens { get; set; }

    public virtual DbSet<KategorijaHrane> KategorijaHranes { get; set; }

    public virtual DbSet<Korisnik> Korisniks { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Narudzba> Narudzbas { get; set; }

    public virtual DbSet<NarudzbaHrana> NarudzbaHranas { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=WIN-IL1B7CU893Q;Database=RWAGrill;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alergen>(entity =>
        {
            entity.HasKey(e => e.Idalergen).HasName("PK__Alergen__5F4CB147C94BD871");

            entity.ToTable("Alergen");

            entity.Property(e => e.Idalergen)
                .ValueGeneratedNever()
                .HasColumnName("IDAlergen");
            entity.Property(e => e.Naziv)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Hrana>(entity =>
        {
            entity.HasKey(e => e.Idhrana).HasName("PK__Hrana__0395A109C8FBE869");

            entity.ToTable("Hrana");

            entity.Property(e => e.Idhrana)
                .ValueGeneratedNever()
                .HasColumnName("IDHrana");
            entity.Property(e => e.Cijena).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.KategorijaHraneId).HasColumnName("KategorijaHraneID");
            entity.Property(e => e.Naslov)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.KategorijaHrane).WithMany(p => p.Hranas)
                .HasForeignKey(d => d.KategorijaHraneId)
                .HasConstraintName("FK__Hrana__Kategorij__4BAC3F29");
        });

        modelBuilder.Entity<HranaAlergen>(entity =>
        {
            entity.HasKey(e => e.IdhranaAlergen).HasName("PK__HranaAle__99A53278F716CB9C");

            entity.ToTable("HranaAlergen");

            entity.Property(e => e.IdhranaAlergen)
                .ValueGeneratedNever()
                .HasColumnName("IDHranaAlergen");
            entity.Property(e => e.AlergenId).HasColumnName("AlergenID");
            entity.Property(e => e.HranaId).HasColumnName("HranaID");

            entity.HasOne(d => d.Alergen).WithMany(p => p.HranaAlergens)
                .HasForeignKey(d => d.AlergenId)
                .HasConstraintName("FK__HranaAler__Alerg__5629CD9C");

            entity.HasOne(d => d.Hrana).WithMany(p => p.HranaAlergens)
                .HasForeignKey(d => d.HranaId)
                .HasConstraintName("FK__HranaAler__Hrana__5535A963");
        });

        modelBuilder.Entity<KategorijaHrane>(entity =>
        {
            entity.HasKey(e => e.IdkategorijaHrane).HasName("PK__Kategori__EC1C4505083E0FEA");

            entity.ToTable("KategorijaHrane");

            entity.Property(e => e.IdkategorijaHrane)
                .ValueGeneratedNever()
                .HasColumnName("IDKategorijaHrane");
            entity.Property(e => e.Naziv)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Korisnik>(entity =>
        {
            entity.HasKey(e => e.Idkorisnik).HasName("PK__Korisnik__6F9CD5C4A97BBBBE");

            entity.ToTable("Korisnik");

            entity.Property(e => e.Idkorisnik)
                .ValueGeneratedNever()
                .HasColumnName("IDKorisnik");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Ime)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Prezime)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Korisniks)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Korisnik__RoleId__01142BA1");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Logs__3214EC0795BA204C");

            entity.Property(e => e.Level).HasMaxLength(20);
            entity.Property(e => e.Message).HasMaxLength(500);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Narudzba>(entity =>
        {
            entity.HasKey(e => e.Idnarudzba).HasName("PK__Narudzba__458DF9D9852ED1DB");

            entity.ToTable("Narudzba");

            entity.Property(e => e.Idnarudzba)
                .ValueGeneratedNever()
                .HasColumnName("IDNarudzba");
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.KorisnikId).HasColumnName("KorisnikID");

            entity.HasOne(d => d.Korisnik).WithMany(p => p.Narudzbas)
                .HasForeignKey(d => d.KorisnikId)
                .HasConstraintName("FK__Narudzba__Korisn__52593CB8");
        });

        modelBuilder.Entity<NarudzbaHrana>(entity =>
        {
            entity.HasKey(e => e.IdnarudzbaHrana).HasName("PK__Narudzba__1DAB733478BCFDEE");

            entity.ToTable("NarudzbaHrana");

            entity.Property(e => e.IdnarudzbaHrana)
                .ValueGeneratedNever()
                .HasColumnName("IDNarudzbaHrana");
            entity.Property(e => e.HranaId).HasColumnName("HranaID");
            entity.Property(e => e.NarudzbaId).HasColumnName("NarudzbaID");

            entity.HasOne(d => d.Hrana).WithMany(p => p.NarudzbaHranas)
                .HasForeignKey(d => d.HranaId)
                .HasConstraintName("FK__NarudzbaH__Hrana__59FA5E80");

            entity.HasOne(d => d.Narudzba).WithMany(p => p.NarudzbaHranas)
                .HasForeignKey(d => d.NarudzbaId)
                .HasConstraintName("FK__NarudzbaH__Narud__59063A47");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1ADC821143");

            entity.ToTable("Role");

            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
