/// Author : Ioannis Christodoulou
/// Provides configuration for Course entity type using the Fluent API.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using Courseware.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursewareContext
{
    /// <summary>
    /// Provides configuration for given entity instance type using the Fluent API.
    /// </summary>
   internal class CourseEntityConfiguration : EntityTypeConfiguration<Course>
   {
      public CourseEntityConfiguration()
      {
         //table and pk mapping
         this.ToTable("Courses", "dbo").HasKey<long>(c => c.CourseId).Property(c => c.CourseId)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
            .HasColumnName("CourseID");

         //column names
         this.Property(c => c.Code).HasColumnName("Code").IsRequired();
         this.Property(c => c.Title).HasColumnName("Title").IsRequired();
         this.Property(c => c.Semester).HasColumnName("Semester").IsRequired();
         this.Property(c => c.Category).HasColumnName("Category").IsRequired();
         this.Property(c => c.Credits).HasColumnName("Credits").IsRequired();
         this.Property(c => c.Weight).HasColumnName("Weight").IsRequired();
         this.Property(c => c.IsDependent).HasColumnName("Is Dependent").IsRequired();

         this.HasMany(c => c.Dependents)
            .WithMany(c => c.Prerequisites)
            .Map(c => c.MapLeftKey("CourseID_FK").MapRightKey("DependentCourseID_FK")
            .ToTable("Prerequisites", "dbo"));

      }

      
   }
}
