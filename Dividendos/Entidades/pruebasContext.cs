using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Dividendos.Entidades
{
    public partial class pruebasContext : DbContext
    {
        public pruebasContext()
        {
        }

        public pruebasContext(DbContextOptions<pruebasContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TAddressContract> TAddressContracts { get; set; }
        public virtual DbSet<TBalanceOfChange> TBalanceOfChanges { get; set; }
        public virtual DbSet<TBscpaf> TBscpafs { get; set; }
        public virtual DbSet<TTransferEvent> TTransferEvents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=localhost;Database=pruebas;Username=UsuSistema;Password=Welcome101!");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Spanish_Colombia.1252");

            modelBuilder.Entity<TAddressContract>(entity =>
            {
                entity.HasKey(e => e.Account)
                    .HasName("t_balanceof_pk");

                entity.ToTable("t_address_contract");

                entity.Property(e => e.Account)
                    .HasColumnType("character varying")
                    .HasColumnName("account");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<TBalanceOfChange>(entity =>
            {
                entity.ToTable("t_balance_of_change");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasColumnType("character varying")
                    .HasColumnName("address");

                entity.Property(e => e.Block).HasColumnName("block");

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .HasColumnName("date");

                entity.Property(e => e.ValueNew).HasColumnName("value_new");

                entity.Property(e => e.ValueOld).HasColumnName("value_old");

                entity.HasOne(d => d.AddressNavigation)
                    .WithMany(p => p.TBalanceOfChanges)
                    .HasForeignKey(d => d.Address)
                    .HasConstraintName("t_balance_of_change_fk");
            });

            modelBuilder.Entity<TBscpaf>(entity =>
            {
                entity.HasKey(e => e.Direccion)
                    .HasName("t_bscpaf_pk");

                entity.ToTable("t_bscpaf");

                entity.Property(e => e.Direccion)
                    .HasColumnType("character varying")
                    .HasColumnName("direccion");

                entity.Property(e => e.Balance)
                    .HasPrecision(20)
                    .HasColumnName("balance");

                entity.Property(e => e.Fecha)
                    .HasColumnType("date")
                    .HasColumnName("fecha");
            });

            modelBuilder.Entity<TTransferEvent>(entity =>
            {
                entity.HasKey(e => e.Hash)
                    .HasName("t_transfer_event_pk");

                entity.ToTable("t_transfer_event");

                entity.Property(e => e.Hash)
                    .HasColumnType("character varying")
                    .HasColumnName("hash");

                entity.Property(e => e.Bloque).HasColumnName("bloque");

                entity.Property(e => e.FechaLectura)
                    .HasColumnType("date")
                    .HasColumnName("fecha_lectura");

                entity.Property(e => e.From)
                    .IsRequired()
                    .HasColumnType("character varying")
                    .HasColumnName("from");

                entity.Property(e => e.To)
                    .IsRequired()
                    .HasColumnType("character varying")
                    .HasColumnName("to");

                entity.Property(e => e.Valor).HasColumnName("valor");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
