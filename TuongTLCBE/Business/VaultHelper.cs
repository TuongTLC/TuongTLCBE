using System.Text;
using TuongTLCBE.Data.Models;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

namespace TuongTLCBE.Business;

public static class VaultHelper
{
    private static readonly string Token =
        Environment.GetEnvironmentVariable("VaultToken", EnvironmentVariableTarget.Process)
        ?? throw new ArgumentException("Environment variable not found.");

    private static readonly string Endpoint =
        Environment.GetEnvironmentVariable("VaultEndPoint", EnvironmentVariableTarget.Process)
        ?? throw new ArgumentException("Environment variable VaultEndPoint not found.");

    public static async Task<string> GetSecrets(string secretType)
    {
        try
        {
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(Token);

            VaultClientSettings vaultClientSettings = new(Endpoint, authMethod);

            IVaultClient vaultClient = new VaultClient(vaultClientSettings);

            Secret<SecretData> kv2Secret =
                await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("tuongtlc");

            string secret =
                kv2Secret.Data.Data[secretType].ToString()
                ?? throw new ArgumentException("Failed to retrieve secret.");

            return secret;
        }
        catch
        {
            bool unseal = await UnsealVaultAsync();
            return unseal ? await GetSecrets(secretType) : throw new ArgumentException("Environment variable " + secretType + " not found.");
        }
    }

    public static async Task<EmailSecretModel> GetEmailSecrets()
    {
        try
        {
            EmailSecretModel emailSecretModel = new();
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(Token);

            VaultClientSettings vaultClientSettings = new(Endpoint, authMethod);

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
            bool unseal = await UnsealVaultAsync();
            return unseal ? await GetEmailSecrets() : throw new ArgumentException("Email environment variable not found.");
        }
    }

    private static async Task<bool> UnsealVaultAsync()
    {
        string unsealEndpoint = $"{Endpoint}/v1/sys/unseal";
        string[] unsealKeys = new[]
        {
            Environment.GetEnvironmentVariable("VaultKey1", EnvironmentVariableTarget.Process)
            ?? throw new ArgumentException("Environment variable VaultKey1 not found."),
            Environment.GetEnvironmentVariable("VaultKey2", EnvironmentVariableTarget.Process)
            ?? throw new ArgumentException("Environment variable VaultKey2 not found."),
            Environment.GetEnvironmentVariable("VaultKey3", EnvironmentVariableTarget.Process)
            ?? throw new ArgumentException("Environment variable VaultKey3 not found.")
        };
        bool success = false;
        foreach (string? key in unsealKeys)
        {
            using HttpClient httpClient = new();
            string requestData = $"{{ \"key\": \"{key}\" }}";
            StringContent content = new(requestData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(unsealEndpoint, content);
            if (response.IsSuccessStatusCode)
            {
                success = true;
            }
        }

        return success;
    }
}
