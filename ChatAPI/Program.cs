using ChatAPI;
using System.Web;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
//ustaw adresy ip i porty
app.Urls.Add("http://192.168.6.25:5000");

ChatHistory chatHistory = new ChatHistory();
chatHistory.AddMessage(new ChatMessage("User", "Hello!"));
chatHistory.AddMessage(new ChatMessage("Bot", "Hi there! How can I assist you today?"));
chatHistory.AddMessage(new ChatMessage("User", "Can you tell me a joke?"));


app.MapGet("/", () => "Hello World!");


app.MapGet("/chat", (string? timestamp) =>
{
    //timestamp jest null tylko wtedy kiedy jawnie odpytamy bez niego
    //a nie na przyk�ad na skutek b��du
    if (timestamp == null)
        return chatHistory.GetLast(10);
    //tutaj zwracamy histori� czatu
    //ustawiamy defaultowy czas ostatniej wiadomo�ci na teraz
    //nie jest to zbyt logiczne ale potrzebujemy pocz�tkowej warto�ci
    DateTime parsedTimestamp = DateTime.Now;
    //timestamp jest w postaci stringa "2025-10-13T13:08:22.1712280+02:00"
    //parsujemy go do formatu obiektu datetime
    DateTime.TryParse(timestamp, out parsedTimestamp);
    //odpytujemy nasz� klas� o wiadomo�ci po danym timestampie i wysy�amy
    return chatHistory.GetMessagesAfter(parsedTimestamp);
});
app.MapPost("/chat", (ChatMessage message) =>
{
    chatHistory.AddMessage(message);
    return Results.Created($"/chat/{message.Timestamp.Ticks}", message);
});
app.Run();
