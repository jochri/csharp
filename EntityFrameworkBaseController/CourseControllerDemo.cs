/// Author: Ioannis Christodoulou
/// Demonstrates use of the generic base controller abstract class used in controller class in a courseware scenario for a Course type.
using BaseContext;
using BaseContext.Exceptions;
using BaseContext.Utils;
using Courseware.Model;
using Courseware.Model.EntityNavigation;
using Courseware.Model.EntityNavigation.Marker;
using Courseware.Model.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Courseware.Exceptions;

namespace Courseware.Controller
{
   /// <summary>Controls all of CRUD-related functions around a Courseware.Model.Course instance in the underlying CoursewareContext.Context instance
   /// </summary>   
   public class CourseController : BaseEntityController<Courseware.Model.Course, CoursewareContext.Context>
   {
      private DependentController dc = new DependentController();

      public CourseController() { }

      /// <summary> Finds a Courseware.Model.Course entity with the given id as primary key including loaded navigation data (i.e. collection or reference)
      /// loaded from the database of type TNavigationEntity.
      /// </summary>
      /// <typeparam name="TNavigationEntity">The entity type of data to eagerly load with the Courseware.Model.Course entity</typeparam>
      /// <param name="id">The primary key</param>
      /// <param name="navigationPropertyType">Either reference or collection navigation property type</param>
      /// <returns>The course found or null</returns>
      public Course Find<TNavigationEntity>(object id, NavigationPropertyType navigationType) where TNavigationEntity : class, ICourseNavigable
      {
         return base.FindIncluding<TNavigationEntity>(id, navigationType);
      }

      /// <summary>Sets the value of the TNavigationEntity property of the Courseware.Model.Course entity
      /// </summary>
      /// <typeparam name="TNavigationEntity">The type of navigation property entity.</typeparam>
      /// <param name="navigationEntity">The navigation entity parameter object.</param>
      /// <param name="course">The Courseware.Model.Course entity.</param>
      /// <param name="navigationPropertyType">The type of navigation property.</param>
      public void SetValueOf<TNavigationEntity>(TNavigationEntity navigationEntity, Courseware.Model.Course course, NavigationPropertyType navigationPropertyType) where TNavigationEntity : class, ICourseNavigable
      {
         base.SetNavigationProperty<TNavigationEntity>(navigationEntity, course, navigationPropertyType);
      }

      /// <summary> Adds the entity of TNavigationEntity type to the collection property of the Courseware.Model.Course entity
      /// </summary>
      /// <typeparam name="TNavigationEntity">The type of navigation entity</typeparam>
      /// <param name="navigationEntity">The TNavigationEntity to add to the collection property of Courseware.Model.Course entity</param>
      /// <param name="course">The Courseware.Model.Course entity.</param>
      /// <param name="navigationPropertyType"></param>
      /// <exception cref="NavigationPropertyNameNotFoundException">Thrown if navigation property is not found.</exception>
      /// <exception cref="NavigationPropertyExistsException">Thrown if navigation property already exists in course entity.</exception>
      public void AddTo<TNavigationEntity>(TNavigationEntity navigationEntity, Course course, NavigationPropertyType navigationPropertyType) where TNavigationEntity : class, ICourseNavigable
      {
         base.AddToNavigationProperty<TNavigationEntity>(navigationEntity, course, navigationPropertyType);
      }

      /// <summary>Removes the navigation property of given type of the Courseware.Model.Course entity
      /// </summary>
      /// <typeparam name="TNavigationEntity"></typeparam>
      /// <param name="navigationEntity"></param>
      /// <param name="course"></param>
      /// <param name="navigationPropertyType"></param>
      public void RemoveFrom<TNavigationEntity>(TNavigationEntity navigationEntity, Course course, NavigationPropertyType navigationPropertyType) where TNavigationEntity : class, ICourseNavigable
      {
         base.RemoveFromNavigationProperty<TNavigationEntity>(navigationEntity, course, navigationPropertyType);
      }

      /// <summary>Marks a Courseware.Model.Course entity as a dependent course so that it can be related to prerequisite Courseware.Model.Course entities.
      /// </summary>
      /// <param name="course"></param>
      /// <returns></returns>
      public bool MarkDependent(Course course)
      {
         if (course == null)
            throw new ArgumentNullException("course", "Argument cannot be null");

         var id = course.CourseId;
         course = Find<Dependent>(id, NavigationPropertyType.NullableReference);
         if (course != null)
         {
            course.IsDependent = true;
            Update(course);
            return true;
         }
         return false;
      }

      /// <summary>Marks a Courseware.Model.Course entity as a not dependent course so that it can not be related to prerequisite Courseware.Model.Course entities.
      /// </summary>
      /// <param name="course"></param>
      /// <returns></returns>
      public bool UnMarkDependent(Course course)
      {
         if (course == null)
            throw new ArgumentNullException("course", "Argument cannot be null");

         var id = course.CourseId;
         //course = Find<Dependent>(id, NavigationPropertyType.Collection); //load list of prerequisite courses of current course
         var entity = Find<Dependent>(id, NavigationPropertyType.NullableReference); //find dependent entity of course
         var dependent = entity.Dependent;
         if (dependent != null)
         {
            var depid = dependent.CourseId;
            dependent = dc.Find<Course>(depid, NavigationPropertyType.Collection); //eager load
            var courses = dependent.Prerequisites;
            if (courses != null)
               throw new EntityHasNavigationDependencyException("Cannot change status of course dependency with id <" + course.CourseId + "> because it is referenced as a foreign key in the Dependents table.");
         }
         entity.IsDependent = false;
         Update(entity);
         return true;         
         
      }

      /// <summary> Saves the Courseware.Model.Course object as a Courseware.Model.Dependent entity in the underlying table of the data store.
      /// </summary>
      /// <param name="course">The entity to save as a Courseware.Model.Dependent entity.</param>
      public void ToDependents(Course course)
      {
         if (course == null)
            throw new ArgumentNullException("course", "Argument cannot be null");
         if (course.IsDependent != true)
            throw new NotDependentCourseException("Course with id<"+course.CourseId+"> has not been set as a dependent course. It should be first set as dependent prior to putting to 'Dependents' table.");

         var load = Find<Dependent>(course.CourseId, NavigationPropertyType.NullableReference);
         if (load.Dependent != null)
         {
            if(load.Dependent.Course.CourseId == course.CourseId) 
               throw new DependentCourseExistsException("Course with id<" + course.CourseId + "> has already been set as a dependent course.");
         }
         Dependent dependent = new Dependent();
         SetValueOf<Dependent>(dependent,course, NavigationPropertyType.NullableReference);
      }

      /// <summary> Saves the pair of Courseware.Model.Course id objects to their reference table in the underlying reference table of the data store.
      /// </summary>
      /// <param name="prerequisite">The prerequisite Courseware.Model.Course entity</param>
      /// <param name="dependent">The dependent Courseware.Model.Course entity</param>
      public void ToPrerequisites(Course prerequisite, Course dependent)
      {
         //do some null and pre-existing checking
         if (prerequisite == null)
            throw new ArgumentNullException("course", "Argument cannot be null");
         if (dependent == null)
            throw new ArgumentNullException("dependent", "Argument cannot be null");
         if (prerequisite == dependent) 
            throw new ArgumentException("Prerequisite course with id" +"cannot be set as dependent course of itself.");
         else if (dependent.IsDependent != true) 
            throw new NotDependentCourseException("Course with id<" + prerequisite.CourseId + "> has not been set as a dependent course. It should be first set as dependent prior to putting to 'Dependents' table.");
      
         var dependentCourse = Find<Dependent>(dependent.CourseId,NavigationPropertyType.NullableReference).Dependent; //load dependent
         var prerequisites = Find<Dependent>(prerequisite.CourseId, NavigationPropertyType.Collection); //load prerequisites
         if (prerequisites != null && dependentCourse != null)
         {
            var exists = prerequisites.Dependents.Exists(c => c.Prerequisites.Exists(t => t.CourseId == prerequisite.CourseId));   
            if(exists)
               throw new PrerequisiteCourseExistsException("The pair of prerequisite course id <" + prerequisite.CourseId + "> and dependent course id <" + dependent.CourseId
                  +"> is already defined in the store.");

            AddTo<Dependent>(dependentCourse, prerequisite, NavigationPropertyType.Collection);
         }

      }

   }
}
