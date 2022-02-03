using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Recep.Services;

public record class RsaKeySetting2
{
    HttpClient httpClient;
    IHttpClientFactory httpClientFactory;

    //public RsaKeySetting2()
    //{
    //}

    public RsaKeySetting2(IHttpClientFactory httpClientFactory)
    {
        this.httpClient = httpClientFactory.CreateClient();
    }

    //public RsaKeySetting2(HttpClient httpClient)
    //{
    //    this.httpClient = httpClient;
    //}

    public RsaKeyDataSource SourceType { get; init; } = RsaKeyDataSource.Unknown;

    public string Source { get; init; } = string.Empty;

    private async Task<string> SourceXmlAsync()
    {
        string rsaXml = string.Empty;

        if (SourceType == RsaKeyDataSource.EnvironmentVariable)
        {
            rsaXml = Environment.GetEnvironmentVariable(Source) ?? rsaXml;
        }

        if (SourceType == RsaKeyDataSource.File)
        {
            if (!File.Exists(Source))
                throw new FileNotFoundException("File not found", Source);

            rsaXml = File.ReadAllText(Source);
        }

        if (SourceType == RsaKeyDataSource.Http)
        {
            rsaXml = await httpClient.GetStringAsync(Source);
        }

        return rsaXml;
    }

    public void EnsureIsValid()
    {
        if (SourceType == RsaKeyDataSource.Unknown)
            throw new InvalidOperationException("SourceType is unknown");

        if (string.IsNullOrWhiteSpace(Source))
            throw new InvalidOperationException("Source is null or whitespace");
    }

    public async Task<RsaSecurityKey> GetRsaSecurityKeyAsync(bool withPrivateParameters)
    {
        using RSA rsa = RSA.Create();

        rsa.FromXmlString(await SourceXmlAsync());

        return new RsaSecurityKey(rsa.ExportParameters(withPrivateParameters));
    }

    public async Task<string> GetRsaSecurityKeyXmlAsync(bool withPrivateParameters)
    {
        using RSA rsa = RSA.Create();

        rsa.FromXmlString(await SourceXmlAsync());

        return rsa.ToXmlString(withPrivateParameters);
    }

    public enum RsaKeyDataSource
    {
        Unknown = 0,
        EnvironmentVariable = 1,
        File = 2,
        Http = 3
    }
}