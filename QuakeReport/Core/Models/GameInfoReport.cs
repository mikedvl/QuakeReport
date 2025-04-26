
using System.Text.Json.Serialization;

namespace QuakeReport.ParserCore.Models
{
    public class GameInfoReport
    {
        [JsonPropertyName("total_kills")]
        public int TotalKills { get; set; }

        [JsonPropertyName("kills")]
        public Dictionary<string, int> Kills
        {
            get
            {
                return _playersList.ToDictionary(player => player.Name!, player => player.Score).OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        [JsonPropertyName("players")]
        public string[]? Players
        {
            get
            {
                return _playersList.Select(x => x.Name!).ToArray();
            }
        }

        private List<Player> _playersList;

        public GameInfoReport()
        {
            _playersList = new List<Player>();
        }

        public bool AddPlayer(Player player)
        {
            if (_playersList.FirstOrDefault(c => c.Name!.Equals(player.Name)) != null)
                return false;

            _playersList.Add(player);

            return true;
        }

        public Player? GetPlayer(int id)
        {
            return _playersList.FirstOrDefault(c => c.GetId() == id);
        }
    }
}

