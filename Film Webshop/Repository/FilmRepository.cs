using System.Collections.Generic;
using System.Linq;
using Film_Webshop.Context.MSSQL;
using Film_Webshop.Models;

namespace Film_Webshop.Repository
{
    public class FilmRepository
    {
        private readonly MssqlFilmContext _filmContext;

        public FilmRepository(MssqlFilmContext filmContext)
        {
            _filmContext = filmContext;
        }

        public List<Film> GetAllFilms()
        {
            return _filmContext.Select();
        }


        public Film SelectFilm(int id)
        {
            Film film = (from f in _filmContext.Select()
                         where f.Id.Equals(id)
                         select f).Single();
            return film;
        }

        public Film GetById(int id)
        {
            return _filmContext.GetById(id);
        }

        public void InsertFilm(Film film)
        {
            _filmContext.Insert(film);
        }

        public List<Genre> CheckGenres(bool actie, bool avontuur, bool drama, bool fantasie, bool horror, bool comedie, bool misdaad, bool oorlog, bool scienceFiction, bool sport, bool thriller, bool western, bool romantiek)
        {
            List<Genre> genres = new List<Genre>();
            if (actie)
            {
                genres.Add(new Genre("Actie"));
            }
            if (avontuur)
            {
                genres.Add(new Genre("Avontuur"));
            }
            if (drama)
            {
                genres.Add(new Genre("Drama"));
            }
            if (fantasie)
            {
                genres.Add(new Genre("Fantasie"));
            }
            if (horror)
            {
                genres.Add(new Genre("Horror"));
            }
            if (comedie)
            {
                genres.Add(new Genre("Comedie"));
            }
            if (misdaad)
            {
                genres.Add(new Genre("Misdaad"));
            }
            if (oorlog)
            {
                genres.Add(new Genre("Oorlog"));
            }
            if (scienceFiction)
            {
                genres.Add(new Genre("Sciencefiction"));
            }
            if (sport)
            {
                genres.Add(new Genre("Sport"));
            }
            if (thriller)
            {
                genres.Add(new Genre("Thriller"));
            }
            if (western)
            {
                genres.Add(new Genre("Western"));
            }
            if (romantiek)
            {
                genres.Add(new Genre("Romantiek"));
            }
            return genres;
        }

        public void EditFilm(Film film)
        {
            _filmContext.Update(film);
        }

        public void BuyFilm(Account acc, Film f)
        {
            _filmContext.BuyFilm(f.Id, acc.Id, f.Prijs);
        }

        public List<Film> GetBoughtFilms(int accId)
        {
            return _filmContext.GetBoughtFilms(accId);
        }
    }
}