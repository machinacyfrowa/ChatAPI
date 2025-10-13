using ChatAPI;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

ChatHistory chatHistory = new ChatHistory();
chatHistory.AddMessage(new ChatMessage("User", "Hello!"));
chatHistory.AddMessage(new ChatMessage("Bot", "Hi there! How can I assist you today?"));
chatHistory.AddMessage(new ChatMessage("User", "Can you tell me a joke?"));


app.MapGet("/", () => "Hello World!");


app.MapGet("/chat", (DateTime? timestamp) =>
{
    if (timestamp == null)
        return chatHistory.GetLast(10);
    //tutaj zwracamy historiê czatu
    return chatHistory.GetMessagesAfter(timestamp ?? DateTime.Now);
});
app.MapPost("/chat", (ChatMessage message) =>
{
    chatHistory.AddMessage(message);
    return Results.Created($"/chat/{message.Timestamp.Ticks}", message);
});
app.Run();
