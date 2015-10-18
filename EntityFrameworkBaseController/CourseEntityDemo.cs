/// Author: Ioannis Christodoulou
/// Abstract class to represent Course base class for all inherited classes in a courseware scenario.
using Courseware.Model;
using Courseware.Model.Dependency;
using Courseware.Model.EntityNavigation;
using Courseware.Model.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Courseware.Model
{
   
   public abstract class Course : ITimetableNavigable, IAssignmentNavigable, ICourseInfoNavigable, IRegistrationDetailsNavigable, IDependentNavigable
   {
      #region Private fields

      private long _courseid { get; set; }
      private string _code { get; set; }
      private string _title { get; set; }
      private int _semester { get; set; }
      private string _category { get; set; }
      private int _credits { get; set; }
      private int _weight { get; set; }
      private bool _isDependent { get; set; }
      private List<Timetable> timetables { get; set; }
      private List<Assignment> assignments { get; set; }
      private List<RegistrationDetail> registrationDetails { get; set; }
      public CourseType CourseType { get; set; }
      public Dependent Dependent { get; set; }
      public List<Dependent> Dependents { get; set; }

      #endregion

      #region contructors

      public Course()
      {
         //_courseid = 0;
      }

      #endregion

      #region properties

      public long CourseId
      {
         get { return _courseid; }
         private set { _courseid = value; }
      }

      public string Code
      {
         get { return _code; }
         set { _code = value; }
      }

      public string Title
      {
         get { return _title; }
         set { _title = value; }
      }

      public int Semester
      {
         get { return _semester; }
         set { _semester = value; }
      }

      public string Category
      {
         get { return _category; }
         protected set { _category = value; }
      }

      public int Credits
      {
         get { return _credits; }
         set { _credits = value; }
      }

      public int Weight
      {
         get { return _weight; }
         set { _weight = value; }
      }

      public List<Timetable> Timetables
      {
         get { return timetables; }
         set { timetables = value; }
      }

      public List<Assignment> Assignments
      {
         get { return assignments; }
         set { assignments = value; }
      }

      public List<RegistrationDetail> RegistrationDetails
      {
         get { return registrationDetails; }
         set { registrationDetails = value; }
      }

      public bool IsDependent
      {
         get { return _isDependent; }
         set { _isDependent = value; }
      }


      #endregion
   }
}
