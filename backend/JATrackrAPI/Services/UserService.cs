using JATrackrAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace JATrackrAPI.Services;

/**
 * Class for handling CRUD operations on User model by interacting with the database's Users Collection; part of business logic
 * Used by DI via constructor injections
 * Based off of Microsoft's official tutorial: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app
 */
public class UserService
{
    // MongoCollection object, representing Users Collection, for CRUDing it easily
    private readonly IMongoCollection<User> _usersCollection;
    // Another similar object for JobData collection 
    private readonly IMongoCollection<JobData> _jobAppDataCollection;

    // Constructor to setup initial object state using Database config options class as parameter
    public UserService(DatabaseSettings usersDatabaseSettings) 
    {
        // Read server instance for running database ops
        var mongoClient = new MongoClient(usersDatabaseSettings.DBConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(usersDatabaseSettings.DBName);

        // Access document data on Users Collection
        _usersCollection = mongoDatabase.GetCollection<User>(usersDatabaseSettings.UsersCollectionName);
        // Access document data on JobData collection that are associated with Users 
        _jobAppDataCollection = mongoDatabase.GetCollection<JobData>(usersDatabaseSettings.JobDataCollectionName);
    }

    // Method(s) for: CREATE

    // Add a new user account to Users collection
    public async Task CreateUserAsync(User newUser) =>
        await _usersCollection.InsertOneAsync(newUser);

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

    // Get user account by username AND email
    public async Task<User?> GetUserByUNAndEmailAsync(string username, string email)
    {
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(u => u.Username, username),
            Builders<User>.Filter.Eq(u => u.Email, email)
            );
        
        var user = await _usersCollection.Find(filter).FirstOrDefaultAsync();

        return user;
    }

    // Get user account by ID AND username
    public async Task<User?> GetUserByIDAndUNAsync(string id, string username)
    {
        var userFilter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(u => u.Id, id),
            Builders<User>.Filter.Eq(u => u.Username, username)
        );
        
        var user = await _usersCollection.Find(userFilter).FirstOrDefaultAsync();

        return user;
    }

    // Check if there is existing user account with username OR ID provided ONLY ONE of them as parameter
    public async Task<bool> DoesUserExist(string IDorUN)
    {   
        // Check if the parameter is a objectId type
        // https://mongodb.github.io/mongo-csharp-driver/2.7/apidocs/html/M_MongoDB_Bson_ObjectId_TryParse.htm
        if (ObjectId.TryParse(IDorUN, out var objectId)){
            var existingUser = await GetUserByIDAsync(IDorUN);
            return existingUser != null;
        }
        else{
            // Assumption that it is an username otherwise
            var existingUser = await GetUserByUNAsync(IDorUN);
            return existingUser != null;
        }
    }

    // Check if there is existing user account with username OR email provided BOTH of them as parameters
    public async Task<bool> UserExists(string username, string email)
    {
        var userFilter = Builders<User>.Filter.Or(
        Builders<User>.Filter.Eq(u => u.Username, username),
        Builders<User>.Filter.Eq(u => u.Email, email)
        );

        var existingUser = await _usersCollection.Find(userFilter).FirstOrDefaultAsync();

        return existingUser != null;
    }

    // Get list of all job application (docs) associated to an existing user account
    public async Task<List<JobData>> GetAllJobAppsUserAsync(string username)
    {
        User user;
        if ( await DoesUserExist(username))
        {
            user = await GetUserByUNAsync(username);
        }else
        {
            throw new Exception("User not found in database!");
        }

        var jobDocumentIds = user.JobDocumentIds ?? new List<string>();

        if (jobDocumentIds.Count == 0)
        {
            // No associated job apps found for User, return empty list
            return new List<JobData>();
        }

        var jobFilter = Builders<JobData>.Filter.In(jd => jd.Id, jobDocumentIds);
        var jobApps = await _jobAppDataCollection.Find(jobFilter).ToListAsync();

        return jobApps;
    }



    // Methods for: UPDATE

    // Find an existing user account by id and Update record in Users collection
    public async Task UpdateUserByIDAsync(string id, User updatedUser) =>
        await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

    // Find existing user account by username and Update record in Users collection
    public async Task UpdateUserByUNAsync(string username, User updatedUser) =>
        await _usersCollection.ReplaceOneAsync(x => x.Username == username, updatedUser);

    // Update user account by adding existing Job Application (JobData document)
    public async Task AddJobAppToUserByIDAsync(string id, string jobAppDataId)
    // jobAppDataId != JD ID posted by HR
    {
        var user = await _usersCollection.FindOneAndUpdateAsync(
            Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.Id, id),
                Builders<User>.Filter.Not(
                    Builders<User>.Filter.ElemMatch(u => u.JobDocumentIds, j => j == jobAppDataId)
                    )
            ),
            Builders<User>.Update.AddToSet(u => u.JobDocumentIds, jobAppDataId)
        );

        if (user == null)
        {
            throw new ArgumentException($"User: {id} not found or job application already associated with user.");
        }
    }

    // Methods for: DELETE
    
    // Delete user record, given user id, from Users collection
    public async Task DeleteUserByIDAsync(string id) =>
        await _usersCollection.DeleteOneAsync(x => x.Id == id);

    // Delete user record, given username, from Users collection
    public async Task DeleteUserByUNAsync(string username) =>
        await _usersCollection.DeleteOneAsync(x => x.Username == username);

    // Delete user record, given user id and username, from Users collection
    public async Task DeleteUserByIDAndUNAsync(string id, string username)
    {
        var userFilter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(u => u.Id, id),
            Builders<User>.Filter.Eq(u => u.Username, username)
        );

        await _usersCollection.DeleteOneAsync(userFilter);
    }
}

