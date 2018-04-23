using System.Collections.Generic;
using Film_Webshop.Models;

namespace Film_Webshop.Context
{
    public interface IAccountContext
    {
        void Insert(Account account);
        List<Account> SelectAll();
        void Update(Account account, int credits);
    }
}