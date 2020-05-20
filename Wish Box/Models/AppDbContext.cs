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
    class Country
    {
        List<string> cities;
    }
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
            string json = File.ReadAllText("wwwroot/Content/countries.json");
            //var c = JsonConvert.DeserializeXmlNode(json);
            
            var countries = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
            int countryId = 1;
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=aspnet-Wish_Box-FE7D3E55-F2B7-4477-88B5-C537D05A53C6;Trusted_Connection=True;MultipleActiveResultSets=true";

            foreach (KeyValuePair<string, List<string>> country in countries)
            {
                string countryName = country.Key;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sql = $"Insert Into country (CountryName) Values ('{countryName}')";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                List<string> cities = country.Value;
                foreach(string city in cities)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string sql = $"Insert Into City (CityName, CountryId) Values ('{city}', '{countryId}')";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.CommandType = CommandType.Text;
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                }
                countryId++;
            }
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
    }
}
