using WebAPI.Extensions;
using WebAPI.Middleware;
using WebAPI.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<RepositoryFilePathOptions>(
    builder.Configuration.GetSection(RepositoryFilePathOptions.RepositoryFilePath));

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("WebAPI/appsettings.json")
    .Build();

builder.Services.AddProductService(configuration);
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<ExceptionHandlingMiddleware>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
