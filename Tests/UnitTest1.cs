using System.Text.Json;
using Microsoft.Extensions.Logging;
using QuakeReport.ParserCore;
using QuakeReport.ParserCore.Enums;
using QuakeReport.ParserCore.Interfaces;
using QuakeReport.ParserCore.Models;

[TestFixture]
public class ParserTests
{
    private IStateMachine _stateMachine;
    private IGameInfoManager _gameInfoManager;
    private Parser _parser;

    [SetUp]
    public void SetUp()
    {
        ILogger<GameInfoManager> logger = new LoggerFactory().CreateLogger<GameInfoManager>();

        _stateMachine = new StateMachine();
        _gameInfoManager = new GameInfoManager(logger);
        _parser = new Parser(_stateMachine, _gameInfoManager);
    }

    [Test,Order(1)]
    public void ParseLogLine_Should_Parse_InitGame_Command()
    {
        // Arrange
        string logLine = "0:00 InitGame: \\sv_floodProtect\\1\\sv_maxPing\\0\\sv_minPing\\0\\sv_maxRate\\10000\\sv_minRate\\0\\sv_hostname\\Code Miner Server\\g_gametype\\0\\sv_privateClients\\2\\sv_maxclients\\16\\sv_allowDownload\\0\\dmflags\\0\\fraglimit\\20\\timelimit\\15\\g_maxGameClients\\0\\capturelimit\\8\\version\\ioq3 1.36 linux-x86_64 Apr 12 2009\\protocol\\68\\mapname\\q3dm17\\gamename\\baseq3\\g_needpass\\0";

        // Act
        bool result = _parser.ParseLogLine(logLine);

        // Assert
        Assert.IsTrue(result);
        Assert.That(_stateMachine.CurrentState, Is.EqualTo(GameStatus.InitGame));
        Assert.That(_gameInfoManager.GetCurrentGameName(), Is.EqualTo("baseq3-1"));
    }

    [Test, Order(2)]
    public void ParseLogLine_Should_Parse_ClientUserinfoChanged_Command()
    {
        string logLine = "0:00 InitGame: \\sv_floodProtect\\1\\sv_maxPing\\0\\sv_minPing\\0\\sv_maxRate\\10000\\sv_minRate\\0\\sv_hostname\\Code Miner Server\\g_gametype\\0\\sv_privateClients\\2\\sv_maxclients\\16\\sv_allowDownload\\0\\dmflags\\0\\fraglimit\\20\\timelimit\\15\\g_maxGameClients\\0\\capturelimit\\8\\version\\ioq3 1.36 linux-x86_64 Apr 12 2009\\protocol\\68\\mapname\\q3dm17\\gamename\\baseq3\\g_needpass\\0";
        _parser.ParseLogLine(logLine);

        // Arrange
        logLine = "20:34 ClientUserinfoChanged: 2 n\\Isgalamido\\t\\0\\model\\xian/default\\hmodel\\xian/default\\g_redteam\\\\g_blueteam\\\\c1\\4\\c2\\5\\hc\\100\\w\\0\\l\\0\\tt\\0\\tl\\0";

        // Act
        bool result = _parser.ParseLogLine(logLine);

        // Assert
        Assert.IsTrue(result);
        Assert.That(_stateMachine.CurrentState, Is.EqualTo(GameStatus.InitGame));
        Assert.That(_gameInfoManager.GetPlayers("baseq3-1")![0], Is.EqualTo("Isgalamido"));
    }

    
    [Test, Order(3)]
    public void ParseLogLine_Should_Parse_Kill_Command()
    {
        string logLine = "0:00 InitGame: \\sv_floodProtect\\1\\sv_maxPing\\0\\sv_minPing\\0\\sv_maxRate\\10000\\sv_minRate\\0\\sv_hostname\\Code Miner Server\\g_gametype\\0\\sv_privateClients\\2\\sv_maxclients\\16\\sv_allowDownload\\0\\dmflags\\0\\fraglimit\\20\\timelimit\\15\\g_maxGameClients\\0\\capturelimit\\8\\version\\ioq3 1.36 linux-x86_64 Apr 12 2009\\protocol\\68\\mapname\\q3dm17\\gamename\\baseq3\\g_needpass\\0";
        _parser.ParseLogLine(logLine);
        logLine = "20:34 ClientUserinfoChanged: 2 n\\Isgalamido\\t\\0\\model\\xian/default\\hmodel\\xian/default\\g_redteam\\\\g_blueteam\\\\c1\\4\\c2\\5\\hc\\100\\w\\0\\l\\0\\tt\\0\\tl\\0";
        _parser.ParseLogLine(logLine);

        // Arrange
        logLine = "20:54 Kill: 2 3 1: <world> killed Isgalamido by MOD_TRIGGER_HURT";

        // Act
        bool result = _parser.ParseLogLine(logLine);
        var obj = JsonSerializer.Deserialize<Dictionary<string, KillsByMeansData>>(_gameInfoManager.KillsByMeansReport());

        // Assert
        Assert.IsTrue(result);
        Assert.That(_stateMachine.CurrentState, Is.EqualTo(GameStatus.InitGame));
        Assert.That(obj!["baseq3-1"].KillsByMeans["MOD_SHOTGUN"], Is.EqualTo(1));
    }

    [Test, Order(4)]
    public void ParseLogLine_Should_Parse_ShutdownGame_Command()
    {
        // Arrange
        string logLine = "20:37 ShutdownGame:";

        // Act
        bool result = _parser.ParseLogLine(logLine);

        // Assert
        Assert.IsTrue(result);
        Assert.That(_stateMachine.CurrentState, Is.EqualTo(GameStatus.ShutdownGame));
    }

    [Test, Order(5)]
    public void ParseLogLine_Should_Return_False_For_Empty_Line()
    {
        // Arrange
        string logLine = string.Empty;

        // Act
        bool result = _parser.ParseLogLine(logLine);

        // Assert
        Assert.IsFalse(result);
    }
}
