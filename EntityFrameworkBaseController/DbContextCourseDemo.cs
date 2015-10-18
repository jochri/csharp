/// Author: Ioannis Christodoulou
/// Provides the context and the unit of work for the given set of entities to query against the underying store.
using Courseware.Model;
using Courseware.Model.Dependency;
using Courseware.Model.Types;
using CoursewareContext.CourseTypes;
using CoursewareContext.Dependencies;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CoursewareContext
{
    /// <summary>
    /// Represents the unit of work for all sets of declared sets of entities to be queried against the underlying store.
    /// </summary>
    public class Context : DbContext
    {
       #region DbSets of entities
       
       public DbSet<Course> Courses { get; set; }
       //public DbSet<CourseInfo> CourseInformation { get; set; }
       public DbSet<CourseType> CourseTypes { get; set; }
       public DbSet<Assignment> Assignments { get; set; }
       public DbSet<ClassRegistration> ClassRegistrations { get; set; }       
       public DbSet<Educator> Educators { get; set; }
       public DbSet<SemesterRegistration> SemesterRegistrations { get; set; }
       public DbSet<RegistrationDetail> RegistrationDetails { get; set; }
       public DbSet<Student> Students { get; set; }
       public DbSet<Personal> Personals { get; set; }
       public DbSet<Timetable> Timetables { get; set; }
       public DbSet<TranscriptRequest> TranscriptRequests { get; set; }

       //public DbSet<Prerequisite> Prerequisites { get; set; }
       public DbSet<Dependent> Dependents { get; set; }

       #endregion

       public Context()
       {
         
       }

       #region Override DbContext

       protected override void OnModelCreating(DbModelBuilder modelBuilder)
       {

          modelBuilder.Configurations.Add(new CourseEntityConfiguration());
          modelBuilder.Configurations.Add(new CourseTypeEntityConfiguration());
          //modelBuilder.Configurations.Add(new PrerequisiteEntityConfiguration());
          modelBuilder.Configurations.Add(new DependentEntityConfiguration());
          //modelBuilder.Configurations.Add(new MixedCourseTypeConfiguration());
          //modelBuilder.Configurations.Add(new PlainCourseTypeConfiguration());
          //modelBuilder.Configurations.Add(new CourseInfoEntityConfiguration());
          modelBuilder.Configurations.Add(new TimetableEntityConfiguration());
          modelBuilder.Configurations.Add(new AssignmentEntityConfiguration());
          modelBuilder.Configurations.Add(new StudentEntityConfiguration());
          modelBuilder.Configurations.Add(new ClassRegistrationEntityConfiguration());
          modelBuilder.Configurations.Add(new SemesterRegistrationEntityConfiguration());
          modelBuilder.Configurations.Add(new RegistrationDetailEntityConfiguration());
          modelBuilder.Configurations.Add(new TranscriptRequestEntityConfiguration());
          modelBuilder.Configurations.Add(new PersonalEntityConfiguration());

          modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>(); //disable cascade on delete (m:m)      
          modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>(); //disable cascade on delete (1:m)
       }

       #endregion


    }
}
