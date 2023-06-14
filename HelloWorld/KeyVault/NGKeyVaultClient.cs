

public static class NGKeyVaultClient
{
    public static string? ApplicationName;
    public static string GetSecret(string name, string VaultUri, bool isDevelopment = false)
    {
        if(isDevelopment)
            return name switch
            {
                "ServiceBusQueueName" => "testqueue",
                "ServiceBusNamespace" => "ngsbtest.servicebus.windows.net",
                _ => "Wut?"
            };

        var client = new SecretClient(vaultUri: new Uri(VaultUri), credential: new DefaultAzureCredential());

        return client.GetSecret($"{ApplicationName}__{name}").Value.Value;
    } 
}