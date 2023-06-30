using CartCastle.Common.Serialization;
using CartCastle.Domain.Commands;
using CartCastle.Domain.DomainEvents;
using CartCastle.Service.API;
using MediatR;
using CartCastle.Services.Core.Common;
using Hellang.Middleware.ProblemDetails;
using System.Net;
using CartCastle.Common;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false)
                           .AddEnvironmentVariables();
// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<IEventSerializer>(new JsonEventSerializer(new[]
{
                typeof(CustomerEvents.CustomerCreated).Assembly
            })).AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IMediator, Mediator>();
builder.Services.AddTransient<IPaymentService, FakePaymentService>();

builder.Services.Scan(scan =>
{
    scan.FromAssembliesOf(typeof(CreateCustomer))
        .RegisterHandlers(typeof(IRequestHandler<>))
        .RegisterHandlers(typeof(IRequestHandler<,>))
        .RegisterHandlers(typeof(INotificationHandler<>));
});

builder.Services.AddProblemDetails(opts =>
{
    opts.IncludeExceptionDetails = (ctx, ex) =>
    {
        var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
        return env.IsDevelopment() || env.IsStaging();
    };

    opts.MapToStatusCode<ArgumentOutOfRangeException>((int)HttpStatusCode.BadRequest);
    opts.MapToStatusCode<ValidationException>((int)HttpStatusCode.BadRequest);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
