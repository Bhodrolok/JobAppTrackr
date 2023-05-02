using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace JATrackrAPI.Models;
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

