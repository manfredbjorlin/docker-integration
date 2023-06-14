#region Default Functionality

var cancellationTokenSource = new CancellationTokenSource();
var cancellationToken = cancellationTokenSource.Token;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
Statics.Logger = app.Logger;

NGLogger.LoggingLevel = app.Environment.IsDevelopment() ? NGLogger.LogLevel.Debug : NGLogger.LogLevel.Info;
NGKeyVaultClient.ApplicationName = app.Environment.ApplicationName;

app.Lifetime.ApplicationStarted.Register(() => NGLogger.WriteInfo("Application started..."));
app.Lifetime.ApplicationStopping.Register(() => cancellationTokenSource.Cancel());
app.Lifetime.ApplicationStopped.Register(() => NGLogger.WriteInfo("Application stopped..."));

app.UseSwagger();
app.UseSwaggerUI();

var keyVaultUri = app.Configuration.GetValue<string>("VaultUri");

var serviceBusClient = new NGServiceBusClient(
    inputHandler: new ServiceBusHandler().HandleMessage,
    queueName: app.Environment.IsDevelopment() ? app.Configuration.GetValue<string>("ServiceBusQueueName")! : NGKeyVaultClient.GetSecret("ServiceBusQueueName", keyVaultUri!, app.Environment.IsDevelopment()),
    queueNamespace: app.Environment.IsDevelopment() ? app.Configuration.GetValue<string>("ServiceBusEndpoint")! : NGKeyVaultClient.GetSecret("ServiceBusNamespace", keyVaultUri!, app.Environment.IsDevelopment()),
    isDevelopment: app.Environment.IsDevelopment(),
    cancellationToken);

// Catch default 
app.MapGet("/", () => "You shouldn't be here!")
    .ExcludeFromDescription();

#endregion

// *****************************************************************************************************************************************************
// EDIT BELOW THIS LINE

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

// This is needed for the test project
public partial class Program { }