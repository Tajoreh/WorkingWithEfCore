﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace _02_Ef_Core_Performance.Entities;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? LastSalaryUpdateUtc { get; set; }
    public List<Employee> Employees { get; set; }
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }= string.Empty;
    public decimal Salary { get; set; }
    public int CompanyId { get; set; }
}

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions options):base(options){}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(builder =>
        {
            builder.ToTable("Companies");

            builder
                .HasMany(company => company.Employees)
                .WithOne()
                .HasForeignKey(employee => employee.CompanyId)
                .IsRequired();


            builder.HasData(new Company
            {
                Id = 2,
                Name = "Awesome Company"
            });
        });

        modelBuilder.Entity<Employee>(builder =>
        {
            builder.ToTable("Employees");

            var employees = Enumerable
                .Range(1, 1000)
                .Select(id => new Employee()
                {
                    Id = id,
                    Name = $"Employee #{id}",
                    Salary = 100.0m,
                    CompanyId = 2

                });
        });
    }
}