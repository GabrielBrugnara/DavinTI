using DavinTI.Models;
using Microsoft.EntityFrameworkCore;

namespace DavinTI.Data
{
    public class AgendaContext : DbContext
    {
        public AgendaContext(DbContextOptions<AgendaContext> options) : base(options) { }

        public DbSet<Contato> Contatos { get; set; }
        public DbSet<Telefone> Telefones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define a chave primária de Telefone como o ID único
            modelBuilder.Entity<Telefone>()
                .HasKey(t => t.Id);

            // Define o relacionamento 1:N entre Contato e Telefone
            modelBuilder.Entity<Telefone>()
                .HasOne(t => t.Contato)
                .WithMany(c => c.Telefones)
                .HasForeignKey(t => t.IdContato)
                .OnDelete(DeleteBehavior.Cascade); // Ao deletar o contato, apaga os telefones

            base.OnModelCreating(modelBuilder);
        }
    }
}