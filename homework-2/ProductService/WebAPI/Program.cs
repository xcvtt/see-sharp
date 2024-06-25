using FluentValidation;
using WebAPI.Extensions;
using WebAPI.Interceptors;
using WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRepositories();
builder.Services.AddServices();

builder.Services.AddGrpc(x =>
{
    x.Interceptors.Add<ValidationInterceptor>();
    x.Interceptors.Add<ExceptionInterceptor>();
}).AddJsonTranscoding();

builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrpcService<ProductGrpcService>();

app.Run();