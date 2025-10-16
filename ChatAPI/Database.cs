using Microsoft.EntityFrameworkCore;

namespace ChatAPI
{
    public class Database : DbContext
    {
        //zestaw danych - obiekty klasy ChatMessage
        public DbSet<ChatMessage> Messages { get; set; }
        //przeciążamy metodę konfigurującą bazę danych aby ustawić ją na SQLite
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //ustawiamy bazę danych na SQLite
            optionsBuilder.UseSqlite("Data Source=chat.db");
        }
        //przeciążamy metodę konfigurującą model danych
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //zapisuj wiadomości w tabeli o nazwie "Messages"
            modelBuilder.Entity<ChatMessage>().ToTable("Messages");
            //ustal klucz główny na pole Timestamp
            modelBuilder.Entity<ChatMessage>().HasKey(m => m.Timestamp);
            //ustalamy wymagane pola
            modelBuilder.Entity<ChatMessage>().Property(m => m.Author).IsRequired();
            modelBuilder.Entity<ChatMessage>().Property(m => m.Content).IsRequired();
        }
    }
