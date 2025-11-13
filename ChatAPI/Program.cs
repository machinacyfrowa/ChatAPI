using ChatAPI;
using Isopoh.Cryptography.Argon2;
using System.Web;

var builder = WebApplication.CreateBuilder(args);
//dodaj baze danych do aplikacji
builder.Services.AddDbContext<Database>();


var app = builder.Build();
app.UseBearerMiddleware(); //to jest jeden sposób - ten bardziej poprawny
//app.UseMiddleware<BearerMiddleware>(); //to jest drugi sposób - oba dzia³aj¹
//ustaw adresy ip i porty
app.Urls.Add("http://0.0.0.0:5000");
app.UseRouting();

//TODO: do wywalenia po zintegrowaniu z baz¹ danych
//ChatHistory chatHistory = new ChatHistory();
//chatHistory.AddMessage(new ChatMessage("User", "Hello!"));
//chatHistory.AddMessage(new ChatMessage("Bot", "Hi there! How can I assist you today?"));
//chatHistory.AddMessage(new ChatMessage("User", "Can you tell me a joke?"));

//s³ownik tokenów zalogowanych u¿ytkowników
//Dictionary<string, User> userTokens = new Dictionary<string, User>();

app.MapGet("/", () => "Hello World!");

//logowanie u¿ytkownika
app.MapPost("/user/me", (Database db, User user) =>
{
    //wbrew definicji otrzymany User.PasswordHash zawiera
    //w rzeczywistoœci has³o w postaci zwyk³ego tekstu
    User? existingUser = db.Users.FirstOrDefault(u => u.Email == user.Email);
    //je¿eli u¿ytkownik nie istnieje to zwróæ b³¹d 401
    if (existingUser == null)
    {
        return Results.Unauthorized();
    }
    //sprawdzamy has³o - podajemy do funkcji weryfikuj¹cej
    //hash z bazy i has³o podane przez u¿ytkownika
    //verify zwraca true je¿eli has³o siê zgadza
    bool passwordOK = Argon2.Verify(existingUser.PasswordHash, user.PasswordHash);
    //jeœli has³o siê nie zgadza to zwróæ b³¹d 401
    if (!passwordOK)
    {
        return Results.Unauthorized();
    }
    //wygeneruj nowy token dla u¿ytkownika
    string token = Guid.NewGuid().ToString();
    //zapisz token w s³owniku
    //userTokens.Add(token, existingUser);
    Tokens.userTokens.Add(token, existingUser);
    //userTokens[token] = existingUser;
    //zwróæ token
    return Results.Ok(token);
});
//rejestracja u¿ytkownika
app.MapPost("/users", (Database db, User user) =>
{
    user.PasswordHash = Argon2.Hash(user.PasswordHash);
    db.Users.Add(user);
    db.SaveChanges();
    return Results.Created($"{user.Id}", user);
});
//pobranie historii czatu
app.MapGet("/chat/messages", (Database db, string minimalDate) =>
{
    ChatHistory chatHistory = new ChatHistory(db);
    //tutaj zwracamy historiê czatu
    //ustawiamy defaultowy czas ostatniej wiadomoœci na teraz
    //nie jest to zbyt logiczne ale potrzebujemy pocz¹tkowej wartoœci
    DateTime parsedTimestamp = DateTime.Now;
    //timestamp jest w postaci stringa "2025-10-13T13:08:22.1712280+02:00"
    //parsujemy go do formatu obiektu datetime
    DateTime.TryParse(minimalDate, out parsedTimestamp);
    //odpytujemy nasz¹ klasê o wiadomoœci po danym timestampie i wysy³amy
    return chatHistory.GetMessagesAfter(parsedTimestamp);
});
//dodanie wiadomoœci do czatu
app.MapPost("/chat/messages", (Database db, ChatMessage message) =>
{
    if(string.IsNullOrEmpty(message.Content) || string.IsNullOrEmpty(message.Author))
    {
        return Results.BadRequest("Author and Content fields are required.");
    }
    ChatHistory chatHistory = new ChatHistory(db);
    chatHistory.AddMessage(message);
    return Results.Created($"/chat/messages/{message.Timestamp.Ticks}", message);
});
app.Run();
