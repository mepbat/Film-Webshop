using System.Collections.Generic;
using Film_Webshop.Context.MSSQL;
using Film_Webshop.Models;

namespace Film_Webshop.Repository
{
    public class WinkelmandRepository
    {
        private readonly MssqlWinkelmandContext _winkelmandContext;

        public WinkelmandRepository(MssqlWinkelmandContext winkelmandContext)
        {
            this._winkelmandContext = winkelmandContext;
        }

        public List<Film> GetFilmsInWinkelmand(int winkelmandId)
        {
            return _winkelmandContext.GetFilmsInWinkelmand(winkelmandId);
        }

        public void AddFilmWithId(int winkelmandId, int filmId)
        {
            _winkelmandContext.Insert(winkelmandId, filmId);
        }

        public void RemoveFilmWithId(int winkelmandId, int filmId)
        {
            _winkelmandContext.Delete(winkelmandId, filmId);
        }

        public int GetWinkelmandId(int accId)
        {
            return _winkelmandContext.GetWinkelmandId(accId);
        }

        public int GetPrijs(List<Film> filmList)
        {
            int totaalPrijs = 0;
            foreach (Film f in filmList)
            {
                totaalPrijs += f.Prijs;
            }
            return totaalPrijs;
        }
    }
}