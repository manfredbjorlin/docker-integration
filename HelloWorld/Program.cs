#region Default Functionality

var cancellationTokenSource = new CancellationTokenSource();
var cancellationToken = cancellationTokenSource.Token;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
Statics.Logger = app.Logger;
Statics.Configuration = app.Configuration;
Statics.IsDevelopment = app.Environment.IsDevelopment();
Statics.ApplicationName = app.Environment.ApplicationName;

NGLogger.LoggingLevel = app.Environment.IsDevelopment() ? NGLogger.LogLevel.Debug : NGLogger.LogLevel.Info;

app.Lifetime.ApplicationStarted.Register(() => NGLogger.WriteInfo("Application started..."));
app.Lifetime.ApplicationStopping.Register(() => cancellationTokenSource.Cancel());
app.Lifetime.ApplicationStopped.Register(() => NGLogger.WriteInfo("Application stopped..."));

app.UseSwagger();
app.UseSwaggerUI();

var serviceBusClient = new NGServiceBusClient(
    inputHandler: new ServiceBusHandler().HandleMessage,
    queueNameSend: NGKeyVaultService.GetSecret("ServiceBusQueueSendName"),
    queueNameReceive: NGKeyVaultService.GetSecret("ServiceBusQueueReceiveName"),
    queueNamespace: NGKeyVaultService.GetSecret("ServiceBusNamespace"),
    cancellationToken);

// Catch default 
app.MapGet("/", () => "Go to /swagger to see the API Definition")
    .ExcludeFromDescription();

#endregion

// *****************************************************************************************************************************************************
// EDIT BELOW THIS LINE

// Use serviceBusClient to send to a ServiceBus queue

app.MapPost("/Send", ([FromBody]PersonExample person) =>
{
    serviceBusClient.PostMessage(new[] {JsonSerializer.Serialize(person)});
    return new Success($"{person.Firstname} {person.Lastname} was added!");
})
    .Produces<Success>(statusCode: 200, contentType: "application/json")
    .Produces<Error>(statusCode: 500, contentType: "application/json")
    .WithOpenApi(operation => new(operation)
    {
        Summary = "This will send a message to the Service Bus",
        Description = "When you write a text it is magically sent to a queue!!!"
    });

// If there is a polling - add a ms delay. Timeout.Infinite will make the timer never start

_ = new NGTimerService(pollingDelayMs: Timeout.Infinite,
                        inputHandler: new TimerHandler(serviceBusClient).HandleMessage,
                        cancellationToken);

// EDIT ABOVE THIS LINE
// *****************************************************************************************************************************************************

app.Run();

// NO NOT REMOVE - This is needed for the test project
public partial class Program { }