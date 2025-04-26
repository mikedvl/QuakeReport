using System.Text.Json.Serialization;

namespace QuakeReport.ParserCore.Models
{
    public class KillsByMeansData
    {
        [JsonPropertyName("kills_by_means")]
        public Dictionary<string, int> KillsByMeans { get; set; } = new();
    }
}

