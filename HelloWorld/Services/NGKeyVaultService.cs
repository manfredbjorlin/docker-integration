public static class NGKeyVaultService
{
    public static string? ApplicationName;
    public static string GetSecret(string name)
    {
        if(Statics.IsDevelopment)
            return Statics.Configuration!.GetValue<string>(name) ?? string.Empty;

        var client = new SecretClient(vaultUri: new Uri(Statics.Configuration!.GetValue<string>("VaultUri")!), credential: new DefaultAzureCredential());

        return client.GetSecret($"{ApplicationName}--{name}").Value.Value;
    } 
}