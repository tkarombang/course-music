using System.ComponentModel.DataAnnotations.Schema;
using Backend.Data;
using Backend.Services;
using Backend.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// TAMBAH-1: ADD CORS
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(
		policy =>
		{
			policy.WithOrigins("http://localhost:7009", "http://localhost:5055")
					.AllowAnyHeader()
					.AllowAnyMethod();
		}
	);
});

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IEmailService, EmailService>();

// 1. Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
		options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// TAMBAH: Daftarkan layanan AuthService dan IAuthService
builder.Services.AddScoped<IAuthService, AuthService>();

// 2. Add Controllers
builder.Services.AddControllers();

builder.Services.AddScoped<IFileUploadInterface, FileUploadService>();
builder.Services.AddScoped<ICategoryInterface, CategoryService>();
builder.Services.AddScoped<IUsersInterface, UsersService>();
builder.Services.AddScoped<IEmailService, EmailService>();

//TAMBAH: Layanan Authenticasi JwtBearer
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
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,

		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
	};

	options.Events = new JwtBearerEvents
	{
		OnMessageReceived = context =>
		{
			return Task.CompletedTask;
		},

		OnAuthenticationFailed = context =>
		{
			Console.WriteLine($"TOKEN VALIDATION FAILED: {context.Exception.Message}");
			return Task.CompletedTask;
		}
	};
});

//TAMBAH: Layanan Authorisasi
builder.Services.AddAuthorization();
builder.Services.AddSignalR();

// 3. Add Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new() { Title = "Backend API Kursus Music", Version = "v1" });
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Masukkan token seperti ini: Bearer {token}"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
						new List<string>()
				}
		});
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		var context = services.GetRequiredService<AppDbContext>();

		context.Database.Migrate();
		var initializer = new DataSeeder(context);
		initializer.SeedDataAsync().GetAwaiter().GetResult();
	}
	catch (Exception err)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(err, "ERROR TERJADI SAAT MIGRASI ATAU SEEDING DATA");
		throw;
	}
}

// 4. Configure middleware
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
// TAMBAH: middleware Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 5. Map Controllers
app.MapControllers();

app.Run();
