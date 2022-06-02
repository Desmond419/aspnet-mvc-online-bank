using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ShareResources.UserPhoto
{
    public class UserPhoto
    {
        [NotMapped]
        public HttpPostedFileBase Photo { get; set; }
        public string PhotoFile { get; set; }
    }
}
