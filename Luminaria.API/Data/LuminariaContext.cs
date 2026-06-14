using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Luminaria.API.Models;

namespace Luminaria.API.Data;

public partial class LuminariaContext : DbContext
{
    public LuminariaContext()
    {
    }

    public LuminariaContext(DbContextOptions<LuminariaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<FrasesCelebre> FrasesCelebres { get; set; }

    public virtual DbSet<HitosHistorico> HitosHistoricos { get; set; }

    public virtual DbSet<Libro> Libros { get; set; }

    public virtual DbSet<Personaje> Personajes { get; set; }
    public virtual DbSet<Roles> Roles { get; set; } 
    public virtual DbSet<Usuarios> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.CategoriaId).HasName("PK__Categori__F353C1C54E62D5D6");

            entity.HasIndex(e => e.NombreCategoria, "IDX_Categorias_NombreCategoria");

            entity.Property(e => e.CategoriaId).HasColumnName("CategoriaID");
            entity.Property(e => e.ColorEstilo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NombreCategoria)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FrasesCelebre>(entity =>
        {
            entity.HasKey(e => e.FraseId).HasName("PK__FrasesCe__A813C0633AE2C79A");

            entity.HasIndex(e => e.PersonajeId, "IDX_FrasesCelebres_PersonajeID");

            entity.Property(e => e.FraseId).HasColumnName("FraseID");
            entity.Property(e => e.PersonajeId).HasColumnName("PersonajeID");

            entity.HasOne(d => d.Personaje).WithMany(p => p.FrasesCelebres)
                .HasForeignKey(d => d.PersonajeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__FrasesCel__Perso__4222D4EF");
        });

        modelBuilder.Entity<HitosHistorico>(entity =>
        {
            entity.HasKey(e => e.HitoId).HasName("PK__HitosHis__AA40964C67C96B97");

            entity.HasIndex(e => e.Anno, "IDX_HitosHistoricos_Anno");

            entity.Property(e => e.HitoId).HasColumnName("HitoID");
            entity.Property(e => e.PersonjaeId).HasColumnName("PersonjaeID");
            entity.Property(e => e.TituloHito)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.HasOne(d => d.Personjae).WithMany(p => p.HitosHistoricos)
                .HasForeignKey(d => d.PersonjaeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__HitosHist__Perso__3F466844");
        });

        modelBuilder.Entity<Libro>(entity =>
        {
            entity.HasKey(e => e.LibroId).HasName("PK__Libros__35A1EC8DE32389DF");

            entity.HasIndex(e => e.Titulo, "IDX_Libros_Titulo");

            entity.Property(e => e.LibroId).HasColumnName("LibroID");
            entity.Property(e => e.ArchivoOrLinkUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EsDominioPublico).HasDefaultValue(false);
            entity.Property(e => e.ImagenPortadaUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Titulo)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Personaje>(entity =>
        {
            entity.HasKey(e => e.PersonajeId).HasName("PK__Personaj__FF7028BD8DF74C0A");

            entity.HasIndex(e => e.Nombre, "IDX_Personajes_Nombre");

            entity.Property(e => e.PersonajeId).HasColumnName("PersonajeID");
            entity.Property(e => e.Epoca)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ImgUrl)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("ImgURL");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ResumenBreve)
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.HasMany(d => d.Categoria).WithMany(p => p.Personajes)
                .UsingEntity<Dictionary<string, object>>(
                    "PersonajeCategorium",
                    r => r.HasOne<Categoria>().WithMany()
                        .HasForeignKey("CategoriaId")
                        .HasConstraintName("FK__Personaje__Categ__3C69FB99"),
                    l => l.HasOne<Personaje>().WithMany()
                        .HasForeignKey("PersonajeId")
                        .HasConstraintName("FK__Personaje__Perso__3B75D760"),
                    j =>
                    {
                        j.HasKey("PersonajeId", "CategoriaId").HasName("PK__Personaj__B04514A1101A5ECB");
                        j.ToTable("PersonajeCategoria");
                        j.IndexerProperty<int>("PersonajeId").HasColumnName("PersonajeID");
                        j.IndexerProperty<int>("CategoriaId").HasColumnName("CategoriaID");
                    });

            entity.HasMany(d => d.Libros).WithMany(p => p.Personajes)
                .UsingEntity<Dictionary<string, object>>(
                    "PersonajeLibro",
                    r => r.HasOne<Libro>().WithMany()
                        .HasForeignKey("LibroId")
                        .HasConstraintName("FK__Personaje__Libro__48CFD27E"),
                    l => l.HasOne<Personaje>().WithMany()
                        .HasForeignKey("PersonajeId")
                        .HasConstraintName("FK__Personaje__Perso__47DBAE45"),
                    j =>
                    {
                        j.HasKey("PersonajeId", "LibroId").HasName("PK__Personaj__3C2A367544054FD0");
                        j.ToTable("PersonajeLibro");
                        j.IndexerProperty<int>("PersonajeId").HasColumnName("PersonajeID");
                        j.IndexerProperty<int>("LibroId").HasColumnName("LibroID");
                    });
        });

        modelBuilder.Entity<Roles>(entity =>
        {   
            entity.ToTable("Roles", schema: "Auth");
            entity.HasKey(e => e.RolID).HasName("PK__Roles__3214EC07B9F1C8B7");

            entity.HasIndex(e => e.NombreRol, "IDX_Roles_NombreRol").IsUnique();

            entity.Property(e => e.RolID).HasColumnName("RolID");
            entity.Property(e => e.NombreRol)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.ToTable("Usuarios", schema: "Auth");
            entity.HasKey(e => e.UsuarioID).HasName("PK__Usuarios__1788CC4CBBB2A5F4");
            entity.HasIndex(e => e.NombreUsuario, "IDX_Usuarios_NombreUsuario").IsUnique();
            entity.Property(e => e.UsuarioID).HasColumnName("UsuarioID");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ContraseñaHash)
                .HasMaxLength(64)
                .IsUnicode(false);

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.RolID)
                .HasConstraintName("FK__Usuarios__RolID__3E52440B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
