# How to set up a project

1. Define models in Models/
2. Define method for handling messages from ServiceBus queue in Handlers/ServiceBusHandler.cs
3. Define method for handling messages from Timed events in Handlers/TimerHandler.cs
4. Define Http endpoints in Program.cs
5. Create your mappers in Mappers/
6. Remember to add a appsettings.Development.json in HelloWorld base directory, with "ServiceBusEndpoint", "ServiceBusQueueName" and "VaultUri" (use dev settings)
7. Set VaultUri to the correct prod Vault uri in appsettings.json

0. Write your tests in test project

All secrets in Keyvault should be called ServiceName__SecretName. ServiceName will be added automatically by the KeyVault service.

---

## External resources

_How to set up project and test: https://www.twilio.com/blog/test-aspnetcore-minimal-apis_