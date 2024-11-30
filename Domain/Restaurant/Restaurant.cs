namespace Domain.Restaurants;

public class Restaurant
{
    public RestaurantId Id { get; }
    public string Name { get; private set; }
    public string Description { get; private set; }

    private Restaurant(RestaurantId id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public static Restaurant New(RestaurantId id, string name, string description)
        => new(id, name, description);

    public void UpdateDetails(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
