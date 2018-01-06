using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Language_School_Web.Models
{
    public class TokenModel
    {
        public string Access_Token { get; set; }
        public string Token_Type { get; set; }
        public int Expires_In { get; set; }

    }
}