using Models.Concrete.Entities;

namespace ServiceTests.Fixture;

internal static class DataFixture
{
    public static List<Event> GetEvents()
    {
        return new List<Event>
        {
            new Event
            {
                Id = 1,
                Name = $"Name1",
                Title = $"Title1",
                Description = $"Description1",
                StartDate = DateTimeOffset.Now,
                EndDate = DateTimeOffset.Now.AddDays(7),
                Timezone = $"Timezone1",
                Address = $"Address1",
            },

            new Event
            {
                Id = 2,
                Name = $"Name2",
                Title = $"Title2",
                Description = $"Description2",
                StartDate = DateTimeOffset.Now,
                EndDate = DateTimeOffset.Now.AddDays(7),
                Timezone = $"Timezone2",
                Address = $"Address2"
            },

            new Event
            {
                Id = 3,
                Name = $"Name3",
                Title = $"Title3",
                Description = $"Description3",
                StartDate = DateTimeOffset.Now,
                EndDate = DateTimeOffset.Now.AddDays(7),
                Timezone = $"Timezone3",
                Address = $"Address3"
            },

            new Event
            {
                Id = 4,
                Name = $"Name4",
                Title = $"Title4",
                Description = $"Description4",
                StartDate = DateTimeOffset.Now,
                EndDate = DateTimeOffset.Now.AddDays(7),
                Timezone = $"Timezone4",
                Address = $"Address4"
            },
        };
    }

    public static List<User> GetUsers()
    {
        return new List<User>
        {
            new User
            {
                Id = 1,
                Name = "Name1",
                Username = "Title1",
                Email = "Description1",
                Address = "Address1",
                Phone = "Phone1",
                Website = "Website1",
                Company = "Company1"
            },

            new User
            {
                Id = 2,
                Name = "Name2",
                Username = "Title2",
                Email = "Description2",
                Address = "Address2",
                Phone = "Phone2",
                Website = "Website2",
                Company = "Company2"
            },

            new User
            {
                Id = 3,
                Name = "Name3",
                Username = "Title3",
                Email = "Description3",
                Address = "Address3",
                Phone = "Phone3",
                Website = "Website3",
                Company = "Company3"
            },

            new User
            {
                Id = 4,
                Name = "Name4",
                Username = "Title4",
                Email = "Description4",
                Address = "Address4",
                Phone = "Phone4",
                Website = "Website4",
                Company = "Company4"
            },

            new User
            {
                Id = 5,
                Name = "Name5",
                Username = "Title5",
                Email = "Description5",
                Address = "Address5",
                Phone = "Phone5",
                Website = "Website5",
                Company = "Company5"
            },

            new User
            {
                Id = 6,
                Name = "Name6",
                Username = "Title6",
                Email = "Description6",
                Address = "Address6",
                Phone = "Phone6",
                Website = "Website6",
                Company = "Company6"
            }
        };
    }
} 

