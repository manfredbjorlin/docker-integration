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

Logger.LoggingLevel = app.Environment.IsDevelopment() ? Logger.LogLevel.Debug : Logger.LogLevel.Info;

app.Lifetime.ApplicationStarted.Register(() => Logger.WriteInfo("Application started..."));
app.Lifetime.ApplicationStopping.Register(() => cancellationTokenSource.Cancel());
app.Lifetime.ApplicationStopped.Register(() => Logger.WriteInfo("Application stopped..."));

app.UseSwagger();
app.UseSwaggerUI();

var serviceBusClient = new ServiceBusClient(
    inputHandler: new ServiceBusHandler().HandleMessage,
    queueNameSend: KeyVaultService.GetSecret("ServiceBusQueueSendName"),
    queueNameReceive: KeyVaultService.GetSecret("ServiceBusQueueReceiveName"),
    queueNamespace: KeyVaultService.GetSecret("ServiceBusNamespace"),
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

_ = new TimerService(pollingDelayMs: Timeout.Infinite,
                        inputHandler: new TimerHandler(serviceBusClient).HandleMessage,
                        cancellationToken);

// EDIT ABOVE THIS LINE
// *****************************************************************************************************************************************************

app.Run();

// NO NOT REMOVE - This is needed for the test project
public partial class Program { }