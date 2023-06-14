public class ServiceBusHandler
{
    public void HandleMessage(ServiceBusReceivedMessage message)
    {
        NGLogger.WriteDebug($"I handled the message! It contained: {message.Body}");

        var person = JsonSerializer.Deserialize<PersonExample>(message.Body);

        NGLogger.WriteDebug($"And i deserialized it: {person!.Firstname} {person!.Lastname}");
    }
}