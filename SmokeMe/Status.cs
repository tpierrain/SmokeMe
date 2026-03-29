using System.Text.Json.Serialization;

namespace SmokeMe
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Status
    {
        Executed,
        Timeout,
        Discarded,
        Ignored
    }
}
