using System.Data.Entity;

namespace Photolife.Models
{
    public class PhotolifeEntities : DbContext
    {
        // wrzucamy tutaj tworzone bazy danych

        public DbSet<Message> Message { get; set; }

        public DbSet<Photo> Photos { get; set; }
    }
}