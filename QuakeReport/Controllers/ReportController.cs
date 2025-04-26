using Microsoft.AspNetCore.Mvc;
using QuakeReport.ParserCore;
using QuakeReport.ParserCore.Interfaces;

namespace QuakeReport.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportController : ControllerBase
{
    private readonly ILogger<ReportController> _logger;
    private readonly IGameInfoManager _gameInfoManager;

    public ReportController(ILogger<ReportController> logger,IGameInfoManager gameInfoManager)
    {
        _logger = logger;
        _gameInfoManager = gameInfoManager;
    }

    [HttpGet(Name = "GetReport")]
    public string Get()
    {
        return _gameInfoManager.Report();
    }
}

