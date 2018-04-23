using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Film_Webshop.Models;
using Film_Webshop.Repository;

namespace Film_Webshop.Context.MSSQL
{
    public class MssqlFilmContext : Database.Database, IFilmContext
    {
        public void Insert(Film film)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "INSERT INTO dbo.film (Naam, Beschrijving, Lengte, Prijs, Rating, Image, Jaar) VALUES (@naam, @beschrijving, @lengte, @prijs, @rating, @image, @jaar)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@naam", film.Naam);
                cmd.Parameters.AddWithValue("@beschrijving", film.Beschrijving);
                cmd.Parameters.AddWithValue("@lengte", film.Lengte);
                cmd.Parameters.AddWithValue("@rating", film.Rating);
                cmd.Parameters.AddWithValue("@image", film.Image);
                cmd.Parameters.AddWithValue("@jaar", film.Jaar);
                cmd.Parameters.AddWithValue("@prijs", film.Prijs);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public List<Film> Select()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                GenreRepository genreRepository = new GenreRepository(new MssqlGenreContext());
                List<Film> films = new List<Film>();
                conn.Open();
                string query = "SELECT * FROM dbo.film";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(reader.GetOrdinal("ID"));
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
                    films.Add(f);
                }
                conn.Close();
                return films;
            }
        }

        public Film GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                GenreRepository genreRepository = new GenreRepository(new MssqlGenreContext());
                conn.Open();
                string query = "SELECT * FROM dbo.film WHERE ID = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("ID", id);
                SqlDataReader reader = cmd.ExecuteReader();
                Film f = new Film();
                while (reader.Read())
                {
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
                    f = new Film(id, naam, beschrijving, filmgenres, lengte, prijs, rating, image, jaar);
                }
                conn.Close();
                return f;
            }
        }

        public void Delete(Film film)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "DELETE FROM dbo.film WHERE id = (@id)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue(@"id", film.Id);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        public void Update(Film film)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "UPDATE dbo.film SET Naam = @naam, Beschrijving = @beschrijving, Lengte = @lengte, Prijs = @prijs, Rating = @rating, Image = @image, Jaar = @jaar WHERE ID = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@naam", film.Naam);
                cmd.Parameters.AddWithValue("@beschrijving", film.Beschrijving);
                cmd.Parameters.AddWithValue("@lengte", film.Lengte);
                cmd.Parameters.AddWithValue("@rating", film.Rating);
                cmd.Parameters.AddWithValue("@image", film.Image);
                cmd.Parameters.AddWithValue("@jaar", film.Jaar);
                cmd.Parameters.AddWithValue("@prijs", film.Prijs);
                cmd.Parameters.AddWithValue("@ID", film.Id);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public void BuyFilm(int filmId, int accId, int credits)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command =
                    new SqlCommand("spFilmsKopen", conn) { CommandType = CommandType.StoredProcedure })
                {
                    conn.Open();
                    command.Parameters.AddWithValue("@AccountID", accId);
                    command.Parameters.AddWithValue("@FilmID", filmId);
                    command.Parameters.AddWithValue("@Credits", credits);
                    command.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public List<Film> GetBoughtFilms(int accId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                GenreRepository genreRepository = new GenreRepository(new MssqlGenreContext());
                conn.Open();
                string query = "SELECT * FROM dbo.AccountFilm INNER JOIN dbo.Film ON dbo.AccountFilm.Film_ID = dbo.Film.ID WHERE Account_ID = @Account_ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Account_ID", accId);
                SqlDataReader reader = cmd.ExecuteReader();
                List<Film> filmList = new List<Film>();
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
                    filmList.Add(f);
                }
                conn.Close();
                return filmList;
            }
        }
    }
}