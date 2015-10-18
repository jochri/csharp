using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseContext.Utils
{
    /// <summary>
    /// Compares two entity instances using reflection.
    /// </summary>
   public static class ObjectComparer
   {
      public static bool Compare<TEntity>(TEntity entity1, TEntity entity2) where TEntity : class
      {
         Type type = typeof(TEntity); //Get type
         
         if (entity1 == null || entity2 == null) //Return false if any of the entity instance is null
            return false;

         //Loop and get values of customer's properties
         foreach (System.Reflection.PropertyInfo property in type.GetProperties())
         {
            if (property.Name != "ExtensionData")
            {
               string Entity1Value = string.Empty;
               string Entity2Value = string.Empty;
               if (type.GetProperty(property.Name).GetValue(entity1, null) != null)
                  Entity1Value = type.GetProperty(property.Name).GetValue(entity1, null).ToString();
               if (type.GetProperty(property.Name).GetValue(entity2, null) != null)
                  Entity2Value = type.GetProperty(property.Name).GetValue(entity2, null).ToString();
               if (Entity1Value.Trim() != Entity2Value.Trim())
               {
                  return false;
               }
            }
         }
         return true;
      }
   }
}
