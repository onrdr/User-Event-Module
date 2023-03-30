using Models.Abstract;
using System.Text.Json.Serialization;

namespace Models.Concrete.Entities;

public class Invitation : IBaseEntity
{
    public int Id { get; set; }
    public string Message { get; set; }
    public int EventId { get; set; }
    [JsonIgnore]
    public Event Event { get; set; }
    public int InviterId { get; set; }
    [JsonIgnore]
    public User Inviter { get; set; }
    public int InviteeId { get; set; }
    [JsonIgnore]
    public User Invitee { get; set; }
    public bool IsAccepted { get; set; } = false;
} 
