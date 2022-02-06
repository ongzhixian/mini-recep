using System.ComponentModel.DataAnnotations;

namespace Recep.Models;

//public class User
//{
//    public string Username { get; set; } = string.Empty;
//    public string FirstName { get; set; } = string.Empty;
//    public string LastName { get; set; } = string.Empty;
//}

//public interface IUser : IObject<string>
//{
//    string Username { get; init; }
//    string FirstName { get; init; }
//    string LastName { get; init; }
//    string Password { get; init; }
//    DateTime PasswordUpdatedDateTime { get; init; }
//}



//public readonly record struct User : IUser
//{
//    public string Username { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
//    public string FirstName { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
//    public string LastName { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
//    public string Password { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
//    public DateTime PasswordUpdatedDateTime { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
//    public int Id { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
//    public DateTime CreatedDateTime { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
//    public DateTime LastUpdatedDateTime { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
//}

//public interface IItem : IObject<int>
//{
//    string Name { get; init; }

//    string Description { get; init; }

//}

//public interface IProduct : IObject<int>
//{
//    string Name { get; init; }

//    string Description { get; init; }

//}

//public interface IObject<T>
//{
//    T Id { get; init; }

//    DateTime CreatedDateTime { get; init; }

//    DateTime LastUpdatedDateTime { get; init; }

//}
