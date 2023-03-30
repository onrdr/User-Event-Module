using Models.Abstract;
using System.Text.Json.Serialization;

namespace Models.Concrete.Entities;

public class User : IBaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Website { get; set; }
    public string Company { get; set; }

    [JsonIgnore]
    public List<Event> CreatedEvents { get; set; } = new();
    public List<Participant> ParticipatedEvents { get; set; } = new();
    public List<Invitation> InvitationsSent { get; set; } = new();
    public List<Invitation> InvitationsReceived { get; set; } = new();
}
