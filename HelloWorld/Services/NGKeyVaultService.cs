public static class NGKeyVaultService
{
    public static string? ApplicationName;
    public static IConfiguration? Configuration;
    public static string GetSecret(string name, bool isDevelopment = false)
    {
        if(isDevelopment)
            return Configuration!.GetValue<string>(name) ?? string.Empty;

        var client = new SecretClient(vaultUri: new Uri(Configuration!.GetValue<string>("VaultUri")!), credential: new DefaultAzureCredential());

        return client.GetSecret($"{ApplicationName}--{name}").Value.Value;
    } 
}