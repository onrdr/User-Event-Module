using Business.Constants;
using Business.Services.Abstract;
using Core.Results;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Models.Concrete.Entities;

namespace Business.Services.Concrete;

public class EventService : IEventService
{
    private readonly AppDbContext _dbContext; 

    public EventService(AppDbContext dbContext)
    {
        _dbContext = dbContext; 
    }

    public async Task<IDataResult<Event>> GetEventInfoAsync(int eventId)
    {
        var requestedEvent = await _dbContext.Events
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        return requestedEvent is null
            ? new ErrorDataResult<Event>(Messages.EventNotFound)
            : new SuccessDataResult<Event>(requestedEvent);
    }

    public async Task<IDataResult<List<Event>>> GetAllEventsAsync(int page, int pageSize)
    {
        if (page < 1 || pageSize < 1)
            return new ErrorDataResult<List<Event>>(Messages.PaginationNumberError);

        if (pageSize > 50)
            return new ErrorDataResult<List<Event>> (Messages.PageSizeOver50);        

        var skip = (page - 1) * pageSize;
        var events = await _dbContext.Events
            .Include(e => e.Participants)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        return events.Any()
            ? new SuccessDataResult<List<Event>>(events)
            : new ErrorDataResult<List<Event>>(Messages.EmptyEventList);
    }

    public async Task<IDataResult<List<Event>>> GetCreatedEventListForUserAsync(int userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.CreatedEvents)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return new ErrorDataResult<List<Event>>(Messages.UserNotFound);

        return user.CreatedEvents.Any()
            ? new SuccessDataResult<List<Event>>(user.CreatedEvents)
            : new ErrorDataResult<List<Event>>(Messages.EmptyEventList);
    }

    public async Task<IDataResult<List<Event>>> GetParticipatedEventListForUserAsync(int userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.ParticipatedEvents).ThenInclude(p => p.Event)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return new ErrorDataResult<List<Event>>(Messages.UserNotFound);

        var evetList = user.ParticipatedEvents.Select(p => p.Event).ToList();

        return evetList.Any()
            ? new SuccessDataResult<List<Event>>(evetList)
            : new ErrorDataResult<List<Event>>(Messages.EmptyEventList);
    }

    public async Task<IDataResult<Event>> CreateEventAsync(int creatorId, Event newEvent)
    {
        var user = await _dbContext.Users
             .Include(u => u.CreatedEvents)
             .FirstOrDefaultAsync(u => u.Id == creatorId);

        if (user is null)
            return new ErrorDataResult<Event>(Messages.UserNotFound);

        await CompleteCreateProcessAsync(newEvent, user);

        return new SuccessDataResult<Event>(newEvent, Messages.EventAdded);
    }

    public async Task<IDataResult<Event>> UpdateEventAsync(Event updatedEvent)
    {
        var user = await _dbContext.Users
           .Include(u => u.CreatedEvents)
           .FirstOrDefaultAsync(u => u.Id == updatedEvent.CreatorId);
        if (user is null)
            return new ErrorDataResult<Event>(Messages.UserNotFound);

        var existingEvent = await _dbContext.Events.FindAsync(updatedEvent.Id);
        if (existingEvent is null)
            return new ErrorDataResult<Event>(Messages.EventNotFound);

        if (!user.CreatedEvents.Contains(existingEvent))
            return new ErrorDataResult<Event>(Messages.NotAuthorizeToUpdate);

        var @event = await CompleteUpdateProcessAsync(updatedEvent, user, existingEvent);

        return new SuccessDataResult<Event>(@event, Messages.EventUpdated);
    }

    public async Task<IResult> DeleteEventAsync(int userId, int eventId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user is null)
            return new ErrorResult(Messages.UserNotFound);

        var eventToDelete = await _dbContext.Events.FindAsync(eventId);
        if (eventToDelete is null)
            return new ErrorResult(Messages.EventNotFound);

        if (!user.CreatedEvents.Contains(eventToDelete))
            return new ErrorResult(Messages.NotAuthorizeToDelete);

        await CompleteDeleteProcessAsync(eventToDelete);

        return new SuccessResult(Messages.EventDeleted);
    }

    public async Task<IResult> RegisterEventAsync(int userId, int eventId)
    {
        var user = await _dbContext.Users
           .Include(u => u.ParticipatedEvents)
           .Include(u => u.InvitationsReceived)
           .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return new ErrorResult(Messages.UserNotFound);

        var eventToRegister = await _dbContext.Events
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (eventToRegister is null)
            return new ErrorResult(Messages.EventNotFound);

        if (user.Id == eventToRegister.CreatorId)
            return new ErrorResult(Messages.CreatorCannotBeParticipant);

        var checkResult = CheckIfUserHaveAnInvitationToThisEvent(user, eventToRegister);
        if (!checkResult.Success)
            return checkResult;

        var result = await RegisterUserIfNotRegisteredBefore(user, eventToRegister);
        if (!result.Success)
            return result;

        return new SuccessResult(Messages.SuccessfulRegistration);
    } 

    public async Task<IResult> SendInvitationAsync(int creatorId, int eventId, List<int> userIds)
    {
        var creator = await _dbContext.Users
            .Include(c => c.CreatedEvents)
            .FirstOrDefaultAsync(e => e.Id == creatorId);
        if (creator is null)
            return new ErrorResult(Messages.CreatorNotFound);

        var eventToInvite = await _dbContext.Events
            .Include(e => e.Creator).ThenInclude(e => e.InvitationsSent)
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == eventId);
        if (eventToInvite is null)
            return new ErrorResult(Messages.EventNotFound);
         
        if (!creator.CreatedEvents.Contains(eventToInvite))
            return new ErrorResult(Messages.NotAuthorizeToInvite);

        if (userIds.Contains(creatorId))
            return new ErrorResult(Messages.SelfInvitationNotAllowed);

        var result = await InviteEachUserAsync(userIds, eventToInvite);
        if (!result.Success)
            return result;

        return new SuccessResult(Messages.InvitationsSend);
    }

    public async Task<IDataResult<List<Invitation>>> GetReceivedInvitationsAsync(int userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.InvitationsReceived)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return new ErrorDataResult<List<Invitation>>(Messages.UserNotFound);

        var receivedInvitations = user.InvitationsReceived.ToList();

        return receivedInvitations.Any()
            ? new SuccessDataResult<List<Invitation>>(receivedInvitations)
            : new ErrorDataResult<List<Invitation>>(Messages.EmptyReceivedInvitationList);
    }

    public async Task<IDataResult<List<Invitation>>> GetSentInvitationsAsync(int userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.InvitationsSent)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return new ErrorDataResult<List<Invitation>>(Messages.UserNotFound);

        var sentInvitations = user.InvitationsSent.ToList();

        return sentInvitations.Any()
            ? new SuccessDataResult<List<Invitation>>(sentInvitations)
            : new ErrorDataResult<List<Invitation>>(Messages.EmptySentInvitationList);
    }

    public async Task<IResult> ParticipateInvitationAsync(int invitationId)
    {
        var invitation = await _dbContext.Invitations
            .FirstOrDefaultAsync(inv => inv.Id == invitationId);

        if (invitation is null)
            return new ErrorResult(Messages.InvitationNotFound);

        await CompleteParticipationProcessAsync(invitation);

        return new SuccessResult(Messages.InvitationAccepted);
    }


    #region Private Functions
    private async Task CompleteCreateProcessAsync(Event newEvent, User user)
    {
        newEvent.CreatorId = user.Id;
        await _dbContext.Events.AddAsync(newEvent);
        user.CreatedEvents.Add(newEvent);
        await _dbContext.SaveChangesAsync();
    }

    private async Task<Event> CompleteUpdateProcessAsync(Event updatedEvent, User user, Event existingEvent)
    {
        user.CreatedEvents.Remove(existingEvent);

        existingEvent.Name = updatedEvent.Name;
        existingEvent.Title = updatedEvent.Title;
        existingEvent.Description = updatedEvent.Description;
        existingEvent.StartDate = updatedEvent.StartDate;
        existingEvent.EndDate = updatedEvent.EndDate;
        existingEvent.Timezone = updatedEvent.Timezone;
        existingEvent.Address = updatedEvent.Address;

        user.CreatedEvents.Add(existingEvent);
        await _dbContext.SaveChangesAsync();
        return existingEvent;
    }    

    private async Task<IResult> InviteEachUserAsync(List<int> userIds, Event eventToInvite)
    {
        foreach (var id in userIds)
        {
            var user = await _dbContext.Users
                .Include(u => u.InvitationsReceived)
                .Include(u => u.ParticipatedEvents).ThenInclude(e => e.Event)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
                return new ErrorResult(Messages.InviteeNotFound);

            foreach (var participant in eventToInvite.Participants)
            {
                if (participant.UserId == user.Id)
                    return new ErrorResult(Messages.AlreadyParticipatingTheEvent);
            }

            await CompleteInvitationProcessAsync(eventToInvite, user);
        }

        await _dbContext.SaveChangesAsync();

        return new SuccessResult();
    }

    private async Task CompleteInvitationProcessAsync(Event eventToInvite, User user)
    {
        var invitation = new Invitation
        {
            Message = $"Hi {user.Name}! Would you like to participate my {eventToInvite.Title}?",
            EventId = eventToInvite.Id,
            InviterId = eventToInvite.CreatorId,
            InviteeId = user.Id
        };

        user.InvitationsReceived.Add(invitation);
        eventToInvite.Creator.InvitationsSent.Add(invitation);
        await _dbContext.Invitations.AddAsync(invitation);
    }

    private async Task<IResult> RegisterUserIfNotRegisteredBefore(User user, Event eventToRegister)
    {
        foreach (var participant in eventToRegister.Participants)
        {
            if (participant.UserId == user.Id && participant.EventId == eventToRegister.Id)
                return new ErrorResult(Messages.UserAlreadyParticipated);
        }

        await CompleteRegisterProcessAsync(user, eventToRegister);

        return new SuccessResult();
    }

    private static IResult CheckIfUserHaveAnInvitationToThisEvent(User user, Event eventToRegister)
    {
        foreach (var invitation in user.InvitationsReceived)
        {
            if (invitation.InviteeId == user.Id
                && invitation.EventId == eventToRegister.Id
                && invitation.InviterId == eventToRegister.CreatorId)
            {
                return new ErrorResult(Messages.AlreadyHaveAnInvitation);
            }
        }

        return new SuccessResult();
    }

    private async Task CompleteRegisterProcessAsync(User user, Event eventToRegister)
    {
        var newParticipant = new Participant { UserId = user.Id, EventId = eventToRegister.Id };
        eventToRegister.Participants.Add(newParticipant); ;
        user.ParticipatedEvents.Add(newParticipant);
        await _dbContext.SaveChangesAsync();
    }

    private async Task CompleteParticipationProcessAsync(Invitation invitation)
    {
        invitation.IsAccepted = true;

        var participant = new Participant
        {
            EventId = invitation.EventId,
            UserId = invitation.InviteeId
        };

        _dbContext.Participants.Add(participant);
        _dbContext.Invitations.Remove(invitation);
        await _dbContext.SaveChangesAsync();
    }

    private async Task CompleteDeleteProcessAsync(Event eventToDelete)
    {
        _dbContext.Events.Remove(eventToDelete);
        await _dbContext.SaveChangesAsync();
    }
    #endregion
}

