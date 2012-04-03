using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Photolife.Models
{
    public class SampleData : DropCreateDatabaseAlways<PhotolifeEntities>
    {
        protected override void Seed(PhotolifeEntities context)
        {
            //var genres = new List<Genre>
            //{
            //    new Genre { Name = "Rock" },
            //    new Genre { Name = "BRECHCIK" }
            //};

            //var artists = new List<Artist>
            //{
            //    new Artist { Name = "Aaron Copland & London Symphony Orchestra" },
            //    new Artist { Name = "Zeca Pagodinho" }
            //};

            //new List<Album>
            //{
            //    new Album { Title = "A Copland Celebration, Vol. I", Genre = genres.Single(g => g.Name == "Classical"), Price = 8.99M, Artist = artists.Single(a => a.Name == "Aaron Copland & London Symphony Orchestra"), AlbumArtUrl = "/Content/Images/placeholder.gif" },
            //    new Album { Title = "Ao Vivo [IMPORT]", Genre = genres.Single(g => g.Name == "Latin"), Price = 8.99M, Artist = artists.Single(a => a.Name == "Zeca Pagodinho"), AlbumArtUrl = "/Content/Images/placeholder.gif" },
            //}.ForEach(a => context.Albums.Add(a));
        }
    }
}