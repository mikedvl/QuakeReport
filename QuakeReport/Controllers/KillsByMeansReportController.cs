using Microsoft.AspNetCore.Mvc;
using QuakeReport.ParserCore;
using QuakeReport.ParserCore.Interfaces;

namespace QuakeReport.Controllers;

[ApiController]
[Route("[controller]")]
public class KillsByMeansReportController : ControllerBase
{
    private readonly ILogger<KillsByMeansReportController> _logger;
    private readonly IGameInfoManager _gameInfoManager;

    public KillsByMeansReportController(ILogger<KillsByMeansReportController> logger,IGameInfoManager gameInfoManager)
    {
        _logger = logger;
        _gameInfoManager = gameInfoManager;
    }

    [HttpGet(Name = "GetKillsByMeansReport")]
    public string Get()
    {
        return _gameInfoManager.KillsByMeansReport();
    }
}

