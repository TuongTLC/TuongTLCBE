using System.Text;
using TuongTLCBE.Data.Models;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

namespace TuongTLCBE.Business;

public static class VaultHelper
{
    private const string VaultPort = ":8200";

    private static readonly string Token =
        Environment.GetEnvironmentVariable("VaultToken", EnvironmentVariableTarget.Process)
        ?? throw new ArgumentException("Environment variable not found.");

    private static readonly string Endpoint =
        Environment.GetEnvironmentVariable("EndPoint", EnvironmentVariableTarget.Process)
        ?? throw new ArgumentException("Environment variable not found.");

    public static async Task<string> GetSecrets(string secretType)
    {
        try
        {
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(Token);

            VaultClientSettings vaultClientSettings = new(Endpoint + VaultPort, authMethod);

            IVaultClient vaultClient = new VaultClient(vaultClientSettings);

            Secret<SecretData> kv2Secret =
                await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("tuongtlc");

            var secret =
                kv2Secret.Data.Data[secretType].ToString()
                ?? throw new ArgumentException("Failed to retrieve secret.");

            return secret;
        }
        catch
        {
            var unseal = await UnsealVaultAsync();
            if (unseal)
                return await GetSecrets(secretType);
            throw new ArgumentException("Environment variable not found.");
        }
    }

    public static async Task<EmailSecretModel> GetEmailSecrets()
    {
        try
        {
            EmailSecretModel emailSecretModel = new();
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(Token);

            VaultClientSettings vaultClientSettings = new(Endpoint + VaultPort, authMethod);

            IVaultClient vaultClient = new VaultClient(vaultClientSettings);

            Secret<SecretData> kv2Secret =
                await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("tuongtlc");

            emailSecretModel.Email =
                kv2Secret.Data.Data["contact_email"].ToString();
            emailSecretModel.Password =
                kv2Secret.Data.Data["contact_password"].ToString();
            return emailSecretModel;
        }
        catch
        {
            var unseal = await UnsealVaultAsync();
            if (unseal)
                return await GetEmailSecrets();
            throw new ArgumentException("Environment variable not found.");
        }
    }

    private static async Task<bool> UnsealVaultAsync()
    {
        var unsealEndpoint = $"{Endpoint}{VaultPort}/v1/sys/unseal";
        var unsealKeys = new[]
        {
            Environment.GetEnvironmentVariable("VaultKey1", EnvironmentVariableTarget.Process)
            ?? throw new ArgumentException("Environment variable not found."),
            Environment.GetEnvironmentVariable("VaultKey2", EnvironmentVariableTarget.Process)
            ?? throw new ArgumentException("Environment variable not found."),
            Environment.GetEnvironmentVariable("VaultKey3", EnvironmentVariableTarget.Process)
            ?? throw new ArgumentException("Environment variable not found.")
        };
        var success = false;
        foreach (var key in unsealKeys)
        {
            using var httpClient = new HttpClient();
            var requestData = $"{{ \"key\": \"{key}\" }}";
            var content = new StringContent(requestData, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(unsealEndpoint, content);
            if (response.IsSuccessStatusCode) success = true;
        }

        return success;
    }
}