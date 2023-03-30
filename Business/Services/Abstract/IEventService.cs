using Core.Results;
using Models.Concrete.Entities;

namespace Business.Services.Abstract;

public interface IEventService
{
    Task<IDataResult<Event>> GetEventInfoAsync(int eventId);
    Task<IDataResult<List<Event>>> GetAllEventsAsync(int page, int pageSize);
    Task<IDataResult<List<Event>>> GetCreatedEventListForUserAsync(int userId);
    Task<IDataResult<List<Event>>> GetParticipatedEventListForUserAsync(int userId);
    Task<IDataResult<Event>> CreateEventAsync(int creatorId, Event newEvent);
    Task<IDataResult<Event>> UpdateEventAsync(Event updatedEvent);
    Task<IResult> DeleteEventAsync(int userId, int eventId);
    Task<IResult> RegisterEventAsync(int userId, int eventId);
    Task<IResult> SendInvitationAsync(int creatorId, int eventId, List<int> userIds);
    Task<IDataResult<List<Invitation>>> GetReceivedInvitationsAsync(int userId);
    Task<IDataResult<List<Invitation>>> GetSentInvitationsAsync(int userId);
    Task<IResult> ParticipateInvitationAsync(int invitationId);
}
