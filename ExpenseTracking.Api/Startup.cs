using System.Reflection;
using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Api.Impl.Validation;
using ExpenseTracking.Api.Mapper;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddControllers().AddFluentValidation(x =>
     {
         x.RegisterValidatorsFromAssemblyContaining<EmployeeValidator>();
     });
        services.AddSingleton(new MapperConfiguration(x => x.AddProfile(new MapperConfig())).CreateMapper());

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("MsSqlConnection")));

        services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(CreateEmployeeCommand).GetTypeInfo().Assembly));

        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}