using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using StudentCollab.Models;

namespace StudentCollab.Dal
{
    public class FilesDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<FilesDal>(null);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Files>().ToTable("filesTable");
        }

        public DbSet<Files> Institutions { get; set; }
    }
}