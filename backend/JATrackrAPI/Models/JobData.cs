using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace JATrackrAPI.Models;

/**
 * Class for representing Job Application Data Model
 * Associated with each User specific to their Job Applications 
 * Attributes of this entity are defined by properties such as job title, company, link to job posting, etc. 
 */
public class JobData
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    // Property for establishing Many-to-1 relation; reference 1 User data model/document associated with this document
    [BsonRepresentation(BsonType.ObjectId)]
    public string? UserId { get; set; }

    [BsonElement("Title")]
    public string? JobTitle { get; set; }

    [BsonElement("CompanyName")]
    public string? Company { get; set; }

    [BsonElement("JobID")]
    public string? JobId { get; set; }
}

