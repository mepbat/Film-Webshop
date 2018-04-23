using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Film_Webshop.Context.MSSQL;
using Film_Webshop.Helpers;
using Film_Webshop.Models;
using Film_Webshop.Repository;
using Film_Webshop.Viewmodels;

namespace Film_Webshop.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly AccountRepository _accountRepository = new AccountRepository(new MssqlAccountContext());
        private readonly FilmRepository _filmRepository = new FilmRepository(new MssqlFilmContext());
        private readonly GenreRepository _genreRepository = new GenreRepository(new MssqlGenreContext());

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Account acc)
        {
            if (acc.Email == null || acc.Wachtwoord == null)
            {
                ViewBag.Message1 = "Vul een email en wachtwoord in.";
                return View();
            }
            TicketAuthenticator ticket = new TicketAuthenticator();
            acc = new Account(acc.Email, PasswordManager.Hash(acc.Wachtwoord));
            acc = _accountRepository.LoginAccount(acc);
            if (acc == null)
            {
                ViewBag.Message = "Dit is geen geregistreerd account. Check of de ingevulde gegevens kloppen.";
                return View();
            }
            acc = new Account(acc.Id, acc.Credits, acc.Email, PasswordManager.Hash(acc.Wachtwoord), acc.Admin);
            HttpCookie c = ticket.Encrypt(acc.Id.ToString());
            HttpContext.Response.Cookies.Add(c);
            if (acc.Admin)
            {
                return RedirectToAction("Toevoegen", "Film");
            }
            return RedirectToAction("Index", "Film");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Account acc)
        {
            if (acc.Email == null || acc.Wachtwoord == null)
            {
                ViewBag.Message1 = "Vul een email en wachtwoord in.";
                return View();
            }

            if (acc.Wachtwoord != acc.HerhalingWachtwoord)
            {
                ViewBag.Message1 = "Herhaling wachtwoord niet gelijk. Let op spelfouten.";
                return View();
            }
            if (!_accountRepository.ValidEmail(acc.Email))
            {
                ViewBag.Message1 = "Gebruik een valide email adres.";
                return View();
            }
            if (!_accountRepository.ValidPassword(acc.Wachtwoord))
            {
                ViewBag.Message2 = "Een wachtwoord moet ten minste 1 hoofdletter, 1 kleine letter, 1 cijfer hebben.";
                return View();
            }
            acc = new Account(acc.Email, PasswordManager.Hash(acc.Wachtwoord), acc.HerhalingWachtwoord, acc.Admin);
            if (_accountRepository.CheckBestaatAccount(acc))
            {
                ViewBag.Message1 = "Account met deze email bestaat al.";
                return View();
            }
            _accountRepository.AddAccount(acc);
            return View("Login");
        }

        [HttpGet]
        public ActionResult Credits()
        {
            TicketAuthenticator auth = new TicketAuthenticator();
            Account acc = _accountRepository.GetAccountById(auth.Decrypt());
            return View(acc);
        }

        [HttpPost]
        public ActionResult Credits(Account acc)
        {
            TicketAuthenticator auth = new TicketAuthenticator();
            Account a = _accountRepository.GetAccountById(auth.Decrypt());
            _accountRepository.AddCredits(a, 10);
            a = _accountRepository.GetAccountById(auth.Decrypt());
            return View(a);
        }

        [HttpGet]
        public ActionResult Films()
        {
            TicketAuthenticator auth = new TicketAuthenticator();
            int accId = auth.Decrypt();
            Account acc = _accountRepository.GetAccountById(accId);
            acc.Films = _filmRepository.GetBoughtFilms(accId);
            AccountFilmsViewmodel viewmodel = new AccountFilmsViewmodel
            {
                Genres = _genreRepository.GetAllGenres(),
                Account = acc
            };
            return View(viewmodel);
        }
    }
}