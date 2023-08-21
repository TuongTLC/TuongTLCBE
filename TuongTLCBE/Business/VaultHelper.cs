using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

namespace TuongTLCBE.Business
{
    public static class VaultHelper
    {
        private static readonly string token = Environment.GetEnvironmentVariable("VaultToken", EnvironmentVariableTarget.Process)
            ?? throw new ArgumentException("Environment variable not found.");
        private static readonly string endpoint = Environment.GetEnvironmentVariable("EndPoint", EnvironmentVariableTarget.Process)
           ?? throw new ArgumentException("Environment variable not found.");
        private static string vaultport = ":8200";
        public static async Task<string> GetJWTTokenAsync()
        {
            try
            {
                IAuthMethodInfo authMethod = new TokenAuthMethodInfo(token);

                VaultClientSettings vaultClientSettings = new(endpoint + vaultport, authMethod);

                IVaultClient vaultClient = new VaultClient(vaultClientSettings);

                Secret<SecretData> kv2Secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "tuongtlc");

                string jwt = kv2Secret.Data.Data["jwt"].ToString() ?? throw new ArgumentException("Failed to retrieve JWT secret.");

                return jwt;
            }
            catch
            {
                throw new ArgumentException("Failed to retrieve JWT secret.");
            }
        }
        public static async Task<string> GetDBConnAsync()
        {
            try
            {
                IAuthMethodInfo authMethod = new TokenAuthMethodInfo(token);

                VaultClientSettings vaultClientSettings = new(endpoint + vaultport, authMethod);

                IVaultClient vaultClient = new VaultClient(vaultClientSettings);

                Secret<SecretData> kv2Secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "tuongtlc");

                string conn = kv2Secret.Data.Data["dbconn"].ToString() ?? throw new ArgumentException("Failed to retrieve DB connection string.");

                return conn;
            }
            catch
            {
                throw new ArgumentException("Failed to retrieve DB connection string.");
            }
        }
    }
}

