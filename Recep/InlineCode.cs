using System.Text.Json.Serialization;

namespace Mini.Wms.DomainMessages
{
    //public abstract record NewUser
    //{
    //    // System.NotSupportedException: 
    //    // Deserialization of types without a parameterless constructor,
    //    // a singular parameterized constructor,
    //    // or a parameterized constructor annotated with 'JsonConstructorAttribute' is not supported.
    //    // 

    //    [JsonConstructor]
    //    public NewUser(string Username, string FirstName, string LastName)
    //    {
    //        this.Username = Username;
    //        this.FirstName = FirstName;
    //        this.LastName = LastName;
    //    }

    //    public virtual string Username { get; init; } = string.Empty;
    //    public virtual string FirstName { get; init; } = string.Empty;
    //    public virtual string LastName { get; init; } = string.Empty;

    //}

    //public interface INewUserMessage
    //{
    //    string Username { get; init; }
    //    string FirstName { get; init; }
    //    string LastName { get; init; }

    //}

    ////public record class NewUserMessage : INewUserMessage
    ////{
    ////    public string Username { get; init; } = string.Empty;
    ////    public string FirstName { get; init; } = string.Empty;
    ////    public string LastName { get; init; } = string.Empty;
    ////}

    //public readonly record struct NewUserMessage(string Username, string FirstName, string LastName)
    //: INewUserMessage;
    ////{
    ////    //public string Username { get; init; } = string.Empty;
    ////    //public string FirstName { get; init; } = string.Empty;
    ////    //public string LastName { get; init; } = string.Empty;
    ////}
}
