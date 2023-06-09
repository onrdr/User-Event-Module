﻿using AutoMapper;
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
    private readonly IEventService _service;
    private readonly IMapper _mapper;

    public EventsController(IEventService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet("{eventId}")]
    public async Task<IActionResult> GetEvent(int eventId)
    {
        var result = await _service.GetEventInfoAsync(eventId);

        return result.Success
            ? Ok(result.Data)
            : NotFound(result.Message);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEvents(int page, int pageSize)
    {
        var result = await _service.GetAllEventsAsync(page, pageSize);

        return result.Success
            ? Ok(result.Data)
            : BadRequest(result.Message);
    }

    [HttpGet("created/{userId}")]
    public async Task<IActionResult> GetCreatedEventListForUser(int userId)
    {
        var result = await _service.GetCreatedEventListForUserAsync(userId);

        return result.Success
            ? Ok(result.Data)
            : NotFound(result.Message);
    }

    [HttpGet("participated/{userId}")]
    public async Task<IActionResult> GetParticipatedEventListForUser(int userId)
    {
        var result = await _service.GetParticipatedEventListForUserAsync(userId);

        return result.Success
            ? Ok(result.Data)
            : NotFound(result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvent(int creatorId, EventDto eventDto)
    {
        var newEvent = _mapper.Map<Event>(eventDto);

        var result = await _service.CreateEventAsync(creatorId, newEvent);

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

        var result = await _service.UpdateEventAsync(eventToUpdate);

        return result.Success
            ? Ok(result)
            : result.Message == Messages.NotAuthorizeToUpdate
                ? BadRequest(result.Message)
                : NotFound(result.Message);
    }

    [HttpDelete("{userId}/{eventId}")]
    public async Task<IActionResult> DeleteEvent(int creatorId, int eventId)
    {
        var result = await _service.DeleteEventAsync(creatorId, eventId);

        return result.Success
             ? Ok(result.Message)
             : result.Message == Messages.NotAuthorizeToDelete
                 ? BadRequest(result.Message)
                 : NotFound(result.Message);
    }

    [HttpPost("register/{userId}/{eventId}")]
    public async Task<IActionResult> RegisterEvent(int userId, int eventId)
    {
        var result = await _service.RegisterEventAsync(userId, eventId);

        return result.Success
             ? Ok(result.Message)
             : result.Message == Messages.UserAlreadyParticipated || result.Message == Messages.AlreadyHaveAnInvitation
                 ? BadRequest(result.Message)
                 : NotFound(result.Message);
    }

    [HttpPost("invite/{creatorId}/{eventId}")]
    public async Task<IActionResult> SendInvitation(InvitationDto data)
    {
        var result = await _service.SendInvitationAsync(data.CreatorId, data.EventId, data.UserIds);

        return result.Success
            ? Ok(result.Message)
            : result.Message == Messages.NotAuthorizeToInvite || result.Message == Messages.AlreadyParticipatingTheEvent 
                ? BadRequest(result.Message)
                : NotFound(result.Message);
    }

    [HttpGet("users/{userId}/invitations/received")]
    public async Task<IActionResult> GetReceivedInvitations(int userId)
    {
        var result = await _service.GetReceivedInvitationsAsync(userId);

        return result.Success
          ? Ok(result.Data)
          : NotFound(result.Message);
    }

    [HttpGet("users/{userId}/invitations/sent")]
    public async Task<IActionResult> GetSentInvitations(int userId)
    {
        var result = await _service.GetSentInvitationsAsync(userId);

        return result.Success
          ? Ok(result.Data)
          : NotFound(result.Message);
    }

    [HttpPost("invitations/{invitationId}/participate")]
    public async Task<IActionResult> ParticipateInvitation(int invitationId)
    {
        var result = await _service.ParticipateInvitationAsync(invitationId);

        return result.Success
            ? Ok(result.Message)
            : NotFound(result.Message);
    }
}

