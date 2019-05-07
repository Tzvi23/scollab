using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using StudentCollab.Models;

namespace StudentCollab.Dal
{
    public class ManageConnectionDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<ManageConnectionDal>(null);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ManageConnection>().ToTable("dbManageConnection");
        }

        public DbSet<ManageConnection> ManageConnections { get; set; }
    }
}