namespace Models.Concrete.DTOs;

public class InvitationDto
{
    public int CreatorId { get; set; }
    public int EventId { get; set; }
    public List<int> UserIds { get; set; } = new();
}
