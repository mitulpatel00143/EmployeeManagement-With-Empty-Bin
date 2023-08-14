using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasIndex(e => new { e.FirstName, e.LastName, e.Email })
                .IsUnique();


            modelBuilder.Entity<Employee>().HasData(
               new Employee
               {
                   Id = 1,
                   FirstName = "Mayur",
                   LastName = "Prajapati",
                   Email = "mayur@example.com",
                   Age = 20
               },
               new Employee
               {
                   Id = 2,
                   FirstName = "John",
                   LastName = "Mavileth",
                   Email = "john@example.com",
                   Age = 22
               }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
