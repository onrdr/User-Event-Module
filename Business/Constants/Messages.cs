namespace Business.Constants;

public static class Messages
{
    public const string EventNotFound = "Event not found";

    public const string UserNotFound = "User not found";

    public const string CreatorNotFound = "The creator of the event is not found";

    public const string InviteeNotFound = "One of the invitee is not found";

    public const string CreatorCannotBeParticipant = "Creator of the event cannot be listed as participant";

    public const string EmptyEventList = "Event list is empty";

    public const string EmptySentInvitationList = "You do not have any sent invitation to a user"; 

    public const string EmptyReceivedInvitationList = "You do not have any received invitation to an event"; 

    public const string EventAdded = "Event added successfully";

    public const string EventUpdated = "Event updated successfully";

    public const string EventDeleted = "Event Deleted Successfully";

    public const string UserAlreadyParticipated = "User is already a participant";

    public const string SuccessfulRegistration = "User registered for the event successfully";

    public const string NotAuthorizeToUpdate = "User is not allowed to update it, user is not the event creator";

    public const string NotAuthorizeToDelete = "User is not the creator of this event, user is not authorize to delete";

    public const string NotAuthorizeToInvite = "User is not the creator of the event, user is not authorize to send invitation";

    public const string InvitationsSend = "Invitations sent to the users successfully";

    public const string InvitationNotFound = "Invitation not found";

    public const string InvitationAccepted = "Participation successful";

    public const string AlreadyHaveAnInvitation = "You already have an invitation to this event. Please register from Received Invitations page";

    public const string PaginationNumberError = "Page number and page size should be grater than 0";

    public const string PageSizeOver50 = "Page size should be less than or equal to 50";

    public const string AlreadyParticipatingTheEvent = "User has already participated / registered for the event";

    public const string SelfInvitationNotAllowed = "The creator of the event cannot send invitation her/himself";
}
