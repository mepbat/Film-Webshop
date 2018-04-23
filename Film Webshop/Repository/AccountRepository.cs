using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Film_Webshop.Context;
using Film_Webshop.Context.MSSQL;
using Film_Webshop.Models;

namespace Film_Webshop.Repository
{
    public class AccountRepository
    {
        private readonly MssqlAccountContext _accountContext;

        public AccountRepository(MssqlAccountContext context)
        {
            _accountContext = context;
        }

        public bool ValidEmail(string email)
        {
            string mailregex = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            if (Regex.IsMatch(email, mailregex))
            {
                return true;
            }
            return false;
        }

        public bool ValidPassword(string password)
        {
            string passregex = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{4,16}$";
            if (Regex.IsMatch(password, passregex))
            {
                return true;
            }
            return false;
        }

        public void AddAccount(Account account)
        {
            _accountContext.Insert(account);
        }

        public Account LoginAccount(Account account)
        {
            try
            {
                Account accounts = (from acc in _accountContext.SelectAll()
                                    where acc.Email.Equals(account.Email)
                                            && acc.Wachtwoord.Equals(account.Wachtwoord)
                                    select acc).Single();
                return accounts;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public bool CheckAdminAccount(Account account)
        {
            try
            {
                Account accounts = (from acc in _accountContext.SelectAll()
                                    where acc.Email.Equals(account.Email)
                                            && acc.Wachtwoord.Equals(account.Wachtwoord)
                                    select acc).Single();
                return accounts.Admin;
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is NullReferenceException)
                {
                    return false;
                }
            }
            return false;
        }

        public Account GetAccountById(int id)
        {
            try
            {
                Account acc = (from account in _accountContext.SelectAll()
                               where account.Id.Equals(id)
                               select account).Single();
                return acc;
            }
            catch (Exception)
            {
                return new Account();
            }
        }

        public bool CheckBestaatAccount(Account acc)
        {
            try
            {
                Account Acc = (from account in _accountContext.SelectAll()
                               where acc.Email.Equals(account.Email)
                               select acc).Single();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void AddCredits(Account acc, int credits)
        {
            _accountContext.Update(acc, credits);
        }
    }
}