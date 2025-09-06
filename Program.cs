using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.Repositories;
using Clinic_Complex_Management_System.Services;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.Repositories.Base;
using Clinic_Complex_Management_System1.Repositories.Interfaces;
using Clinic_Complex_Management_System1.Services.Base;
using Clinic_Complex_Management_System1.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;

namespace Clinic_Complex_Management_System1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var jwtKey = builder.Configuration["Jwt:Key"]!;
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"];

            builder.Services.AddControllers();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // SQLITE in dev
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddIdentity<User, IdentityRole<Guid>>()
                            .AddEntityFrameworkStores<AppDbContext>()
                            .AddDefaultTokenProviders();

            // ✅ مهم: إلغاء الـ Redirect للـ /Account/Login
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });

            //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            //    {
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuer = false,
            //            ValidIssuer = jwtIssuer,
            //            ValidateAudience = false,
            //            ValidAudience = jwtAudience,
            //            ValidateIssuerSigningKey = true,
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            //            NameClaimType = ClaimTypes.Name,
            //            RoleClaimType = ClaimTypes.Role
            //        };
            //    });

            //builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role
    };


    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = ctx =>
        {
            Console.WriteLine("AUTH FAILED: " + ctx.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = ctx =>
        {
            Console.WriteLine("✅ TOKEN VALIDATED: " + ctx.Principal?.Identity?.Name);
            return Task.CompletedTask;
        },
        OnMessageReceived = ctx =>
        {
            Console.WriteLine("🔍 JWT Received: " + ctx.Token);
            return Task.CompletedTask;
        },
        OnChallenge = ctx =>
        {
            Console.WriteLine("CHALLENGE ERROR: " + ctx.ErrorDescription);
            return Task.CompletedTask;
        }
    };
});

            builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();
            // Swagger w/ JWT
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BankingSystem API", Version = "v1" });

                var jwtScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "JWT Bearer token",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(jwtScheme.Reference.Id, jwtScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtScheme, Array.Empty<string>() } });
            });

            //builder.Services.AddSwaggerGen(options =>
            //{
            //    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //    {
            //        Name = "Authorization",
            //        Type = SecuritySchemeType.Http,
            //        Scheme = "Bearer",
            //        BearerFormat = "JWT",
            //        In = ParameterLocation.Header,
            //        Description = "Enter JWT token as: Bearer {your token}"
            //    });

            //    options.AddSecurityRequirement(new OpenApiSecurityRequirement
            //    {
            //        {
            //            new OpenApiSecurityScheme
            //            {
            //                Reference = new OpenApiReference
            //                {
            //                    Type = ReferenceType.SecurityScheme,
            //                    Id = "Bearer"
            //                }
            //            },
            //            Array.Empty<string>()
            //        }
            //    });
            //});

            // Single, clear CORS policy for React app
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<IClinicRepository, ClinicRepository>();
            builder.Services.AddScoped<IClinicService, ClinicService>();
            builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
            builder.Services.AddScoped<IDoctorService, DoctorService>();
            builder.Services.AddScoped<IHospitalRepository, HospitalRepository>();
            builder.Services.AddScoped<IHospitalService, HospitalService>();
            builder.Services.AddScoped<IPatientRepository, PatientRepository>();
            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IPrescriptionItemRepository, PrescriptionItemRepository>();
            builder.Services.AddScoped<IPrescriptionItemService, PrescriptionItemService>();
            builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
            builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
            builder.Services.AddScoped<IProfileUserRepository, ProfileUserRepository>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapScalarApiReference();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinic API V1");
                });
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinic API V1");
                });
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            // app.UseHttpsRedirection();

            app.UseStaticFiles();


            app.UseCors("AllowReactApp");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
