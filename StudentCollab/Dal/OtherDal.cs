using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StudentCollab.Models;
using System.Data.Entity;

namespace StudentCollab.Dal
{
    public class OtherDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<OtherDal>(null);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Other>().ToTable("other");
        }

        public DbSet<Other> Others { get; set; }
    }
}