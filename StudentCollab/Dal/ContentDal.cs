using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using StudentCollab.Models;

namespace StudentCollab.Dal
{
    public class ContentDal: DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<UserDal>(null);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Content>().ToTable("dbContent");
        }

        public DbSet<Content> Contents { get; set; }
    }
}