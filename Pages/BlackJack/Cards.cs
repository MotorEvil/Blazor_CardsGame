using System.Text.Json.Serialization;

namespace Blazor_CardsGame.Pages.BlackJack
{
    public class Cards
    {
        public string Url { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
}