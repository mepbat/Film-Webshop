using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Film_Webshop.Models;
using Film_Webshop.Repository;

namespace Film_Webshop.Context.MSSQL
{
    public class MssqlWinkelmandContext : Database.Database, IWinkelmandContext
    {
        public List<Film> GetFilmsInWinkelmand(int winkelmandId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                GenreRepository genreRepository = new GenreRepository(new MssqlGenreContext());
                conn.Open();
                string query = "SELECT * FROM dbo.WinkelmandFilm INNER JOIN dbo.Film ON dbo.WinkelmandFilm.Film_ID = dbo.Film.ID WHERE Winkelmand_ID = @winkelmandId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@winkelmandId", winkelmandId);
                SqlDataReader reader = cmd.ExecuteReader();
                List<Film> listFilm = new List<Film>();
                while (reader.Read())
                {
                    int id = reader.GetInt32(reader.GetOrdinal("Film_ID"));
                    string naam = reader.GetString(reader.GetOrdinal("naam"));
                    string beschrijving = reader.GetString(reader.GetOrdinal("beschrijving"));
                    int lengte = reader.GetInt32(reader.GetOrdinal("lengte"));
                    int prijs = reader.GetInt32(reader.GetOrdinal("prijs"));
                    double rating = (double)reader.GetDecimal(reader.GetOrdinal("rating"));
                    byte[] image;
                    Stream s = reader.GetStream(reader.GetOrdinal("Image"));
                    using (BinaryReader br = new BinaryReader(s))
                    {
                        image = br.ReadBytes((int)s.Length);
                    }
                    int jaar = reader.GetInt32(reader.GetOrdinal("jaar"));
                    List<Genre> filmgenres = genreRepository.GetFilmGenres(id);
                    Film f = new Film(id, naam, beschrijving, filmgenres, lengte, prijs, rating, image, jaar);
                    listFilm.Add(f);
                }
                conn.Close();
                return listFilm;
            }
        }

        public int GetWinkelmandId(int accId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT * FROM dbo.Winkelmand WHERE Account_ID = @accId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@accId", accId);
                SqlDataReader reader = cmd.ExecuteReader();
                int winkelmandId = 0;
                while (reader.Read())
                {
                    winkelmandId = reader.GetInt32(reader.GetOrdinal("ID"));
                }
                conn.Close();
                return winkelmandId;
            }
        }

        public void Delete(int winkelmandId, int filmId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "DELETE FROM dbo.WinkelmandFilm WHERE Film_ID = @Film_ID AND Winkelmand_ID = @Winkelmand_ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Winkelmand_ID", winkelmandId);
                cmd.Parameters.AddWithValue("@Film_ID", filmId);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public void Insert(int winkelmandId, int filmId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "INSERT INTO dbo.WinkelmandFilm (Winkelmand_ID, Film_ID) VALUES (@Winkelmand_ID, @Film_ID)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Winkelmand_ID", winkelmandId);
                cmd.Parameters.AddWithValue("@Film_ID", filmId);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}