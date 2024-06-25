using System.Net;
using FluentValidation.AspNetCore;
using HomeworkApp.Bll.Extensions;
using HomeworkApp.Dal.Extensions;
using HomeworkApp.Interceptors;
using HomeworkApp.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using RedLockNet.SERedis.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5213, o => o.Protocols = HttpProtocols.Http2);
});

// Add services to the container.
var services = builder.Services;

//add grpc
services.AddGrpc(x =>
{
    x.Interceptors.Add<RateLimitInterceptor>();
});

//add validation
services.AddFluentValidation(conf =>
{
    conf.RegisterValidatorsFromAssembly(typeof(Program).Assembly);
    conf.AutomaticValidationEnabled = true;
});

var redisConnectionString = builder.Configuration["DalOptions:RedisConnectionString"];
var redisHostPort = redisConnectionString.Split(':');
var redlockEndpoints = new List<RedLockEndPoint>
{
    new RedLockEndPoint(new DnsEndPoint(redisHostPort[0], int.Parse(redisHostPort[1])))
};

//add inner dependencies
services
    .AddBllServices(redlockEndpoints)
    .AddDalInfrastructure(builder.Configuration)
    .AddDalRepositories();

services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
});

services.AddGrpcReflection();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<TasksService>();
app.MapGrpcReflectionService();

// enroll migrations
app.MigrateUp();

app.Run();