using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using StudentCollab.Models;

namespace StudentCollab.Dal
{
    public class ManagerLogDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<ManagerLogDal>(null);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ManagerLog>().ToTable("dbManagerLog");
        }

        public DbSet<ManagerLog> Mlogs{ get; set; }
    }
}