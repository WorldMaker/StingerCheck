using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using StingerCheck.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace StingerCheck.Services
{
    public class TomatoClient
    {
        private static readonly Uri TomatoBaseUri = new Uri(ConfigurationManager.AppSettings["TomatoApiUrl"]);
        private const string TomatoMoviesKey = "stingercheckmovies";

        ConnectionMultiplexer connection;
        StingerContext context;

        public TomatoClient(ConnectionMultiplexer connection, StingerContext context)
        {
            this.connection = connection;
            this.context = context;
        }

        public async Task<IEnumerable<JObject>> GetCachedNowPlaying()
        {
            var db = connection.GetDatabase();
            var items = await db.ListRangeAsync(TomatoMoviesKey);
            return from item in items.Reverse()
                   select JObject.Parse(item);
        }

        public async Task<IEnumerable<JObject>> GetFreshNowPlaying()
        {
            var db = connection.GetDatabase();
            var uri = new Uri(TomatoBaseUri, string.Concat("lists/movies/in_theaters.json?page_limit=50&country=us&apikey=", ConfigurationManager.AppSettings["TomatoApiKey"]));
            var jmovies = new List<JObject>();
            using (var client = new HttpClient())
            {
                while (uri != null)
                {
                    var result = await client.GetAsync(uri);
                    result.EnsureSuccessStatusCode();
                    var smovie = await result.Content.ReadAsStringAsync();
                    var jmovie = JObject.Parse(smovie);
                    await db.ListLeftPushAsync(TomatoMoviesKey, smovie);
                    jmovies.Add(jmovie);

                    var next = (string)jmovie.SelectToken("links.next");
                    if (next != null)
                    {
                        uri = new Uri(string.Concat(next, "&apikey=", ConfigurationManager.AppSettings["TomatoApiKey"]));
                    }
                }
            }

            await db.KeyExpireAsync(TomatoMoviesKey, TimeSpan.FromDays(3));

            return jmovies;
        }

        public async Task<Movie> GetCachedMovie(string tomatoId)
        {
            var jmovies = await GetCachedNowPlaying();
            if (!jmovies.Any())
            {
                return null;
            }

            var movie = jmovies.SelectMany(jm => jm.SelectToken("movies")).Where(jm => string.Equals((string)jm["id"], tomatoId)).FirstOrDefault();
            if (movie == null)
            {
                return null;
            }

            return new Movie
            {
                Title = (string)movie["title"],
                TomatoId = tomatoId,
                TomatoUrl = (string)movie.SelectToken("links.alternate"),
            };
        }

        public async Task<IEnumerable<Movie>> GetNowPlaying()
        {
            var jmovies = await GetCachedNowPlaying();
            if (!jmovies.Any())
            {
                jmovies = await GetFreshNowPlaying();
            }

            var movies = jmovies.SelectMany(jm => jm.SelectToken("movies"))
                .GroupBy(jm => (string)jm["id"])
                .Select(g => g.First()); // Dupe problem?
            var tomatoids = movies.Select(m => (string)m["id"]);
            var existing = context.Movies.Where(m => tomatoids.Contains(m.TomatoId)).ToDictionary(m => m.TomatoId);
            return from movie in movies
                   let tomatoid = (string)movie["id"]
                   select existing.ContainsKey(tomatoid) ? existing[tomatoid]
                        : new Movie
                        {
                            Title = (string)movie["title"],
                            TomatoId = tomatoid,
                            TomatoUrl = (string)movie.SelectToken("links.alternate"),
                        };
        }
    }
}