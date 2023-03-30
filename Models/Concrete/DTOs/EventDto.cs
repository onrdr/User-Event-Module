namespace Models.Concrete.DTOs;

public class EventDto
{
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string Timezone { get; set; }
    public string Address { get; set; }
}
