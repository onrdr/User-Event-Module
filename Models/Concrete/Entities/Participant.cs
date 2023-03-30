using System.Text.Json.Serialization;

namespace Models.Concrete.Entities;

public class Participant
{
    public int UserId { get; set; }
    [JsonIgnore]
    public User User { get; set; }

    public int EventId { get; set; }
    [JsonIgnore]
    public Event Event { get; set; }
}
