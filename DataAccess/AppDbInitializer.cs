using Microsoft.Extensions.DependencyInjection;
using Models.Concrete.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataAccess;

public static class AppDbInitializer
{
    public static void SeedData(IServiceProvider services)
    {
        var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>(); 

        _ = CreateUsers(context); 
    }

    private static async Task CreateUsers(AppDbContext context)
    {
        if (!context.Users.Any())
        {
            var users = await GetUsers();
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync(); 
        }
    }
    
    public static async Task<List<User>> GetUsers()
    {
        var client = new HttpClient();
        var response = await client.GetAsync("https://jsonplaceholder.typicode.com/users");
        var json = await response.Content.ReadAsStringAsync();
        var users = JsonConvert.DeserializeObject<List<JObject>>(json);

        var userList = new List<User>();
        foreach (var user in users)
        {
            var address = $"{user["address"]["street"]}, {user["address"]["suite"]}, {user["address"]["city"]}, {user["address"]["zipcode"]}";
            var geo = $"{user["address"]["geo"]["lat"]}, {user["address"]["geo"]["lng"]}";
            var company = $"{user["company"]["name"]}, {user["company"]["catchPhrase"]}, {user["company"]["bs"]}";

            userList.Add(new User
            {
                Id = (int)user["id"],
                Name = (string)user["name"],
                Username = (string)user["username"],
                Email = (string)user["email"],
                Address = $"{address}, {geo}",
                Phone = (string)user["phone"],
                Website = (string)user["website"],
                Company = company
            });
        }

        return userList;
    }
}
