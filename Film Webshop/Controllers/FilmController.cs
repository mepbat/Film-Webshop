using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Film_Webshop.Context.MSSQL;
using Film_Webshop.Helpers;
using Film_Webshop.Models;
using Film_Webshop.Repository;
using Film_Webshop.Viewmodels;

namespace Film_Webshop.Controllers
{
    public class FilmController : Controller
    {
        private readonly FilmRepository _filmRepository = new FilmRepository(new MssqlFilmContext());
        private readonly GenreRepository _genreRepository = new GenreRepository(new MssqlGenreContext());
        private readonly AccountRepository _accountRepository = new AccountRepository(new MssqlAccountContext());
        private readonly WinkelmandRepository _winkelmandRepository = new WinkelmandRepository(new MssqlWinkelmandContext());

        [HttpGet]
        public ActionResult Index()
        {
            TicketAuthenticator auth = new TicketAuthenticator();
            List<Film> films = new List<Film>();
            Account acc = _accountRepository.GetAccountById(auth.Decrypt());
            acc.Winkelmand = new Winkelmand();
            acc.Winkelmand.Films = _winkelmandRepository.GetFilmsInWinkelmand(_winkelmandRepository.GetWinkelmandId(acc.Id)); ;
            FilmIndexViewmodel viewmodel = new FilmIndexViewmodel
            {
                ListFilm = _filmRepository.GetAllFilms(),
                ListGenres = _genreRepository.GetAllGenres(),
                Account = acc
            };
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Index(string gekozenGenre)
        {
            TicketAuthenticator auth = new TicketAuthenticator();
            Account acc = _accountRepository.GetAccountById(auth.Decrypt());
            acc.Winkelmand = new Winkelmand
            {
                Films = _winkelmandRepository.GetFilmsInWinkelmand(_winkelmandRepository.GetWinkelmandId(acc.Id))
            };
            FilmIndexViewmodel viewmodel = new FilmIndexViewmodel
            {
                ListGenres = _genreRepository.GetAllGenres(),
                ListFilm = new List<Film>(),
                Account = acc
            };
            List<int> filmIds = _genreRepository.GetFilmsWithGenre(gekozenGenre);
            foreach (int id in filmIds)
            {
                viewmodel.ListFilm.Add(_filmRepository.GetById(id));
            }
            return View(viewmodel);
        }

        [HttpGet]
        public ActionResult Details(int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            Film film = _filmRepository.SelectFilm(id);
            film.GenresString = "";
            foreach (Genre genre in film.ListGenres)
            {
                film.GenresString += genre.ToString() + " ";
            }
            film.Time = TimeSpan.FromMinutes(film.Lengte);
            return View(film);
        }

        [HttpGet]
        public ActionResult Toevoegen()
        {
            ToevoegenViewmodel viewmodel = new ToevoegenViewmodel
            {
                Film = new Film(),
                Genres = _genreRepository.GetAllGenres()
            };
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Toevoegen(ToevoegenViewmodel viewmodel, HttpPostedFileBase postedFile, bool Actie = false, bool Avontuur = false, bool Drama = false, bool Fantasie = false, bool Horror = false, bool Comedie = false, bool Misdaad = false, bool Oorlog = false, bool ScienceFiction = false, bool Sport = false, bool Thriller = false, bool Western = false, bool Romantiek = false)
        {
            viewmodel.Genres = _genreRepository.GetAllGenres();
            viewmodel.Film.ListGenres = _filmRepository.CheckGenres(Actie, Avontuur, Drama, Fantasie, Horror, Comedie, Misdaad, Oorlog, ScienceFiction, Sport, Thriller, Western, Romantiek);
            if (postedFile == null)
            {
                ViewBag.image = "Upload astublieft een foto die bij de film hoort.";
                return View(viewmodel);
            }
            if (viewmodel.Film.Naam == null || viewmodel.Film.Beschrijving == null || viewmodel.Film.Jaar == 0 || viewmodel.Film.Lengte == 0 || viewmodel.Film.Prijs == 0 || viewmodel.Film.ListGenres.Count == 0)
            {
                ViewBag.gegevens = "Vul astublieft alle gegevens van de film in.";
                return View(viewmodel);
            }
            if (viewmodel.Film.Rating == 0)
            {
                ViewBag.rating = "Let op! Gebruik een punt en geen komma.";
                return View(viewmodel);
            }
            WebImage img = new WebImage(postedFile.InputStream);
            img.Resize(124, 186, false);
            viewmodel.Film.Image = img.GetBytes();
            if (_filmRepository.GetAllFilms().Contains(viewmodel.Film))
            {
                return View();
            }
            _filmRepository.InsertFilm(viewmodel.Film);
            viewmodel.Film.Id = _filmRepository.GetAllFilms().Single(x => x.Beschrijving == viewmodel.Film.Beschrijving).Id;
            _genreRepository.InsertFilmGenres(viewmodel.Film);
            return View("Toegevoegd");
        }

        [HttpGet]
        public ActionResult Toegevoegd()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Film film = _filmRepository.SelectFilm(id);
            ToevoegenViewmodel vm = new ToevoegenViewmodel();
            vm.Film = film;
            vm.Genres = _genreRepository.GetAllGenres();
            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit(ToevoegenViewmodel viewmodel, HttpPostedFileBase postedFile, bool Actie = false, bool Avontuur = false, bool Drama = false, bool Fantasie = false, bool Horror = false, bool Comedie = false, bool Misdaad = false, bool Oorlog = false, bool ScienceFiction = false, bool Sport = false, bool Thriller = false, bool Western = false, bool Romantiek = false)
        {
            viewmodel.Genres = _genreRepository.GetAllGenres();
            viewmodel.Film.ListGenres = _filmRepository.CheckGenres(Actie, Avontuur, Drama, Fantasie, Horror, Comedie, Misdaad, Oorlog, ScienceFiction, Sport, Thriller, Western, Romantiek);
            if (postedFile == null)
            {
                ViewBag.image = "Upload astublieft een foto die bij de film hoort.";
                return View(viewmodel);
            }
            if (viewmodel.Film.Naam == null || viewmodel.Film.Beschrijving == null || viewmodel.Film.Jaar == 0 || viewmodel.Film.Lengte == 0 || viewmodel.Film.Prijs == 0 || viewmodel.Film.ListGenres.Count == 0)
            {
                ViewBag.gegevens = "Vul astublieft alle gegevens van de film in.";
                return View(viewmodel);
            }
            if (viewmodel.Film.Rating == 0)
            {
                ViewBag.rating = "Let op! Gebruik een punt en geen komma.";
                return View(viewmodel);
            }
            WebImage img = new WebImage(postedFile.InputStream);
            img.Resize(124, 186, false);
            viewmodel.Film.Image = img.GetBytes();
            if (_filmRepository.GetAllFilms().Contains(viewmodel.Film))
            {
                return View();
            }
            _filmRepository.EditFilm(viewmodel.Film);
            _genreRepository.DeleteFilmGenres(viewmodel.Film);
            _genreRepository.InsertFilmGenres(viewmodel.Film);
            return View("Toegevoegd");
        }

    }
}