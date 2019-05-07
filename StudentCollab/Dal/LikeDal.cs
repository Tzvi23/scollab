using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using StudentCollab.Models;

namespace StudentCollab.Dal
{
    public class LikeDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<LikeDal>(null);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Like>().ToTable("dbLikes");
        }

        public DbSet<Like> Likes { get; set; }
    }
}