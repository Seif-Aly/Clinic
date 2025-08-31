<<<<<<< HEAD
=======
// using Clinic_Complex_Management_System.Data;
// using Clinic_Complex_Management_System.Repositories;
// using Clinic_Complex_Management_System.Services;
// using Clinic_Complex_Management_System1.Models;
// using Clinic_Complex_Management_System1.Repositories.Base;
// using Clinic_Complex_Management_System1.Repositories.Interfaces;
// using Clinic_Complex_Management_System1.Services.Base;
// using Clinic_Complex_Management_System1.Services.Interfaces;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.IdentityModel.Tokens;
// using Microsoft.OpenApi.Models;
// using Scalar.AspNetCore;
// using System.Security.Claims;
// using System.Text;

// namespace Clinic_Complex_Management_System1
// {
//     public class Program
//     {
//         public static void Main(string[] args)
//         {
//             var builder = WebApplication.CreateBuilder(args);

//             var jwtKey = builder.Configuration["Jwt:Key"]!;
//             var jwtIssuer = builder.Configuration["Jwt:Issuer"];
//             var jwtAudience = builder.Configuration["Jwt:Audience"];
//             // var AllowFrontend = "_AllowFrontend";
//             builder.Services.AddControllers();

//             builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//             // builder.Services.AddDbContext<AppDbContext>(options =>
//             //     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//             builder.Services.AddDbContext<AppDbContext>(options =>
//              options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


//             builder.Services.AddIdentity<User, IdentityRole<Guid>>()
//                             .AddEntityFrameworkStores<AppDbContext>()
//                             .AddDefaultTokenProviders();

//             builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//                 .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
//                 {
//                     options.TokenValidationParameters = new TokenValidationParameters
//                     {
//                         ValidateIssuer = true,
//                         ValidIssuer = jwtIssuer,
//                         ValidateAudience = true,
//                         ValidAudience = jwtAudience,
//                         ValidateIssuerSigningKey = true,
//                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
//                         NameClaimType = ClaimTypes.Name,
//                         RoleClaimType = ClaimTypes.Role
//                     };
//                 });

//             builder.Services.AddAuthorization();


//             builder.Services.AddEndpointsApiExplorer();
//             builder.Services.AddSwaggerGen(options =>
//             {
//                 options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//                 {
//                     Name = "Authorization",
//                     Type = SecuritySchemeType.Http,
//                     Scheme = "Bearer",
//                     BearerFormat = "JWT",
//                     In = ParameterLocation.Header,
//                     Description = "Enter JWT token as: Bearer {your token}"
//                 });

//                 options.AddSecurityRequirement(new OpenApiSecurityRequirement
//                 {
//                     {
//                         new OpenApiSecurityScheme
//                         {
//                             Reference = new OpenApiReference
//                             {
//                                 Type = ReferenceType.SecurityScheme,
//                                 Id = "Bearer"
//                             }
//                         },
//                         Array.Empty<string>()
//                     }
//                 });
//             });


//             builder.Services.AddCors(options =>
//             {
//                 options.AddPolicy("AllowReactApp",
//                     policy =>
//                     {
//                         policy.WithOrigins("http://localhost:3000")
//                               .AllowAnyHeader()
//                               .AllowAnyMethod()
//                               .AllowCredentials();

//                     });
//             });

//             builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
//             builder.Services.AddScoped<IAppointmentService, AppointmentService>();
//             builder.Services.AddScoped<IClinicRepository, ClinicRepository>();
//             builder.Services.AddScoped<IClinicService, ClinicService>();
//             builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
//             builder.Services.AddScoped<IDoctorService, DoctorService>();
//             builder.Services.AddScoped<IHospitalRepository, HospitalRepository>();
//             builder.Services.AddScoped<IHospitalService, HospitalService>();
//             builder.Services.AddScoped<IPatientRepository, PatientRepository>();
//             builder.Services.AddScoped<IPatientService, PatientService>();
//             builder.Services.AddScoped<IPrescriptionItemRepository, PrescriptionItemRepository>();
//             builder.Services.AddScoped<IPrescriptionItemService, PrescriptionItemService>();
//             builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
//             builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();




//             var app = builder.Build();


//             if (app.Environment.IsDevelopment())
//             {
//                 app.MapScalarApiReference();
//                 app.UseSwagger();
//                 app.UseSwaggerUI(c =>
//                 {
//                     c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinic API V1");
//                 });
//             }
//             else
//             {
//                 app.UseSwagger();
//                 app.UseSwaggerUI(c =>
//                 {
//                     c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinic API V1");
//                 });
//             }

//             app.UseHttpsRedirection();
//             app.UseStaticFiles();
//             app.UseAuthentication();
//             app.UseCors("AllowReactApp");
//             app.UseAuthorization();
//             app.MapControllers();
//             app.UseCors(AllowFrontend);
//             app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

//             app.Run();
//         }
//     }
// }

>>>>>>> main
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
<<<<<<< HEAD
            var AllowFrontend = "_AllowFrontend";
            builder.Services.AddControllers();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
=======

            builder.Services.AddControllers();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // SQLITE in dev
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
>>>>>>> main

            builder.Services.AddIdentity<User, IdentityRole<Guid>>()
                            .AddEntityFrameworkStores<AppDbContext>()
                            .AddDefaultTokenProviders();

<<<<<<< HEAD
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
            });

            builder.Services.AddAuthorization();


=======
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
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
                });

            builder.Services.AddAuthorization();

>>>>>>> main
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token as: Bearer {your token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

<<<<<<< HEAD

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:3000")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();

                    });
=======
            // Single, clear CORS policy for React app
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                    // .AllowCredentials(); // Not needed unless you actually use cookies
                });
>>>>>>> main
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

<<<<<<< HEAD



            var app = builder.Build();


=======
            var app = builder.Build();

>>>>>>> main
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
            app.UseStaticFiles();
<<<<<<< HEAD
            app.UseAuthentication();
            app.UseCors("AllowReactApp");
            app.UseAuthorization();
            app.MapControllers();
            app.UseCors(AllowFrontend);
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
=======

            app.UseCors("AllowReactApp");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
>>>>>>> main

            app.Run();
        }
    }
}
