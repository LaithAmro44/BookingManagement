namespace BookingManagement.Domain.Entities;

public class Resource
{
    public string Id { get; private set; } = null!;
    public string Name { get; private set; } = null!;

    private Resource() { }

    public Resource(string id, string name)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Resource id is required.", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Resource name is required.", nameof(name));

        Id = id;
        Name = name;
    }
}