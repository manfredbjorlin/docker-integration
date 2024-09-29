//************************************
// Don't change anything in this file
//************************************
public class ServiceBusClient   
{
    private Azure.Messaging.ServiceBus.ServiceBusClient? _client;
    private ServiceBusSender? _sender;
    private ServiceBusProcessor? _processor;
    private Action<ServiceBusReceivedMessage>? _inputHandler;

    public ServiceBusClient(Action<ServiceBusReceivedMessage> inputHandler, string queueNameSend, string queueNameReceive, string queueNamespace, CancellationToken cancellationToken = default)
    {
        if(string.IsNullOrEmpty(queueNamespace))
            return;

        // This is to run on port 443 instead of 5-thousand-something
        var clientOptions = new ServiceBusClientOptions
                            { 
                                TransportType = ServiceBusTransportType.AmqpWebSockets
                            };

        
        if(Statics.IsDevelopment)
            _client = new Azure.Messaging.ServiceBus.ServiceBusClient(queueNamespace, clientOptions);
        else
            _client = new Azure.Messaging.ServiceBus.ServiceBusClient(queueNamespace, new DefaultAzureCredential(), clientOptions);

        if(!string.IsNullOrEmpty(queueNameSend))
            _sender = _client.CreateSender(queueNameSend);

        _inputHandler = inputHandler;
        if(!string.IsNullOrEmpty(queueNameReceive))
        {
            if(_inputHandler == null)
            {
                Statics.Logger!.LogError("InputHandler is null");
            }
            else
            {
                _processor = _client.CreateProcessor(queueNameReceive, new ServiceBusProcessorOptions());

                _processor.ProcessMessageAsync += MessageHandler;
                _processor.ProcessErrorAsync += ErrorHandler;

                _processor.StartProcessingAsync();
            }
        }

        _ = new Timer(
            o => 
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    if(_processor != null)
                    {
                        _processor.StopProcessingAsync();
                        _processor.DisposeAsync();
                    }

                    if(_sender != null)
                        _sender.DisposeAsync();
                    
                    _client.DisposeAsync();

                    Logger.WriteInfo("ServiceBus Client disposed...");
                }
            },
            null,
            100,
            100
        );
    }

    async Task MessageHandler(ProcessMessageEventArgs args)
    {
        Logger.WriteInfo($"Received message: {args.Message.MessageId}");

        _inputHandler!(args.Message);

        await args.CompleteMessageAsync(args.Message);
    }

    Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Logger.WriteError($"Exception when handling message: {args.Exception.Message}");
        return Task.CompletedTask;
    }

    public async void PostMessage(string[] messages)
    {
        if(_sender == null)
        {
            Logger.WriteError("ServiceBus Send Queue is not configured. Message dropped!");
            return;
        }

        using ServiceBusMessageBatch messageBatch = await _sender.CreateMessageBatchAsync();
    
        foreach(var message in messages)
        {
            try
            {
                if(!messageBatch.TryAddMessage(new ServiceBusMessage(message)))
                {
                    Logger.WriteError($"Could not add message: {message}");
                }
            }
            catch(InvalidOperationException)
            {
                Logger.WriteError($"Invalid operation adding message: {message}");
            }
            catch(System.Runtime.Serialization.SerializationException)
            {
                Logger.WriteError($"Unable to serialize message: {message}");
            }
        }

        await _sender.SendMessagesAsync(messageBatch);

        Logger.WriteInfo($"Message batch sent. Batch contained {messages.Length} message(s).");
    }
}

