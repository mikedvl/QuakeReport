using System.Text.Json.Serialization;

namespace QuakeReport.ParserCore.Models
{
    public class GameRecords
    {
        [JsonPropertyName("games")]
        public Dictionary<string, KillsByMeansData> Games { get; set; } = new();
    }
}

