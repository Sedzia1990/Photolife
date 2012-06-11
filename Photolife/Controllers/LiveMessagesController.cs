using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using Photolife.Models;

namespace Photolife.Controllers
{
    public class LiveMessagesController : Controller
    {
        private PhotolifeEntities db = new PhotolifeEntities();

        public JsonResult index() {
            JsonResult json = new JsonResult();
            
            do
            {
              //  json = getMessages();
                Thread.Sleep(500);
                
            }
            while (json == null);

            //json.Data = new {message = };

            return json;
        }

    //    protected JsonResult getMessages() {

         //   JsonResult json = new JsonResult();

            //IEnumerable<Message> messages = db.Message.Where(o => o.Odbiorca == User.Identity.Name);

          //  if (messages.LongCount() != 0) {
          //      foreach(Message msg in messages) {

           //         json.Data = new {message = "Nowa wiadomość od użytkownika "+msg.Nadawca, link = Url.Action("Details", "Messages", new {id = msg.MessageId }) };

          //      }
       //     }
         ////   return json;
      //  }

    }
}
