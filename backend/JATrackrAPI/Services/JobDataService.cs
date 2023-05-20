using JATrackrAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace JATrackrAPI.Services;

/**
 * Class for handling CRUD operations on JobData model by interacting with the database's JobData Collection; part of business logic
 * Used by DI via constructor injections
 * Based off of Microsoft's official tutorial: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app
 */
public class JobDataService
{
    // MongoCollection object, representing JobData Collection, for CRUDing it easily
    private readonly IMongoCollection<JobData> _jobDataCollection;
    private readonly IMongoCollection<User> _usersCollection;

    // Constructor to setup initial object state using Database config options class as parameter
    public JobDataService(DatabaseSettings usersDatabaseSettings) 
    {
        // Read server instance for running database ops
        var mongoClient = new MongoClient(usersDatabaseSettings.DBConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(usersDatabaseSettings.DBName);

        // Access data on JobData Collection and User Collection
        _jobDataCollection = mongoDatabase.GetCollection<JobData>(usersDatabaseSettings.JobDataCollectionName);
        _usersCollection = mongoDatabase.GetCollection<User>(usersDatabaseSettings.UsersCollectionName);
    }

    // Method(s) for: CREATE

    // Add a new job application to JobData collection
    // HAS TO BE ASSOCIATED WITH ONE EXISTING USER (via user id)
    public async Task CreateJobAppAsync(string userid, JobData newJobApp)
    {   
        
        var userFilter = Builders<User>.Filter.Eq(x => x.Id, userid);
        var matchedUser = await _usersCollection.Find(userFilter).FirstOrDefaultAsync();

        if (matchedUser == null)
        {
            throw new ArgumentException($"No matching user account in database with user id: {userid}");
        }

        await _jobDataCollection.InsertOneAsync(newJobApp);
    }


    // Method(s) for: READ

    // Get list of all job applications stored in system (collection)
    public async Task<List<JobData>> GetAllJobAppsAsync() =>
        await _jobDataCollection.Find(_ => true).ToListAsync();

    // Get list of all job applications (documents) associated with user account through username
    public async Task<List<JobData>> GetJobApplicationsForUserUNAsync(string username)
    {
        // Check if username matches with any existing record in User collection in the system database
        var user = await _usersCollection.Find(x => x.Username == username).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new ArgumentException($"Username: {username} was not found in the system records.");
        }
        // var jobappsfilter = Builders<JobData>.Filter.Eq(j => j.UserId, user.Id);
        var jobApplications = await _jobDataCollection.Find(x => x.UserId == user.Id).ToListAsync();
        return jobApplications;
    }
    // Method(s) for: UPDATE

    // Methods for: DELETE
}

