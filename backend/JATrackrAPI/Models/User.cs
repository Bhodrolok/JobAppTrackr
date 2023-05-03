using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace JATrackrAPI.Models;

/**
 * Class for representing User Data Model
 * Attributes of this entity are defined by (rather generic) properties such as username, password, email address, etc. 
 */
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    // ? => nullable reference type; does not have to be non-null
    public string? Username { get; set; }

    public string? Email { get; set; }

    // Property for establishing 1-to-Many relation; List of JobData documents each represent this User's job applications 
    // Either using embed JobData document ( embedded data modelling ) or use Ids to cross-reference them ( reference data modelling )
    // public List<JobData>? JobApplications { get; set; } OR
    [BsonElement("JobIDs")]
    public List<string>? JobApplicationIds { get; set; }
 }