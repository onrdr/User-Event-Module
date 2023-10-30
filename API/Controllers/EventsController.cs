using AutoMapper;
using Business.Constants;
using Business.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Models.Concrete.DTOs;
using Models.Concrete.Entities;

namespace WebAPI.Controllers;

[ApiController]
[Route("events")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly IMapper _mapper;

    public EventsController(IEventService service, IMapper mapper)
    {
        _eventService = service;
        _mapper = mapper;
    }

    [HttpGet("{eventId}")]
    public async Task<IActionResult> GetEvent(int eventId)
    {
        var result = await _eventService.GetEventInfoAsync(eventId);
        return result.Success
            ? Ok(result.Data)
            : NotFound(result.Message);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEvents(int page, int pageSize)
    {
        var result = await _eventService.GetAllEventsAsync(page, pageSize);
        return result.Success
            ? Ok(result.Data)
            : BadRequest(result.Message);
    }

    [HttpGet("created/{userId}")]
    public async Task<IActionResult> GetCreatedEventListForUser(int userId)
    {
        var result = await _eventService.GetCreatedEventListForUserAsync(userId);
        return result.Success
            ? Ok(result.Data)
            : NotFound(result.Message);
    }

    [HttpGet("participated/{userId}")]
    public async Task<IActionResult> GetParticipatedEventListForUser(int userId)
    {
        var result = await _eventService.GetParticipatedEventListForUserAsync(userId);
        return result.Success
            ? Ok(result.Data)
            : NotFound(result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvent(int creatorId, EventDto eventDto)
    {
        var newEvent = _mapper.Map<Event>(eventDto);
        var result = await _eventService.CreateEventAsync(creatorId, newEvent);

        return result.Success
            ? Ok(result)
            : NotFound(result.Message);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateEvent(int creatorId, int eventId, EventDto eventDto)
    {
        var eventToUpdate = _mapper.Map<Event>(eventDto);
        eventToUpdate.Id = eventId;
        eventToUpdate.CreatorId = creatorId;

        var result = await _eventService.UpdateEventAsync(eventToUpdate);
        return result.Success
            ? Ok(result)
            : result.Message == Messages.NotAuthorizeToUpdate
                ? BadRequest(result.Message)
                : NotFound(result.Message);
    }

    [HttpDelete("{userId}/{eventId}")]
    public async Task<IActionResult> DeleteEvent(int creatorId, int eventId)
    {
        var result = await _eventService.DeleteEventAsync(creatorId, eventId);
        return result.Success
             ? Ok(result.Message)
             : result.Message == Messages.NotAuthorizeToDelete
                 ? BadRequest(result.Message)
                 : NotFound(result.Message);
    }

    [HttpPost("register/{userId}/{eventId}")]
    public async Task<IActionResult> RegisterEvent(int userId, int eventId)
    {
        var result = await _eventService.RegisterEventAsync(userId, eventId);
        return result.Success
             ? Ok(result.Message)
             : result.Message == Messages.UserAlreadyParticipated || result.Message == Messages.AlreadyHaveAnInvitation
                 ? BadRequest(result.Message)
                 : NotFound(result.Message);
    }

    [HttpPost("invite/{creatorId}/{eventId}")]
    public async Task<IActionResult> SendInvitation(InvitationDto data)
    {
        var result = await _eventService.SendInvitationAsync(data.CreatorId, data.EventId, data.UserIds);
        return result.Success
            ? Ok(result.Message)
            : result.Message == Messages.NotAuthorizeToInvite || result.Message == Messages.AlreadyParticipatingTheEvent 
                ? BadRequest(result.Message)
                : NotFound(result.Message);
    }

    [HttpGet("users/{userId}/invitations/received")]
    public async Task<IActionResult> GetReceivedInvitations(int userId)
    {
        var result = await _eventService.GetReceivedInvitationsAsync(userId);
        return result.Success
          ? Ok(result.Data)
          : NotFound(result.Message);
    }

    [HttpGet("users/{userId}/invitations/sent")]
    public async Task<IActionResult> GetSentInvitations(int userId)
    {
        var result = await _eventService.GetSentInvitationsAsync(userId);
        return result.Success
          ? Ok(result.Data)
          : NotFound(result.Message);
    }

    [HttpPost("invitations/{invitationId}/participate")]
    public async Task<IActionResult> ParticipateInvitation(int invitationId)
    {
        var result = await _eventService.ParticipateInvitationAsync(invitationId);
        return result.Success
            ? Ok(result.Message)
            : NotFound(result.Message);
    }
}

