using Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Data
{
    /// <summary>
    /// Contexto do banco de dados da aplicação
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person> People { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração do cascade delete para Pessoa -> Transações
            modelBuilder.Entity<Person>()
                .HasMany(p => p.Transactions)
                .WithOne(t => t.Person)
                .HasForeignKey(t => t.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuração do relacionamento Categoria -> Transações
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Transactions)
                .WithOne(t => t.Category)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Não deleta categoria se houver transações

            // Configurações adicionais para garantir integridade dos dados
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Value)
                .HasPrecision(18, 2);
        }
    }
}