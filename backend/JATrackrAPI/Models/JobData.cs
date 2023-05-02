using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace JATrackrAPI.Models;

/**
 * Class for representing Job Application Data Model
 * Associated with each User specific to their Job Applications 
 * Attributes of this entity are defined by properties such as job title, company, link to job posting, etc. 
 */
public class JobDataModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public int? Id { get; set; }

    // ? => nullable reference type; does not have to be non-null
    public string? JobTitle { get; set; }

    [BsonElement("CompanyName")]
    public string? Company { get; set; }

    [BsonElement("JobID")]
    public int JobId { get; set; }
}

