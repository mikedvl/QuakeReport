
using System.Text.Json;
using QuakeReport.ParserCore.Enums;
using QuakeReport.ParserCore.Interfaces;
using QuakeReport.ParserCore.Models;

namespace QuakeReport.ParserCore
{
	public class GameInfoManager: IGameInfoManager
    {
        private Dictionary<string, KillsByMeansData> _killsByMeansReportDictionary = new();
        private Dictionary<string, GameInfoReport> _gameInfoReports = new();
        private string? _currentGameName;
        private readonly ILogger<GameInfoManager> _logger;
        private int gameIndex = 1;
        private const int WordId = 1022;
        private const string DashStr = "-";
        private const string JsonSerializationExceptionStr = "Json  serialization exception";

        public GameInfoManager(ILogger<GameInfoManager> logger)
		{
            _logger = logger;
        }

        public string? GetCurrentGameName()
        {
            return _currentGameName;
        }

        public bool InitGame(string gameName)
        {
            if (string.IsNullOrEmpty(gameName))
                return false;

            _currentGameName = string.Join(DashStr, gameName, gameIndex++);
            _gameInfoReports.Add(_currentGameName, new GameInfoReport());

            return true;
        }

        public bool AddPlayer(string playerName, int playerId)
        {
            if (string.IsNullOrEmpty(_currentGameName))
                return false;

            if (!_gameInfoReports.ContainsKey(_currentGameName))
                return false;

            return _gameInfoReports[_currentGameName].AddPlayer(new Player(playerName, playerId));
        }

        public string[]? GetPlayers(string gameName)
        {
            if (!_gameInfoReports.ContainsKey(gameName))
                return null;
            else
            return _gameInfoReports[gameName].Players;
        }

        public bool Kill(int sourcePlayerId,int targetPlayerId, int weaponId)
        {
            if (string.IsNullOrEmpty(_currentGameName))
                return false;

            _gameInfoReports[_currentGameName].TotalKills += 1;

            if (sourcePlayerId == WordId)
            {
                var player = _gameInfoReports[_currentGameName].GetPlayer(targetPlayerId);

                if (player == null)
                    return false;

                player.Score -= 1;
            }
            else
            {
                var player = _gameInfoReports[_currentGameName].GetPlayer(sourcePlayerId);

                if (player == null)
                    return false;

                player.Score += 1;

                AddKillsByMeansData(_currentGameName, weaponId);
            }

            return true;
        }

        public void ShutdownGame()
        {
            _currentGameName = string.Empty;
        }

        public string Report()
        {
            return PrepareJsonData(_gameInfoReports);
        }

        public string KillsByMeansReport()
        {
            SortKillsByMeansData();
            return PrepareJsonData(_killsByMeansReportDictionary);
        }

        public string PrepareJsonData(object obj)
        {
            try
            {
                return JsonSerializer.Serialize(obj, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, JsonSerializationExceptionStr);
            }

            return JsonSerializationExceptionStr;
        }

        private bool AddKillsByMeansData(string gameName, int weaponId)
        {
            MeansOfDeath resultEnum = (MeansOfDeath)weaponId;
            string enumName = resultEnum.ToString();

            if (_killsByMeansReportDictionary.TryGetValue(gameName, out var gameStats))
            {
                gameStats.KillsByMeans.TryGetValue(enumName, out var kills);
                gameStats.KillsByMeans[enumName] = kills + 1;
            }
            else
            {
                _killsByMeansReportDictionary[gameName] = new KillsByMeansData
                {
                    KillsByMeans = { { enumName, 1 } }
                };
            }

            return true;
        }

        private void SortKillsByMeansData()
        {
            foreach (var c in _killsByMeansReportDictionary.Values)
            {
                c.KillsByMeans = c.KillsByMeans.OrderByDescending(x => x.Value)
                                                               .ToDictionary(x => x.Key, x => x.Value);
            }
        }
    }
}

