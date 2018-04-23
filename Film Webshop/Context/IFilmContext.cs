using System.Collections.Generic;
using Film_Webshop.Models;

namespace Film_Webshop.Context
{
    public interface IFilmContext
    {
        void Insert(Film film);
        List<Film> Select();
        void Delete(Film film);
        Film GetById(int id);
        void Update(Film film);
        void BuyFilm(int filmId, int accId, int credits);
        List<Film> GetBoughtFilms(int accountId);
    }
}