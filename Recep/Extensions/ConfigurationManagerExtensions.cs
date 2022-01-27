using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Recep.Extensions;

//public static class IConfigurationExtensions
//{
//    public static T GetSectionAs<T>(this IConfiguration configuration, string section)
//    {
//        var configurationSection = GetConfigurationSection(configuration, section);

//        return configurationSection.Get<T>();
//    }

//    public static IEnumerable<IConfigurationSection> GetSectionChildren(this IConfiguration configuration, string section)
//    {
//        var configurationSection = GetConfigurationSection(configuration, section);

//        return configurationSection.GetChildren();
//    }

//    public static IConfigurationSection GetConfigurationSection(this IConfiguration configuration, string section)
//    {
//        var configurationSection = configuration.GetSection(section);

//        if (!configurationSection.Exists())
//            throw new InvalidOperationException($"Configuration section {section} does not exists.");

//        // CONSIDER: Implementing an Exception class for such an occurrence.
//        // throw new NotExistsException("Configuration section does not exists.", "Jwt:Conso2")
//        // throw new MissingConfigurationSectionException("Jwt:Conso2")

//        return configurationSection;
//    }

//}



//public static class SecurityKeyHelper
//{
//    public static (SymmetricSecurityKey sc, SymmetricSecurityKey ec) SymmetricSecurityKey(string pipeSeparatedValue)
//    {
//        string? hashName = HashAlgorithmName.SHA256.Name;

//        if (hashName == null)
//            throw new InvalidOperationException("Hash algorithm name is null.");

//        var values = pipeSeparatedValue.Split('|', StringSplitOptions.None);

//        if (values.Length < 4)
//            throw new InvalidDataException("Invalid pipe separated value.");

//        byte[] salt = System.Text.Encoding.UTF8.GetBytes(values[0]);
//        byte[] pwd = System.Text.Encoding.UTF8.GetBytes(values[1]);

//        _ = int.TryParse(values[2], out int scIt);
//        _ = int.TryParse(values[3], out int ecIt);

//        return (
//            new(new PasswordDeriveBytes(pwd, salt, hashName, scIt).GetBytes(64)), 
//            new(new PasswordDeriveBytes(pwd, salt, hashName, ecIt).GetBytes(32)));
//    }
//}