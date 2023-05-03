using JATrackrAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace JATrackrAPI.Services;

/**
 * Class for handling CRUD operations on JobData model by interacting with the database's JobData Collection; part of business logic
 * Used by DI via constructor injections
 * Based off of Microsoft's official tutorial: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-7.0&tabs=visual-studio-code#add-a-crud-operations-service
 */
public class JobDataService
{
    // MongoCollection object, representing JobData Collection, for CRUDing it easily
    private readonly IMongoCollection<User> _jobDataCollection;

    // Constructor to setup initial object state using Database config options class as parameter
    public JobDataService(IOptions<DatabaseSettings> usersDatabaseSettings) 
    {
        // Read server instance for running database ops
        var mongoClient = new MongoClient(usersDatabaseSettings.Value.DBConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(usersDatabaseSettings.Value.DBName);

        // Access data on JobData Collection
        _jobDataCollection = mongoDatabase.GetCollection<User>(usersDatabaseSettings.Value.JobDataCollectionName);
    }

    // Method(s) for: CREATE

    // Method(s) for: READ

    // Method(s) for: UPDATE

    // Methods for: DELETE
}

