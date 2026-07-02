namespace BookingManagement.Domain.Entities;

public class BookingUser
{
    public string Id { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;

    private BookingUser() { }

    public BookingUser(string id, string displayName)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("User id is required.", nameof(id));

        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("User display name is required.", nameof(displayName));

        Id = id;
        DisplayName = displayName;
    }
}