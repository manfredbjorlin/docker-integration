# How to start

1. Define models in Models/
2. Define method for handling messages from ServiceBus queue in Handlers/ServiceBusHandler.cs
3. Define method for handling messages from Timed events in Handlers/TimerHandler.cs
4. Define Http endpoints in Program.cs
5. Remember to add a appsettings.Development.json in HelloWorld base directory, with "ServiceBusEndpoint", "ServiceBusQueueName" and "VaultUri"
6. Set VaultUri to the correct Vault uri in appsettings.json

x. Write your tests in test project

How to set up project and test: https://www.twilio.com/blog/test-aspnetcore-minimal-apis