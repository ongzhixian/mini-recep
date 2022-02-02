using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Recep.Models;

public interface ICustomer
{
    string Id { get; set; }
    
    string Name { get; set; }

    DateTime DateCreated { get; set; }

}

public class Customer : ICustomer
{
    [Key]
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    //[JsonPropertyName("Wind")]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    //[BsonId]
    //[BsonRepresentation(BsonType.ObjectId)]
    //public string Id { get; set; } = string.Empty;

    //public string Title { get; set; } = string.Empty;

    //[BsonElement("content")]
    //public string Content { get; set; } = string.Empty;

    //[BsonElement("date_created")]
    //public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    //[BsonElement("tags")]
    //public string[] Tags { get; set; } = new string[] { };

}
