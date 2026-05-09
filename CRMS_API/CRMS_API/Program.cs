using CRMS_API.Auth;
using CRMS_API.Data;
using CRMS_API.Endpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using CRMS_API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with an Authorize button for Basic Auth testing
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("basic", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "basic",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your username and password"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            new string[] {}
        }
    });
});

// Register the database context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register our custom Basic Auth handler
builder.Services.AddAuthentication("BasicAuth")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("BasicAuth", null);

// Enables role-based access control on endpoints
builder.Services.AddAuthorization();

// Allow requests from any origin (needed for HTML client files)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors();

// Order matters — authentication must come before authorization
app.UseAuthentication();
app.UseAuthorization();

// Seed the database with initial users on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedData.Initialize(db);
}

// Map all endpoint groups
app.MapAuthEndpoints();
app.MapCarEndpoints();
app.MapBookingEndpoints();

app.Run();