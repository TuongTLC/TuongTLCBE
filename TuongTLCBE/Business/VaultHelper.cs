using System.Text;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

namespace TuongTLCBE.Business
{
    public static class VaultHelper
    {
        private static readonly string token =
            Environment.GetEnvironmentVariable("VaultToken", EnvironmentVariableTarget.Process)
            ?? throw new ArgumentException("Environment variable not found.");
        private static readonly string endpoint =
            Environment.GetEnvironmentVariable("EndPoint", EnvironmentVariableTarget.Process)
            ?? throw new ArgumentException("Environment variable not found.");
        private static readonly string vaultport = ":8200";

        public static async Task<string> GetSecrets(string secretType)
        {
            try
            {
                IAuthMethodInfo authMethod = new TokenAuthMethodInfo(token);

                VaultClientSettings vaultClientSettings = new(endpoint + vaultport, authMethod);

                IVaultClient vaultClient = new VaultClient(vaultClientSettings);

                Secret<SecretData> kv2Secret =
                    await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "tuongtlc");

                string secret =
                    kv2Secret.Data.Data[secretType].ToString()
                    ?? throw new ArgumentException("Failed to retrieve secret.");

                return secret;
            }
            catch
            {
                var unseal = await UnsealVaultAsync();
                if (unseal == true)
                {
                    return await GetSecrets(secretType);
                }
                else
                {
                    throw new ArgumentException("Environment variable not found.");
                }
            }
        }

        private static async Task<bool> UnsealVaultAsync()
        {
            var unsealEndpoint = $"{endpoint}{vaultport}/v1/sys/unseal";
            var unsealKeys = new[]
            {
                Environment.GetEnvironmentVariable("VaultKey1", EnvironmentVariableTarget.Process)
                    ?? throw new ArgumentException("Environment variable not found."),
                Environment.GetEnvironmentVariable("VaultKey2", EnvironmentVariableTarget.Process)
                    ?? throw new ArgumentException("Environment variable not found."),
                Environment.GetEnvironmentVariable("VaultKey3", EnvironmentVariableTarget.Process)
                    ?? throw new ArgumentException("Environment variable not found.")
            };
            bool success = false;
            foreach (var key in unsealKeys)
            {
                using HttpClient httpClient = new HttpClient();
                var requestData = $"{{ \"key\": \"{key}\" }}";
                var content = new StringContent(requestData, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(unsealEndpoint, content);
                if (response.IsSuccessStatusCode)
                {
                    success = true;
                }
            }
            return success;
        }
    }
}
