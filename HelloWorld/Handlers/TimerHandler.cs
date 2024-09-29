public class TimerHandler
{
    ServiceBusClient _serviceBusClient;
    public TimerHandler(ServiceBusClient serviceBusClient)
    {
        _serviceBusClient = serviceBusClient;

    }

    public void HandleMessage()
    {
        Logger.WriteDebug("This is triggered every ms delay");
    }
}