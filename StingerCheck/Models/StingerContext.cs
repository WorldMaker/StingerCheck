using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace StingerCheck.Models
{
    public class StingerContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Stinger> Stingers { get; set; }

        public StingerContext() : base()
        {
            this.Configuration.LazyLoadingEnabled = false; // Turn off lazy loading by default
        }
    }
}