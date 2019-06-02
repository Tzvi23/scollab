using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using StudentCollab.Models;

namespace StudentCollab.Dal
{
    public class FollowDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<FollowDal>(null);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Follow>().ToTable("dbFollow");
        }

        public DbSet<Follow> Follows { get; set; }
    }
}