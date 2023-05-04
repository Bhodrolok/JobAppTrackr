using Microsoft.Extensions.Options;

namespace JATrackrAPI.Models;

/**
 * Class for representing Database Settings Data Model
 * Configuration settings for establishing connection to Data Storage (MongoDB)
 * Attributes of this entity are defined by Neccessary properties (settings) including Database Connection String, Name of Database, Name of Collection, etc.
 */
public class DatabaseSettings
{
    public string? DBConnectionString { get; set; }

    public string? DBName { get; set; }

    public string? UsersCollectionName { get; set; }
    
    public string? JobDataCollectionName { get; set;}
}