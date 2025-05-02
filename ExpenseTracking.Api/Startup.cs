using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Api.Impl.Service;
using ExpenseTracking.Api.Impl.Validation;
using ExpenseTracking.Api.Mapper;
using ExpenseTracking.Base;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ExpenseTracking.Api;

public class Startup
{
    public IConfiguration Configuration { get; }

    public static JwtConfig JwtConfig { get; private set; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        JwtConfig = Configuration.GetSection("JwtConfig").Get<JwtConfig>();

        services.AddSingleton<JwtConfig>(JwtConfig);
        services.AddControllers()
           .AddJsonOptions(opts =>
           {
               opts.JsonSerializerOptions.Converters
                   .Add(new JsonStringEnumConverter());
           })
           .AddFluentValidation(x =>
           {
               x.RegisterValidatorsFromAssemblyContaining<EmployeeValidator>();
           });

        services.AddSingleton(new MapperConfiguration(x => x.AddProfile(new MapperConfig())).CreateMapper());

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("MsSqlConnection")));

        services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(CreateEmployeeCommand).GetTypeInfo().Assembly));

        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IAppSession, AppSession>();

        services.AddScoped<IUserService, UserService>();

        services.AddAuthentication(x =>
   {
       x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
       x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
   }).AddJwtBearer(x =>
   {
       x.RequireHttpsMetadata = true;
       x.SaveToken = true;
       x.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = true,
           ValidIssuer = JwtConfig.Issuer,
           ValidateIssuerSigningKey = true,
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtConfig.Secret)),
           ValidAudience = JwtConfig.Audience,
           ValidateAudience = false,
           ValidateLifetime = true,
           ClockSkew = TimeSpan.FromMinutes(2)
       };
   });
        services.AddSwaggerGen(c =>
       {
           c.SwaggerDoc("v1", new OpenApiInfo { Title = "PAPARA Expense Tracking", Version = "v1.0" });
           var securityScheme = new OpenApiSecurityScheme
           {
               Name = "Authorization",
               Description = "Enter JWT Bearer token **_only_**",
               In = ParameterLocation.Header,
               Type = SecuritySchemeType.Http,
               Scheme = "bearer",
               BearerFormat = "JWT",
               Reference = new OpenApiReference
               {
                   Id = JwtBearerDefaults.AuthenticationScheme,
                   Type = ReferenceType.SecurityScheme
               }
           };
           c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
           c.AddSecurityRequirement(new OpenApiSecurityRequirement
           {
                     { securityScheme, new string[] { } }
           });
       });

        services.AddCors(options =>
       {
           options.AddPolicy("AllowAll",
               builder =>
         {
             builder.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader();
         });
       });

        services.AddScoped<IAppSession>(provider =>
        {
            var httpContextAccessor = provider.GetService<IHttpContextAccessor>();
            AppSession appSession = JwtManager.GetSession(httpContextAccessor.HttpContext);
            return appSession;
        });
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

        app.UseAuthentication();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}