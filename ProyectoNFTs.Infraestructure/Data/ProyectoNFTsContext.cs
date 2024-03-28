using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProyectoNFTs.Infraestructure.Models;

namespace ProyectoNFTs.Infraestructure.Data;

public partial class ProyectoNFTsContext : DbContext
{
    public ProyectoNFTsContext(DbContextOptions<ProyectoNFTsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Cliente { get; set; }

    public virtual DbSet<FacturaDetalle> FacturaDetalle { get; set; }

    public virtual DbSet<FacturaEncabezado> FacturaEncabezado { get; set; }

    public virtual DbSet<Nft> Nft { get; set; }

    public virtual DbSet<Pais> Pais { get; set; }

    public virtual DbSet<Rol> Rol { get; set; }

    public virtual DbSet<Tarjeta> Tarjeta { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente);

            entity.Property(e => e.IdCliente).ValueGeneratedNever();
            entity.Property(e => e.Apellido1)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Apellido2)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FechaNacimiento).HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Sexo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.IdPaisNavigation).WithMany(p => p.Cliente)
                .HasForeignKey(d => d.IdPais)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cliente_Pais");
        });

        modelBuilder.Entity<FacturaDetalle>(entity =>
        {
            entity.HasKey(e => new { e.IdFactura, e.Secuencia });

            entity.Property(e => e.IdNft).HasColumnName("IdNFT");
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdFacturaNavigation).WithMany(p => p.FacturaDetalle)
                .HasForeignKey(d => d.IdFactura)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FacturaDetalle_FacturaEncabezado");

            entity.HasOne(d => d.IdNftNavigation).WithMany(p => p.FacturaDetalle)
                .HasForeignKey(d => d.IdNft)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FacturaDetalle_NFT");
        });

        modelBuilder.Entity<FacturaEncabezado>(entity =>
        {
            entity.HasKey(e => e.IdFactura);

            entity.Property(e => e.IdFactura).ValueGeneratedNever();
            entity.Property(e => e.FechaFacturacion).HasColumnType("datetime");
            entity.Property(e => e.TarjetaNumero)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.FacturaEncabezado)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FacturaEncabezado_Cliente");

            entity.HasOne(d => d.IdTarjetaNavigation).WithMany(p => p.FacturaEncabezado)
                .HasForeignKey(d => d.IdTarjeta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FacturaEncabezado_Tarjeta");
        });

        modelBuilder.Entity<Nft>(entity =>
        {
            entity.HasKey(e => e.IdNft);

            entity.ToTable("NFT");

            entity.Property(e => e.IdNft).ValueGeneratedNever();
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasMany(d => d.IdCliente).WithMany(p => p.IdNft)
                .UsingEntity<Dictionary<string, object>>(
                    "Billetera",
                    r => r.HasOne<Cliente>().WithMany()
                        .HasForeignKey("IdCliente")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Billetera_Cliente"),
                    l => l.HasOne<Nft>().WithMany()
                        .HasForeignKey("IdNft")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Billetera_NFT"),
                    j =>
                    {
                        j.HasKey("IdNft", "IdCliente");
                    });
        });

        modelBuilder.Entity<Pais>(entity =>
        {
            entity.HasKey(e => e.IdPais);

            entity.Property(e => e.IdPais).ValueGeneratedNever();
            entity.Property(e => e.Alfa2)
                .HasMaxLength(2)
                .IsUnicode(false);
            entity.Property(e => e.Alfa3)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol);

            entity.Property(e => e.IdRol).ValueGeneratedNever();
            entity.Property(e => e.Descripcion)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Tarjeta>(entity =>
        {
            entity.HasKey(e => e.IdTarjeta);

            entity.Property(e => e.IdTarjeta).ValueGeneratedNever();
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Login);

            entity.Property(e => e.Login)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Apellidos)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK_Usuario_Rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
