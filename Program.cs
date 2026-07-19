using BudgetTracker.Data;
using BudgetTracker.Repositories.Implementations;
using BudgetTracker.Repositories.Interfaces;
using BudgetTracker.Services.Implementations;
using BudgetTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.RateLimiting;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection for Repositories and Services
// Whenever a class ask for IGenericRepository<T>, it will get an instance of GenericRepository<T>
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IIncomeRepository, IncomeRepository>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IInvestmentRepository, InvestmentRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IImportRepository, ImportRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAuthenticationAppService, AuthenticationAppService>();
builder.Services.AddScoped<IIncomeAppService, IncomeAppService>();
builder.Services.AddScoped<IExpenseAppService, ExpenseAppService>();
builder.Services.AddScoped<IInvestmentAppService, InvestmentAppService>();
builder.Services.AddScoped<IReportAppService, ReportAppService>();
builder.Services.AddScoped<ITagAppService, TagAppService>();
builder.Services.AddScoped<IImportAppService, ImportAppService>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IImportQueueService, ImportQueueService>();

// Token Validation Parameters
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
    ClockSkew = TimeSpan.Zero // Optional: eliminate clock skew
};

// Register the token validation parameters as a singleton so it can be injected wherever needed
builder.Services.AddSingleton(tokenValidationParameters);

var storageConnectionString = builder.Configuration["AzureStorage:ConnectionString"];
if (string.IsNullOrEmpty(storageConnectionString))
{
    throw new InvalidOperationException("Azure Storage connection string is not configured.");
}
builder.Services.AddSingleton(new BlobServiceClient(storageConnectionString));
builder.Services.AddSingleton(new QueueServiceClient(storageConnectionString));

// Add Identity services 
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<AppDbContext>();

// Register AutoMapper
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

// Define rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("ApiReportPolicy", context =>
    {
        var userId = context.User.FindFirst("sub")?.Value
                     ?? context.User.Identity?.Name
                     ?? "anonymous";
        return RateLimitPartition.GetFixedWindowLimiter(partitionKey: userId, factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10, // Max 10 requests
            Window = TimeSpan.FromMinutes(1), // Per minute
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0 // No queuing, reject immediately when limit is reached
        });
    });
});

// Build authentication
builder.Services.AddAuthentication()
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = tokenValidationParameters;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Discover API endpoints
builder.Services.AddEndpointsApiExplorer();
// Register Swagger generator. Creates the API description
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });

    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("https://tomonobu22.github.io") // specify the allowed origins
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Apply pending EF Core migrations and create the database if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    // Exposes the API description as JSON
    app.UseSwagger();
    // Renders a web UI for exploring the API endpoints
    app.UseSwaggerUI();
    //app.UseCors("AllowAll");
}

app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();
app.MapStaticAssets();

app.MapRazorPages(); // important for Identity pages
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
