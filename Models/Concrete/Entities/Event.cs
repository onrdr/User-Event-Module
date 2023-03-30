using Models.Abstract;
using System.Text.Json.Serialization;

namespace Models.Concrete.Entities;

public class Event : IBaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string Timezone { get; set; }
    public string Address { get; set; }

    public int CreatorId { get; set; }
    [JsonIgnore]
    public User Creator { get; set; }

    public List<Participant> Participants { get; set; } = new(); 
}