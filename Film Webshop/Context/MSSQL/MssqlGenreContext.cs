using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Film_Webshop.Models;

namespace Film_Webshop.Context.MSSQL
{
    public class MssqlGenreContext : Database.Database, IGenreContext
    {
        public int GetGenreId(string genre)
        {
            int genreId = 0;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT * FROM dbo.Genre WHERE Naam = @genre";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@genre", genre);
                cmd.ExecuteNonQuery();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        genreId = reader.GetInt32(reader.GetOrdinal("ID"));
                    }
                }
            }
            return genreId;
        }

        public List<int> SelectByGenreId(int id)
        {
            List<int> idList = new List<int>();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT * FROM dbo.FilmGenre WHERE Genre_ID = @genreID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@genreID", id);
                cmd.ExecuteNonQuery();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        idList.Add(reader.GetInt32(reader.GetOrdinal("Film_ID")));
                    }
                }
            }
            return idList;
        }

        public List<Genre> SelectByFilmId(int id)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query =
                    "SELECT * FROM dbo.FilmGenre INNER JOIN dbo.Genre ON dbo.FilmGenre.Genre_ID = dbo.Genre.ID WHERE Film_ID = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("ID", id);
                SqlDataReader reader = cmd.ExecuteReader();
                List<Genre> genres = new List<Genre>();

                while (reader.Read())
                {
                    Genre fg = new Genre(reader.GetString(reader.GetOrdinal("Naam")));
                    genres.Add(fg);
                }
                conn.Close();
                return genres;
            }
        }

        public List<Genre> SelectAll()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT * FROM dbo.Genre";
                SqlCommand cmd = new SqlCommand(query, conn);
                List<Genre> genres = new List<Genre>();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Genre fg = new Genre(reader.GetString(reader.GetOrdinal("Naam")));
                    genres.Add(fg);
                }
                conn.Close();
                return genres;
            }
        }

        public void Insert(int filmId, int genreId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "INSERT INTO dbo.FilmGenre (Film_ID, Genre_ID) VALUES (@Film_ID, @Genre_ID)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Genre_ID", genreId);
                cmd.Parameters.AddWithValue("@Film_ID", filmId);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public void Delete(Film film)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "DELETE FROM dbo.FilmGenre WHERE Film_ID = @filmID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@filmID", film.Id);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}