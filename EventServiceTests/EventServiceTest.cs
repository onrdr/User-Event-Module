using DataAccess;
using Models.Concrete.Entities;
using Business.Services.Concrete;
using Core.Results;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Business.Services.Abstract;
using Business.Constants;
using ServiceTests.Fixture;

namespace Tests.ServiceTests;

[TestFixture]
public class EventServiceTests
{
    private AppDbContext _dbContext;
    private IEventService _eventService;
    private DbContextOptions<AppDbContext> _dbContextOptions;

    [SetUp]
    public void Setup()
    {
        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _dbContext = new AppDbContext(_dbContextOptions);
        _eventService = new EventService(_dbContext);
        _dbContext.Database.EnsureCreated();
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
    } 

    [Test]
    public async Task GetEventInfoAsync_WhenEventDoesNotExist_ReturnsErrorDataResultWith()
    {
        // Action
        var result = await _eventService.GetEventInfoAsync(1);

        // Assert
        result.Should().BeOfType<ErrorDataResult<Event>>();
        result.Message.Should().Be(Messages.EventNotFound);
    }

    [Test]
    public async Task GetEventInfoAsync_WhenEventExists_ReturnsSuccessDataResul()
    {
        // Arrange  
        var eventToAdd = DataFixture.GetEvents().First();
        _dbContext.Add(eventToAdd);
        _dbContext.SaveChanges();

        // Action
        var result = await _eventService.GetEventInfoAsync(eventToAdd.Id);

        // Assert
        result.Should().BeOfType<SuccessDataResult<Event>>();
        result.Data.Id.Should().Be(eventToAdd.Id);
        result.Data.Name.Should().Be(eventToAdd.Name);
    }

    [Test]
    public async Task GetAllEventsAsync_WhenEventsDoNotExist_ReturnsErrorDataResult()
    {
        // Action
        var result = await _eventService.GetAllEventsAsync(page: 1, pageSize: 10);

        // Assert
        result.Should().BeOfType<ErrorDataResult<List<Event>>>();
        result.Message.Should().Be(Messages.EmptyEventList);
    }

    [Test]
    public async Task GetAllEventsAsync_WhenPageIsLessThanOne_ReturnsErrorDataResult()
    {
        // Action
        var result = await _eventService.GetAllEventsAsync(page: 0, pageSize: 10);

        // Assert
        result.Should().BeOfType<ErrorDataResult<List<Event>>>();
        result.Message.Should().Be(Messages.PaginationNumberError);
    }

    [Test]
    public async Task GetAllEventsAsync_WhenPageSizeIsLessThanOne_ReturnsErrorDataResult()
    {
        // Action
        var result = await _eventService.GetAllEventsAsync(page: 1, pageSize: 0);

        // Assert
        result.Should().BeOfType<ErrorDataResult<List<Event>>>();
        result.Message.Should().Be(Messages.PaginationNumberError);
    }

    [Test]
    public async Task GetAllEventsAsync_WhenPageSizeIsGraterThan50_ReturnsErrorDataResult()
    {
        // Action 
        var result = await _eventService.GetAllEventsAsync(page: 1, pageSize: 51);

        // Assert
        result.Should().BeOfType<ErrorDataResult<List<Event>>>();
        result.Message.Should().Be(Messages.PageSizeOver50);
    }

    [Test]
    public async Task GetAllEventsAsync_WhenEventsExist_ReturnsSuccessDataResult()
    {
        // Arrange 
        var events = DataFixture.GetEvents().GetRange(0, 3).ToList();
        _dbContext.AddRange(events);
        _dbContext.SaveChanges();

        // Action
        var result = await _eventService.GetAllEventsAsync(page: 1, pageSize: 10);

        // Assert
        result.Should().BeOfType<SuccessDataResult<List<Event>>>();
        result.Data.Should().HaveCount(events.Count);
        result.Data.Should().BeEquivalentTo(events);
    } 

    [Test]
    public async Task GetCreatedEventListForUserAsync_WhenUserDoesNotExis_ReturnsErrorDataResult()
    {
        // Arrange 
        var userId = 1;

        // Action
        var result = await _eventService.GetCreatedEventListForUserAsync(userId);

        // Assert
        result.Should().BeOfType<ErrorDataResult<List<Event>>>();
        result.Message.Should().Be(Messages.UserNotFound);
    }

    [Test]
    public async Task GetCreatedEventListForUserAsync_WhenUserExistsAndListIsEmpty_ReturnsErrorDataResult()
    {
        // Arrange   
        var user = DataFixture.GetUsers().First();

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Action
        var result = await _eventService.GetCreatedEventListForUserAsync(user.Id);

        // Assert
        result.Should().BeOfType<ErrorDataResult<List<Event>>>();
        result.Message.Should().Be(Messages.EmptyEventList);
    }

    [Test]
    public async Task GetCreatedEventListForUserAsync_WhenUserExistsAndListIsNotEmpty_ReturnsSuccessDataResult()
    {
        // Arrange   
        var user = DataFixture.GetUsers().First();
        var events = DataFixture.GetEvents().GetRange(0, 3).ToList();
        events.ForEach(e => e.CreatorId = user.Id);

        await _dbContext.Users.AddAsync(user);
        await _dbContext.Events.AddRangeAsync(events);
        await _dbContext.SaveChangesAsync();

        // Action
        var result = await _eventService.GetCreatedEventListForUserAsync(user.Id);

        // Assert
        result.Should().BeOfType<SuccessDataResult<List<Event>>>();
        result.Data.Should().HaveCount(events.Count);
        result.Data.Should().BeEquivalentTo(events);
    }

    [Test]
    public async Task GetParticipatedEventListForUserAsync_WhenUserDoesNotExist_ReturnsErrorDataResult()
    {
        // Arrange
        var userId = 1;

        // Action
        var result = await _eventService.GetParticipatedEventListForUserAsync(userId);

        // Assert
        result.Should().BeOfType<ErrorDataResult<List<Event>>>();
        result.Message.Should().Be(Messages.UserNotFound);
    }

    [Test]
    public async Task GetParticipatedEventListForUserAsync_WhenUserExistsAndListIsEmpty_ReturnsErrorDataResult()
    {
        // Arrange  
        var user = DataFixture.GetUsers().First();

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Action
        var result = await _eventService.GetParticipatedEventListForUserAsync(user.Id);

        // Assert
        result.Should().BeOfType<ErrorDataResult<List<Event>>>();
        result.Message.Should().Be(Messages.EmptyEventList);
    }

    [Test]
    public async Task GetParticipatedEventListForUserAsync_WhenUserExistsAndListIsNotEmpty_ReturnsSuccessDataResult()
    {
        // Arrange  
        var user = DataFixture.GetUsers().First();
        var events = DataFixture.GetEvents().GetRange(0, 3).ToList();

        var participation = new List<Participant>
        {
            new Participant { UserId = user.Id, EventId = 1 },
            new Participant { UserId = user.Id, EventId = 3 }
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.Events.AddRangeAsync(events);
        await _dbContext.Participants.AddRangeAsync(participation);
        await _dbContext.SaveChangesAsync();

        // Action
        var result = await _eventService.GetParticipatedEventListForUserAsync(user.Id);

        // Assert
        result.Should().BeOfType<SuccessDataResult<List<Event>>>();
        result.Data.Should().HaveCount(participation.Count);
        result.Data.Should().BeEquivalentTo(events.Where(e => e.Id != 2).ToList());
    }

    [Test]
    public async Task CreateEventAsync_WhenUserExistsAndEventIsCreated_ReturnsSuccessDataResult()
    {
        // Arrange 
        var user = DataFixture.GetUsers().First();
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var newEvent = DataFixture.GetEvents().First();
        newEvent.CreatorId = user.Id;

        // Action
        var result = await _eventService.CreateEventAsync(user.Id, newEvent);

        // Assert
        result.Should().BeOfType<SuccessDataResult<Event>>();
        result.Data.Id.Should().NotBe(0);
        result.Data.CreatorId.Should().Be(user.Id);
        result.Data.Name.Should().Be(newEvent.Name);
        result.Data.Description.Should().Be(newEvent.Description);
        result.Data.EndDate.Should().Be(newEvent.EndDate);
    }

    [Test]
    public async Task CreateEventAsync_WhenUserDoesNotExist_ReturnsErrorDataResult()
    {
        // Arrange
        var creatorId = 1;
        var newEvent = DataFixture.GetEvents().First();

        // Action
        var result = await _eventService.CreateEventAsync(creatorId, newEvent);

        // Assert
        result.Should().BeOfType<ErrorDataResult<Event>>();
        result.Message.Should().Be(Messages.UserNotFound);
    }

    [Test]
    public async Task UpdateEventAsync_WhenUserNotFound_ReturnsErrorDataResult()
    {
        // Arrange
        var updatedEvent = DataFixture.GetEvents().First();
        updatedEvent.CreatorId = 1;

        // Action
        var result = await _eventService.UpdateEventAsync(updatedEvent);

        // Assert
        result.Should().BeOfType<ErrorDataResult<Event>>();
        result.Message.Should().Be(Messages.UserNotFound);
    }

    [Test]
    public async Task UpdateEventAsync_WhenEventNotFound_ReturnsErrorDataResult()
    {
        // Arrange
        var user = DataFixture.GetUsers().First();
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var updatedEvent = DataFixture.GetEvents().First();
        updatedEvent.CreatorId = user.Id;

        // Action
        var result = await _eventService.UpdateEventAsync(updatedEvent);

        // Assert
        result.Should().BeOfType<ErrorDataResult<Event>>();
        result.Message.Should().Be(Messages.EventNotFound);
    }

    [Test]
    public async Task UpdateEventAsync_WhenUserIsNotAuthorizedToUpdate_ReturnsErrorDataResult()
    {
        // Arrange
        var user = DataFixture.GetUsers().First();
        var user2 = DataFixture.GetUsers()[1];
        _dbContext.Users.AddRange(user, user2);

        var existingEvent = DataFixture.GetEvents().First();
        existingEvent.CreatorId = user.Id;
        _dbContext.Events.Add(existingEvent);
        await _dbContext.SaveChangesAsync();

        var updatedEvent = DataFixture.GetEvents().First();
        updatedEvent.CreatorId = user2.Id;

        // Action
        var result = await _eventService.UpdateEventAsync(updatedEvent);

        // Assert
        result.Should().BeOfType<ErrorDataResult<Event>>();
        result.Message.Should().Be(Messages.NotAuthorizeToUpdate);
    }

    [Test]
    public async Task UpdateEventAsync_WhenUpdateSuccessful_ReturnsSuccessDataResult()
    {
        // Arrange
        var user = DataFixture.GetUsers()[1];
        _dbContext.Users.Add(user);

        var existingEvent = DataFixture.GetEvents().First();
        user.CreatedEvents.Add(existingEvent);
        _dbContext.Events.Add(existingEvent);
        await _dbContext.SaveChangesAsync();

        var updatedEvent = new Event
        {
            Id = 1,
            CreatorId = 2,
            Name = "Updated Name",
            Title = "Updated Title",
            Description = "Updated Description",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(2),
            Timezone = "UTC+1",
            Address = "Updated Address"
        };

        // Action
        var result = await _eventService.UpdateEventAsync(updatedEvent);

        // Assert
        result.Should().BeOfType<SuccessDataResult<Event>>();
        result.Message.Should().Be(Messages.EventUpdated);

        var dataResult = (SuccessDataResult<Event>)result;
        dataResult.Data.Name.Should().Be(updatedEvent.Name);
        dataResult.Data.Title.Should().Be(updatedEvent.Title);
        dataResult.Data.Description.Should().Be(updatedEvent.Description);
        dataResult.Data.StartDate.Should().Be(updatedEvent.StartDate);
        dataResult.Data.EndDate.Should().Be(updatedEvent.EndDate);
        dataResult.Data.Timezone.Should().Be(updatedEvent.Timezone);
        dataResult.Data.Address.Should().Be(updatedEvent.Address);
    }

    [Test]
    public async Task DeleteEventAsync_WhenUserNotFound_ReturnsErrorResult()
    {
        // Arrange
        var user = DataFixture.GetUsers().First();
        var eventToDelete = DataFixture.GetEvents().First();

        // Action
        var result = await _eventService.DeleteEventAsync(user.Id, eventToDelete.Id);

        // Assert
        result.Should().BeOfType<ErrorResult>();
        result.Message.Should().Be(Messages.UserNotFound);
    }

    [Test]
    public async Task DeleteEventAsync_WhenEventNotFound_ReturnsErrorDataResult()
    {
        // Arrange
        var user = DataFixture.GetUsers().First();
        var eventToDelete = DataFixture.GetEvents().First();
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Action
        var result = await _eventService.DeleteEventAsync(user.Id, eventToDelete.Id);

        // Assert
        result.Should().BeOfType<ErrorResult>();
        result.Message.Should().Be(Messages.EventNotFound);
    }

    [Test]
    public async Task DeleteEventAsync_WhenUserIsNotAuthorizedToDelete_ShouldReturnErrorResult()
    {
        // Arrange
        var user = DataFixture.GetUsers().First();
        var user2 = DataFixture.GetUsers()[1];
        var eventToDelete = DataFixture.GetEvents().First();
        eventToDelete.CreatorId = user2.Id;
        _dbContext.Users.AddRange(user, user2);
        _dbContext.Events.Add(eventToDelete);
        await _dbContext.SaveChangesAsync();

        // Action
        var result = await _eventService.DeleteEventAsync(user.Id, eventToDelete.Id);

        // Assert
        result.Should().BeOfType<ErrorResult>();
        result.Message.Should().Be(Messages.NotAuthorizeToDelete);
    }

    [Test]
    public async Task DeleteEventAsync_WhenUserAndEventExist_ShouldReturnSuccessResult()
    {
        // Arrange
        var user = DataFixture.GetUsers().First();
        var eventToDelete = DataFixture.GetEvents().First();
        eventToDelete.CreatorId = user.Id;
        _dbContext.Users.Add(user);
        _dbContext.Events.Add(eventToDelete);
        await _dbContext.SaveChangesAsync();

        // Action
        var result = await _eventService.DeleteEventAsync(user.Id, eventToDelete.Id);

        // Assert
        result.Should().BeOfType<SuccessResult>();
        result.Message.Should().Be(Messages.EventDeleted);
    }

    [Test]
    public async Task RegisterEventAsync_WhenUserNotFound_ShouldReturnErrorResult()
    {
        // Arrange
        var user = DataFixture.GetUsers().First();
        var eventToRegister = DataFixture.GetEvents().First();
        await _dbContext.SaveChangesAsync();

        // Action
        var result = await _eventService.RegisterEventAsync(user.Id, eventToRegister.Id);

        // Assert
        result.Should().BeOfType<ErrorResult>();
        result.Message.Should().Be(Messages.UserNotFound);
    }

    [Test]
    public async Task RegisterEventAsync_WhenEventNotFound_ShouldReturnErrorResult()
    {
        // Arrange
        var user = DataFixture.GetUsers().First();
        var eventToRegister = DataFixture.GetEvents().First();
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Action
        var result = await _eventService.RegisterEventAsync(user.Id, eventToRegister.Id);

        // Assert
        result.Should().BeOfType<ErrorResult>();
        result.Message.Should().Be(Messages.EventNotFound);
    }

    [Test]
    public async Task RegisterEventAsync_WhenCreatorTriesToRegister_ShouldReturnErrorResult()
    {
        // Arrange
        var user = DataFixture.GetUsers().First();
        var eventToRegister = DataFixture.GetEvents().First();
        eventToRegister.CreatorId = user.Id;
        _dbContext.Users.Add(user);
        _dbContext.Events.Add(eventToRegister);
        await _dbContext.SaveChangesAsync();

        // Action
        var result = await _eventService.RegisterEventAsync(user.Id, eventToRegister.Id);

        // Assert
        result.Should().BeOfType<ErrorResult>();
        result.Message.Should().Be(Messages.CreatorCannotBeParticipant);
    }

    [Test]
    public async Task RegisterEventAsync_WhenUserAlreadyAnHasInvitation_ShouldReturnSuccessResult()
    {
        // Arrange
        var invitee = DataFixture.GetUsers().First();
        var inviter = DataFixture.GetUsers().Last();
        var eventToRegister = DataFixture.GetEvents().First();
        eventToRegister.CreatorId = inviter.Id;
        _dbContext.Users.Add(invitee);
        _dbContext.Users.Add(inviter);
        _dbContext.Events.Add(eventToRegister);
        _dbContext.Participants.Add(new Participant { User = invitee, Event = eventToRegister });
        _dbContext.Invitations.Add(new Invitation
        { Invitee = invitee, Inviter = inviter, Event = eventToRegister, Message = "Invitation Message" });
        await _dbContext.SaveChangesAsync();

        // Action
        var result = await _eventService.RegisterEventAsync(invitee.Id, eventToRegister.Id);

        // Assert
        result.Should().BeOfType<ErrorResult>();
        result.Message.Should().Be(Messages.AlreadyHaveAnInvitation);
    }

    [Test]
    public async Task RegisterEventAsync_WhenUserHasAlreadyRegistered_ShouldReturnErrorResult()
    {
        // Arrange
        var user = DataFixture.GetUsers().First();
        var eventToRegister = DataFixture.GetEvents().First();
        _dbContext.Users.Add(user);
        _dbContext.Events.Add(eventToRegister);
        eventToRegister.Participants.Add(new Participant { User = user, Event = eventToRegister });
        await _dbContext.SaveChangesAsync();

        // Action
        var result = await _eventService.RegisterEventAsync(user.Id, eventToRegister.Id);

        // Assert
        result.Should().BeOfType<ErrorResult>();
        result.Message.Should().Be(Messages.UserAlreadyParticipated);
    }

    [Test]
    public async Task RegisterEventAsync_WhenUserRegistersForTheFirstTime_ShouldReturnSuccessResult()
    {
        // Arrange
        var user = DataFixture.GetUsers().First();
        var eventToRegister = DataFixture.GetEvents().First();
        _dbContext.Users.Add(user);
        _dbContext.Events.Add(eventToRegister);
        await _dbContext.SaveChangesAsync();

        // Action
        var result = await _eventService.RegisterEventAsync(user.Id, eventToRegister.Id);

        // Assert
        result.Should().BeOfType<SuccessResult>();
        result.Message.Should().Be(Messages.SuccessfulRegistration);
    }

    [Test]
    public async Task SendInvitationAsync_WhenCreatorNotFound_ReturnsErrorResult()
    {
        // Arrange
        var creator = DataFixture.GetUsers().First();
        var eventToInvite = DataFixture.GetEvents().First();
        var userIds = new List<int> { 2, 3 };

        // Action
        var result = await _eventService.SendInvitationAsync(creator.Id, eventToInvite.Id, userIds);

        // Assert
        result.Should().BeOfType<ErrorResult>();
        result.Message.Should().Be(Messages.CreatorNotFound);
    }

    [Test]
    public async Task SendInvitationAsync_WhenEventNotFound_ReturnsErrorResult()
    {
        // Arrange
        var creator = DataFixture.GetUsers().First();
        _dbContext.Users.Add(creator);
        await _dbContext.SaveChangesAsync();
        var eventToInvite = DataFixture.GetEvents().First();
        var userIds = new List<int> { 2, 3 };

        // Action
        var result = await _eventService.SendInvitationAsync(creator.Id, eventToInvite.Id, userIds);

        // Assert
        result.Should().BeOfType<ErrorResult>();
        result.Message.Should().Be(Messages.EventNotFound);
    }

    [Test]
    public async Task SendInvitationAsync_WhenCreatorIsNotAuthorizeToInvite_ReturnsErrorResult()
    {
        // Arrange
        var creator = DataFixture.GetUsers().First();
        var user2 = DataFixture.GetUsers().First(u => u.Id == 4);
        var eventToInvite = DataFixture.GetEvents().First();
        eventToInvite.CreatorId = creator.Id;
        _dbContext.Users.AddRange(creator, user2);
        _dbContext.Events.Add(eventToInvite);
        await _dbContext.SaveChangesAsync();
        var userIds = new List<int> { 2, 3 };

        // Action
        var result = await _eventService.SendInvitationAsync(user2.Id, eventToInvite.Id, userIds);

        // Assert
        result.Should().BeOfType<ErrorResult>();
        result.Message.Should().Be(Messages.NotAuthorizeToInvite);
    }

    [Test]
    public async Task SendInvitationAsync_WhenInviteeNotFound_ReturnsErrorResult()
    {
        // Arrange
        var creator = DataFixture.GetUsers().First();
        var eventToInvite = DataFixture.GetEvents().First();
        eventToInvite.CreatorId = creator.Id;
        _dbContext.Users.Add(creator);
        _dbContext.Events.Add(eventToInvite);
        await _dbContext.SaveChangesAsync();
        var userIds = new List<int> { 2, 3, 4 };

        // Action
        var result = await _eventService.SendInvitationAsync(creator.Id, eventToInvite.Id, userIds);

        // Assert
        result.Should().BeOfType<ErrorResult>();
        result.Message.Should().Be(Messages.InviteeNotFound);
    }

    [Test]
    public async Task SendInvitationAsync_WhenInvitationSentSuccessfully_SendsInvitationsAndReturnsSuccessResult()
    {
        // Arrange
        var creator = DataFixture.GetUsers().First();
        var user2 = DataFixture.GetUsers().First(u => u.Id == 2);
        var user3 = DataFixture.GetUsers().First(u => u.Id == 3);
        var eventToInvite = DataFixture.GetEvents().First();
        eventToInvite.CreatorId = creator.Id;
        _dbContext.Users.AddRange(creator, user2, user3);
        _dbContext.Events.Add(eventToInvite);
        await _dbContext.SaveChangesAsync();
        var userIds = new List<int> { 2, 3 };
        var initialInvitationCount = _dbContext.Invitations.ToListAsync().Result.Count;

        // Action
        var result = await _eventService.SendInvitationAsync(creator.Id, eventToInvite.Id, userIds);

        // Assert
        result.Should().BeOfType<SuccessResult>();
        result.Message.Should().Be(Messages.InvitationsSend);
        var finalInvitationCount = _dbContext.Invitations.ToListAsync().Result.Count;
        finalInvitationCount.Should().Be(initialInvitationCount + userIds.Count);
    }

    [Test]
    public async Task SendInvitationAsync_WhenUserAlreadyParticipatingTheEvent_ReturnsErrorResult()
    {
        // Arrange
        var creator = DataFixture.GetUsers().First();
        var user2 = DataFixture.GetUsers().First(u => u.Id == 2);
        var user3 = DataFixture.GetUsers().First(u => u.Id == 3);
        var eventToInvite = DataFixture.GetEvents().First();
        eventToInvite.CreatorId = creator.Id;
        eventToInvite.Participants = new List<Participant>
    {
        new Participant {UserId = user2.Id, EventId = eventToInvite.Id},
        new Participant {UserId = user3.Id, EventId = eventToInvite.Id}
    };
        _dbContext.Users.AddRange(creator, user2, user3);
        _dbContext.Events.Add(eventToInvite);
        await _dbContext.SaveChangesAsync();
        var userIds = new List<int> { 2, 3 };

        // Action
        var result = await _eventService.SendInvitationAsync(creator.Id, eventToInvite.Id, userIds);

        // Assert
        result.Should().BeOfType<ErrorResult>();
        result.Message.Should().Be(Messages.AlreadyParticipatingTheEvent);
    }

    [Test]
    public async Task GetReceivedInvitationsAsync_WhenUserNotFound_ReturnsErrorDataResult()
    {
        // Arrange
        var user = DataFixture.GetUsers().First();

        // Action
        var result = await _eventService.GetReceivedInvitationsAsync(user.Id);

        // Assert
        result.Should().BeOfType<ErrorDataResult<List<Invitation>>>();
        result.Data.Should().BeNull();
        result.Message.Should().Be(Messages.UserNotFound);
    }

    [Test]
    public async Task GetReceivedInvitationsAsync_WhenUserHasNoReceivedInvitations_ReturnsErrorDataResult()
    {
        // Arrange
        var user = DataFixture.GetUsers().First();
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Action
        var result = await _eventService.GetReceivedInvitationsAsync(user.Id);

        // Assert
        result.Should().BeOfType<ErrorDataResult<List<Invitation>>>();
        result.Data.Should().BeNull();
        result.Message.Should().Be(Messages.EmptyReceivedInvitationList);
    }

    [Test]
    public async Task GetReceivedInvitationsAsync_WhenUserHasReceivedInvitations_ReturnsSuccessDataResult()
    {
        // Arrange
        var invitee = DataFixture.GetUsers().First();
        var events = DataFixture.GetEvents().GetRange(0, 2);

        var invitation1 = new Invitation { InviterId = 1, Invitee = invitee, Event = events[0], Message = "Invitation1 Message" };
        var invitation2 = new Invitation { InviterId = 2, Invitee = invitee, Event = events[1], Message = "Invitation2 Message" };

        _dbContext.Users.Add(invitee);
        _dbContext.Events.AddRange(events);
        _dbContext.Invitations.AddRange(new[] { invitation1, invitation2 });
        await _dbContext.SaveChangesAsync();

        // Action
        var result = await _eventService.GetReceivedInvitationsAsync(invitee.Id);

        // Assert
        result.Should().BeOfType<SuccessDataResult<List<Invitation>>>();
        result.Data.Should().HaveCount(2);
        result.Data.Should().ContainEquivalentOf(invitation1);
        result.Data.Should().ContainEquivalentOf(invitation2);
        result.Message.Should().BeNull();
    }

    [Test]
    public async Task GetSentInvitationsAsync_WhenUserNotFound_ReturnsErrorDataResult()
    {
        // Arrange
        var user = DataFixture.GetUsers().First();

        // Action
        var result = await _eventService.GetSentInvitationsAsync(user.Id);

        // Assert
        result.Should().BeOfType<ErrorDataResult<List<Invitation>>>();
        result.Data.Should().BeNull();
        result.Message.Should().Be(Messages.UserNotFound);
    }

    [Test]
    public async Task GetSentInvitationsAsync_WhenUserHasNoSentInvitations_ReturnsErrorDataResult()
    {
        // Arrange
        var user = DataFixture.GetUsers().First();
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Action
        var result = await _eventService.GetSentInvitationsAsync(user.Id);

        // Assert
        result.Should().BeOfType<ErrorDataResult<List<Invitation>>>();
        result.Data.Should().BeNull();
        result.Message.Should().Be(Messages.EmptySentInvitationList);
    }

    [Test]
    public async Task GetSentInvitationsAsync_WhenUserExistsAndHasInvitations_ReturnsSuccessResultWithData()
    {
        // Arrange
        var user = DataFixture.GetUsers().First();
        var invitation1 = new Invitation { InviterId = user.Id, InviteeId = 2, EventId = 1, Message = "Invitation1 Message" };
        var invitation2 = new Invitation { InviterId = user.Id, InviteeId = 3, EventId = 2, Message = "Invitation2 Message" };
        user.InvitationsSent.Add(invitation1);
        user.InvitationsSent.Add(invitation2);
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Action
        var result = await _eventService.GetSentInvitationsAsync(user.Id);

        // Assert
        // Assert
        result.Should().BeOfType<SuccessDataResult<List<Invitation>>>();
        result.Data.Should().HaveCount(2);
        result.Data.Should().ContainEquivalentOf(invitation1);
        result.Data.Should().ContainEquivalentOf(invitation2);
        result.Message.Should().BeNull();
    }

    [Test]
    public async Task ParticipateInvitationAsync_WhenInvitationNotFound_ShouldReturnErrorResult()
    {
        // Arrange
        var invitationId = 1;

        // Action
        var result = await _eventService.ParticipateInvitationAsync(invitationId);

        // Assert
        result.Should().BeOfType<ErrorResult>();
        result.Message.Should().Be(Messages.InvitationNotFound);
    }

    [Test]
    public async Task ParticipateInvitationAsync_WhenInvitationIsFound_ShouldReturnSuccessResult()
    {
        // Arrange
        var invitation = new Invitation
        {
            Id = 1,
            EventId = 1,
            InviteeId = 2,
            IsAccepted = false,
            Message = "Invitation Message"
        };
        _dbContext.Invitations.Add(invitation);
        await _dbContext.SaveChangesAsync();

        // Action
        var result = await _eventService.ParticipateInvitationAsync(invitation.Id);

        // Assert
        result.Should().BeOfType<SuccessResult>();
        result.Message.Should().Be(Messages.InvitationAccepted);
    }
}












