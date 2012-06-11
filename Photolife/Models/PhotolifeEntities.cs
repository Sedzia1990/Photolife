using System.Data.Entity;

namespace Photolife.Models
{
    public class PhotolifeEntities : DbContext
    {

        public PhotolifeEntities() {

             System.Data.Entity.Database.SetInitializer(new Photolife.Models.DataInitalizer());
        }
        // wrzucamy tutaj tworzone bazy danych

        public DbSet<Message> Message { get; set; }

        public DbSet<Photo> Photos { get; set; }

        public DbSet<PhotoVote> PhotoVotes { get; set; }

        public DbSet<Friendship> Friendships { get; set; }

        public DbSet<UserData> UserDatas { get; set; }
    }
}