using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using StudentCollab.Models;

namespace StudentCollab.Dal
{
    public class DepartmentDal: DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<UserDal>(null);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Department>().ToTable("dbDepartment");
        }

        public DbSet<Department> Departments { get; set; }
    }
}