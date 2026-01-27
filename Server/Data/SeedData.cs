using Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Data
{
    /// <summary>
    /// Classe para popular o banco de dados com dados iniciais
    /// </summary>
    public static class SeedData
    {
        public static void Initialize(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Garante que o banco de dados foi criado
                context.Database.EnsureCreated();

                // Verifica se já existem dados no banco
                if (context.People.Any() && context.Categories.Any())
                {
                    return; // Banco já foi populado
                }

                // Criar categorias
                var categories = new List<Category>
                {
                    new Category
                    {
                        Description = "Alimentação",
                        Purpose = PurposeType.Expense
                    },
                    new Category
                    {
                        Description = "Transporte",
                        Purpose = PurposeType.Expense
                    },
                    new Category
                    {
                        Description = "Saúde",
                        Purpose = PurposeType.Expense
                    },
                    new Category
                    {
                        Description = "Entretenimento",
                        Purpose = PurposeType.Expense
                    },
                    new Category
                    {
                        Description = "Salário",
                        Purpose = PurposeType.Income
                    },
                    new Category
                    {
                        Description = "Freelance",
                        Purpose = PurposeType.Income
                    },
                    new Category
                    {
                        Description = "Moradia",
                        Purpose = PurposeType.Expense
                    }
                };

                context.Categories.AddRange(categories);
                context.SaveChanges();

                // Criar pessoas
                var people = new List<Person>
                {
                    new Person
                    {
                        Name = "João Silva",
                        Age = 28
                    },
                    new Person
                    {
                        Name = "Maria Santos",
                        Age = 35
                    },
                    new Person
                    {
                        Name = "Pedro Oliveira",
                        Age = 42
                    },
                    new Person
                    {
                        Name = "Ana Costa",
                        Age = 31
                    }
                };

                context.People.AddRange(people);
                context.SaveChanges();

                // Criar transações
                var transactions = new List<Transaction>
                {
                    // Transações do João Silva
                    new Transaction
                    {
                        Description = "Supermercado - Compras",
                        Value = 150.50m,
                        Type = TransactionType.Expense,
                        CategoryId = categories.First(c => c.Description == "Alimentação").Id,
                        PersonId = people.First(p => p.Name == "João Silva").Id,
                        CreatedAt = DateTime.Now.AddDays(-5)
                    },
                    new Transaction
                    {
                        Description = "Uber para o trabalho",
                        Value = 35.00m,
                        Type = TransactionType.Expense,
                        CategoryId = categories.First(c => c.Description == "Transporte").Id,
                        PersonId = people.First(p => p.Name == "João Silva").Id,
                        CreatedAt = DateTime.Now.AddDays(-4)
                    },
                    new Transaction
                    {
                        Description = "Salário mensal",
                        Value = 4500.00m,
                        Type = TransactionType.Income,
                        CategoryId = categories.First(c => c.Description == "Salário").Id,
                        PersonId = people.First(p => p.Name == "João Silva").Id,
                        CreatedAt = DateTime.Now.AddDays(-1)
                    },
                    new Transaction
                    {
                        Description = "Consulta médica",
                        Value = 200.00m,
                        Type = TransactionType.Expense,
                        CategoryId = categories.First(c => c.Description == "Saúde").Id,
                        PersonId = people.First(p => p.Name == "João Silva").Id,
                        CreatedAt = DateTime.Now.AddDays(-3)
                    },
                    
                    // Transações da Maria Santos
                    new Transaction
                    {
                        Description = "Aluguel do apartamento",
                        Value = 1200.00m,
                        Type = TransactionType.Expense,
                        CategoryId = categories.First(c => c.Description == "Moradia").Id,
                        PersonId = people.First(p => p.Name == "Maria Santos").Id,
                        CreatedAt = DateTime.Now.AddDays(-10)
                    },
                    new Transaction
                    {
                        Description = "Cinema com amigos",
                        Value = 60.00m,
                        Type = TransactionType.Expense,
                        CategoryId = categories.First(c => c.Description == "Entretenimento").Id,
                        PersonId = people.First(p => p.Name == "Maria Santos").Id,
                        CreatedAt = DateTime.Now.AddDays(-2)
                    },
                    new Transaction
                    {
                        Description = "Projeto freelance concluído",
                        Value = 800.00m,
                        Type = TransactionType.Income,
                        CategoryId = categories.First(c => c.Description == "Freelance").Id,
                        PersonId = people.First(p => p.Name == "Maria Santos").Id,
                        CreatedAt = DateTime.Now
                    },
                    
                    // Transações do Pedro Oliveira
                    new Transaction
                    {
                        Description = "Restaurante",
                        Value = 120.00m,
                        Type = TransactionType.Expense,
                        CategoryId = categories.First(c => c.Description == "Alimentação").Id,
                        PersonId = people.First(p => p.Name == "Pedro Oliveira").Id,
                        CreatedAt = DateTime.Now.AddDays(-6)
                    },
                    new Transaction
                    {
                        Description = "Academia",
                        Value = 150.00m,
                        Type = TransactionType.Expense,
                        CategoryId = categories.First(c => c.Description == "Saúde").Id,
                        PersonId = people.First(p => p.Name == "Pedro Oliveira").Id,
                        CreatedAt = DateTime.Now.AddDays(-15)
                    },
                    
                    // Transações da Ana Costa
                    new Transaction
                    {
                        Description = "Café da manhã",
                        Value = 25.00m,
                        Type = TransactionType.Expense,
                        CategoryId = categories.First(c => c.Description == "Alimentação").Id,
                        PersonId = people.First(p => p.Name == "Ana Costa").Id,
                        CreatedAt = DateTime.Now.AddDays(-7)
                    },
                    new Transaction
                    {
                        Description = "Passagem de ônibus",
                        Value = 5.00m,
                        Type = TransactionType.Expense,
                        CategoryId = categories.First(c => c.Description == "Transporte").Id,
                        PersonId = people.First(p => p.Name == "Ana Costa").Id,
                        CreatedAt = DateTime.Now.AddDays(-1)
                    }
                };

                context.Transactions.AddRange(transactions);
                context.SaveChanges();
            }
        }
    }
}
