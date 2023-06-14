//************************************
// Don't change anything in this file
//************************************
public class NGServiceBusClient   
{
    private ServiceBusClient _client;
    private ServiceBusSender _sender;
    private ServiceBusProcessor _processor;
    private Action<ServiceBusReceivedMessage> _inputHandler;
    private string _queueName;

    public NGServiceBusClient(Action<ServiceBusReceivedMessage> inputHandler, string queueName, string queueNamespace, bool isDevelopment, CancellationToken cancellationToken = default)
    {
        var clientOptions = new ServiceBusClientOptions
                            { 
                                TransportType = ServiceBusTransportType.AmqpWebSockets
                            };

        
        if(isDevelopment)
            _client = new ServiceBusClient(queueNamespace, clientOptions);
        else
            _client = new ServiceBusClient(queueNamespace, new DefaultAzureCredential(), clientOptions);

        _sender = _client.CreateSender(queueName);
        _processor = _client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

        _inputHandler = inputHandler;
        _queueName = queueName;

        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;

        _processor.StartProcessingAsync();

        _ = new Timer(
            o => 
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    _processor.StopProcessingAsync();

                    _sender.DisposeAsync();
                    _processor.DisposeAsync();
                    _client.DisposeAsync();

                    NGLogger.WriteInfo("ServiceBus Client disposed...");
                }
            },
            null,
            100,
            100
        );
    }

    async Task MessageHandler(ProcessMessageEventArgs args)
    {
        NGLogger.WriteInfo($"Received message: {args.Message.MessageId}");

        _inputHandler(args.Message);

        await args.CompleteMessageAsync(args.Message);
    }

    Task ErrorHandler(ProcessErrorEventArgs args)
    {
        NGLogger.WriteError($"Exception when handling message: {args.Exception.Message}");
        return Task.CompletedTask;
    }

    public async void PostMessage(string[] messages)
    {
        using ServiceBusMessageBatch messageBatch = await _sender.CreateMessageBatchAsync();
    
        foreach(var message in messages)
        {
            try
            {
                if(!messageBatch.TryAddMessage(new ServiceBusMessage(message)))
                {
                    NGLogger.WriteError($"Could not add message: {message}");
                }
            }
            catch(InvalidOperationException)
            {
                NGLogger.WriteError($"Invalid operation adding message: {message}");
            }
            catch(System.Runtime.Serialization.SerializationException)
            {
                NGLogger.WriteError($"Unable to serialize message: {message}");
            }
        }

        await _sender.SendMessagesAsync(messageBatch);

        NGLogger.WriteInfo($"Message batch sent. Batch contained {messages.Length} message(s).");
    }
}

