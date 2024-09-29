public class ServiceBusHandler
{
    public void HandleMessage(ServiceBusReceivedMessage message)
    {
        Logger.WriteDebug($"I handled an incoming message from queue! It contained: {message.Body}");

        var person = JsonSerializer.Deserialize<PersonExample>(message.Body);

        Logger.WriteDebug($"And i deserialized it: {person!.Firstname} {person!.Lastname}");
    }
}