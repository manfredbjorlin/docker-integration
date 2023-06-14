public class TimerHandler
{
    NGServiceBusClient _serviceBusClient;
    public TimerHandler(NGServiceBusClient serviceBusClient)
    {
        _serviceBusClient = serviceBusClient;

    }

    public void HandleMessage()
    {
        NGLogger.WriteDebug("This is triggered every ms delay");
    }
}