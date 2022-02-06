using Mini.Wms.DataAbstraction;

// Mini.Wms.Abstraction
namespace Mini.Wms.DataAbstraction
{
    public interface IObject<T>
    {
        T Id { get; init; }
        DateTime CreatedDateTime { get; init; }
        DateTime LastUpdatedDateTime { get; init; }
    }

    public interface IUser<T> : IObject<T>
    {
        string Username { get; init; }
        string FirstName { get; init; }
        string LastName { get; init; }
        string Password { get; init; }
        DateTime PasswordUpdatedDateTime { get; init; }
    }

    //public interface IItem : IObject<string>
    //{
    //    string Name { get; init; }
    //    string Description { get; init; }
    //}
}

//namespace Mini.Wms.MongoDbAbstraction
//{
//    public interface IUser : IObject<string>
//    {
//        string Username { get; init; }
//        string FirstName { get; init; }
//        string LastName { get; init; }
//        string Password { get; init; }
//        DateTime PasswordUpdatedDateTime { get; init; }
//    }
//}

namespace Mini.Wms.MongoDbImplementation
{
    public readonly record struct User : IUser<string>
    {
        public string Username { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public string FirstName { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public string LastName { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public string Password { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public DateTime PasswordUpdatedDateTime { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public string Id { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public DateTime CreatedDateTime { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public DateTime LastUpdatedDateTime { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
    }

    //    [JsonPropertyName("id")]
    //    [BsonId]
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string Id { get; set; } = string.Empty;

    //    [JsonPropertyName("username")]
    //    [BsonElement("username")]
    //    public string Username { get; set; } = string.Empty;

}



namespace Mini.Wms.SqliteDbImplementation
{
    public readonly record struct User : IUser<int>
    {
        public string Username { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public string FirstName { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public string LastName { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public string Password { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public DateTime PasswordUpdatedDateTime { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public int Id { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public DateTime CreatedDateTime { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public DateTime LastUpdatedDateTime { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
    }

    //    [JsonPropertyName("id")]
    //    [BsonId]
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string Id { get; set; } = string.Empty;

    //    [JsonPropertyName("username")]
    //    [BsonElement("username")]
    //    public string Username { get; set; } = string.Empty;

}
