using JATrackrAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace JATrackrAPI.Services;

/**
 * Class for handling CRUD operations on User model by interacting with the database's Users Collection; part of business logic
 * Used by DI via constructor injections
 * Based off of Microsoft's official tutorial: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-7.0&tabs=visual-studio-code#add-a-crud-operations-service
 */
public class UserService
{
    // MongoColleciton object, representing Users Collection, for CRUDing it easily
    private readonly IMongoCollection<User> _usersCollection;

    // Constructor to setup initial object state using Database config options class as parameter
    public UserService(IOptions<DatabaseSettings> usersDatabaseSettings) 
    {
        // Read server instance for running database ops
        var mongoClient = new MongoClient(usersDatabaseSettings.Value.DBConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(usersDatabaseSettings.Value.DBName);

        // Access data on Users Collection
        _usersCollection = mongoDatabase.GetCollection<User>(usersDatabaseSettings.Value.UsersCollectionName);
    }

    // Methods for: READ

    // Get list of all accounts stored in Users collection
    public async Task<List<User>> GetAllUsersAsync() =>
        await _usersCollection.Find(_ => true).ToListAsync();

    // Get user account by id
    public async Task<User?> GetUserByIDAsync(string id) =>
        await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    // Get user account by username 
    public async Task<User?> GetUserByUNAsync(string username) =>
        await _usersCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

    // Get user account by email
    public async Task<User?> GetUserByEmailAsync(string email) =>
        await _usersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

    // Get user account's Job Application docs

    // Method(s) for: CREATE

    // Add a new user account to Users collection
    public async Task CreateUserAsync(User newUser) =>
        await _usersCollection.InsertOneAsync(newUser);

    // Methods for: UPDATE

    // Find an existing user account by id and Update record in Users collection
    public async Task UpdateUserByIDAsync(string id, User updatedUser) =>
        await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

    // Find existing user account by username and Update record in Users collection
    public async Task UpdateUserByUNAsync(string username, User updatedUser) =>
        await _usersCollection.ReplaceOneAsync(x => x.Username == username, updatedUser);

    // Methods for: DELETE
    
    // Find existing user account by id and Delete record from Users collection
    public async Task DeleteUserByIDAsync(string id) =>
        await _usersCollection.DeleteOneAsync(x => x.Id == id);

    // Find existing user account by username and Delete record from Users collection
    public async Task DeleteUserByUNAsync(string username) =>
        await _usersCollection.DeleteOneAsync(x => x.Username == username);
}

