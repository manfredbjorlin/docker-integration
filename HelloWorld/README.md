# How to set up a project

1. Define models in Models/
2. Define method for handling messages from ServiceBus queue in Handlers/ServiceBusHandler.cs
3. Define method for handling messages from Timed events in Handlers/TimerHandler.cs
4. Define Http endpoints in Program.cs
5. Create your mappers in Mappers/
6. Remember to add a appsettings.Development.json in HelloWorld base directory, with "ServiceBusEndpoint", "ServiceBusQueueSendName", "ServiceBusQueueReceiveName" and "VaultUri" (use dev settings)
7. Set VaultUri to the correct prod Vault uri in appsettings.json
0. Write your tests in test project
0. Add Secrets to keyvalt - see below

## Secrets in Keyvault
All secrets in Keyvault should be called ServiceName--SecretName. ServiceName will be added automatically by the KeyVault service.

Secrets needed:

1. ServiceBusNamespace
2. ServiceBusQueuereceiveName
3. ServiceBusQueueSendName

Secrets must be created even if there is no send/receive. Empty values indicates not in use. E.g. empty receive name will not start receiver.

