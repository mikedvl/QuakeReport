using QuakeReport.ParserCore;
using QuakeReport.ParserCore.Interfaces;
using QuakeReport.Worker;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("log.txt")
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddSingleton<IGameInfoManager, GameInfoManager>();
    builder.Services.AddSingleton<IStateMachine, StateMachine>();
    builder.Services.AddSingleton<IParse, Parser>();
    builder.Services.AddHostedService<Worker>();


    builder.Services.AddControllers();
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
}
catch(Exception ex)
{
    Log.Error(ex, "start app error");
}

