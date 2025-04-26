
namespace QuakeReport.ParserCore.Interfaces
{
        public interface IGameInfoManager
        {
            bool InitGame(string gameName);
            bool AddPlayer(string playerName, int playerId);
            bool Kill(int sourcePlayerId, int targetPlayerId, int weaponId);
            void ShutdownGame();
            string Report();
            string KillsByMeansReport();
            string? GetCurrentGameName();
            string[]? GetPlayers(string gameName);
        }
}

