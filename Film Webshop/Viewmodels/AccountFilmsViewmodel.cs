using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Film_Webshop.Models;

namespace Film_Webshop.Viewmodels
{
    public class AccountFilmsViewmodel
    {
        public Account Account { get; set; }
        public List<Genre> Genres { get; set; }
    }
}