using System.Security.Cryptography.X509Certificates;
using AutoMapper.Configuration;
// using Csdsa.Application.Common.Interfaces;
using Csdsa.Application.Common.Interfaces;
using Csdsa.Domain.Context;
using Csdsa.Infrastructure.Common.Repository;
using Csdsa.Infrastructure.Context;
using Csdsa.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Csdsa.Infrastructure;

namespace Csdsa.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        // Add this in your Program.cs
        builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddInfrastructure(builder.Configuration);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // await app.Services.InitializeDatabaseAsync();

        await app.RunAsync();
    }
}
