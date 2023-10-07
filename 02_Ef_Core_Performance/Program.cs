using _02_Ef_Core_Performance.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>(
    o => o.UseSqlServer(builder.Configuration.GetConnectionString("Database"))
);

var app = builder.Build();


app.UseHttpsRedirection();


//Method 1: it takes 1001 updates for each  employee and also company
app.MapPut("increase-salary", async (int companyId, DatabaseContext dbContext) =>
{
    var company = await dbContext
        .Set<Company>()
        .Include(x => x.Employees)
        .AsNoTracking()
        .FirstOrDefaultAsync(c => c.Id == companyId);

    if (company is null)
        return Results.NotFound($"The company with id '{companyId}' was not found.");

    foreach (var employee in company.Employees)
    {
        employee.Salary *= 1.1m;
    }

    company.LastSalaryUpdateUtc = DateTime.UtcNow;
    await dbContext.SaveChangesAsync();

    return Results.NoContent();
});


app.MapPut("increase-salary-sql", async (int companyId, DatabaseContext dbContext) =>
{
    var company = await dbContext
        .Set<Company>()
        .AsNoTracking()
        .FirstOrDefaultAsync(c => c.Id == companyId);

    if (company is null)
        return Results.NotFound($"The company with id '{companyId}' was not found.");

    await dbContext.Database.ExecuteSqlInterpolatedAsync(
         $"UPDATE Employees SET Salary= Salary * 1.1 WHERE companyId={companyId}"
     );

    company.LastSalaryUpdateUtc = DateTime.UtcNow;
    await dbContext.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();

