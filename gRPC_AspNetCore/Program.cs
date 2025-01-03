
using gRPC_AspNetCore.Context;
using gRPC_AspNetCore.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<GrpcContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("GrpcConnection"));
});
// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddScoped<GrpcProductService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GrpcProductService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
