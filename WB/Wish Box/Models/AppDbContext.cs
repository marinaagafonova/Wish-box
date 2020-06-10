using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wish_Box.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Wish> Wishes { get; set; }
        public DbSet<TakenWish> TakenWishes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Following> Followings { get; set; }
        public DbSet<WishRating> WishRatings { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
           // InsertDataOfCitiesCountries();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Comment>()
            .HasOne(p => p.Wish)
            .WithMany()
            .HasForeignKey("WishId")
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
            .HasOne(p => p.InReply)
            .WithMany()
            .HasForeignKey("InReplyId")
            .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Wish>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void InsertDataOfCitiesCountries()
        {
            #region insert data from json to DB
            string json = File.ReadAllText("wwwroot/Content/countries.json");
            var countries = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
            int countryId = 1;
            string connectionString = ConnectionString.Value;

            foreach (KeyValuePair<string, List<string>> country in countries)
            {
                string countryName = country.Key;
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                string check = "select count(1) from country";
                SqlCommand check_cmd = new SqlCommand(check, connection);
                object counts = check_cmd.ExecuteScalar();
                int count = Convert.ToInt32(counts);
                if (count == 155)
                    break;
                connection.Close();

                string sql = $"Insert Into country (CountryName) Values ('{countryName}')";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                string sql2 = "select top 1 * from country order by CountryId desc";
                connection.Open();

                SqlCommand command2 = new SqlCommand(sql2, connection);
                using (SqlDataReader dataReader = command2.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        countryId = Convert.ToInt32(dataReader["CountryId"]);
                    }
                }
                connection.Close();
                List<string> cities = country.Value;
                foreach (string city in cities)
                {
                    sql = $"Insert Into City (CityName, CountryId) Values ('{city}', '{countryId}')";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            #endregion
        }
    }
}
