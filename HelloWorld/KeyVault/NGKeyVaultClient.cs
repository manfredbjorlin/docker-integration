public static class NGKeyVaultClient
{
    public static string? ApplicationName;
    public static string GetSecret(string name)
    {
        // Get from KeyVault : Get($"{ApplicationName}__{name}") 

        return name switch
        {
            "ServiceBusQueueName" => "testqueue",
            "ServiceBusNamespace" => "ngsbtest.servicebus.windows.net",
            _ => "Wut?"
        };
    } 
}