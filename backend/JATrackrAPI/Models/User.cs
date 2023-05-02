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

}