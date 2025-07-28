using AutoMapper.Configuration;
using Csdsa.Domain.Context;
using Csdsa.Domain.Repository.Implementation;
using Csdsa.Domain.Repository.IRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace Csdsa.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        //    public void ConfigureServices(IServiceCollection services)
        //    {
        //        services.AddAutoMapper(typeof(ApiMappingProfile)),
        //                typeof(MappingProfile),
        //                typeof(InfrastructureMappingProfile));

        //        services.AddMediatR(typeof(CreateUserCommandHandler));

        //        services.AddScoped<IGenericRepository, GenericRepository>();
        //        services.AddScoped<IUnitOfWork, UnitOfWork>();

        //        services.AddDbContext<AppDbContext>(options =>
        //        options.UseNpgsql("connectionString"));


        //    }

        // builder.Services.AddApplicationServices();

        // builder.Services.AddInfrastructureServices(builder.Configuration);

        var app = builder.Build();

        //builder.Services.AddScoped<IArrayManipulationService, ArrayManipulationService>();
        //builder.Services.AddScoped<IArraySearchService, ArraySearchService>();
        //builder.Services.AddScoped<IArrayTransformationService, ArrayTransformationService>();

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
