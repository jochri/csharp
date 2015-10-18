using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseContext.Utils
{
   /// <summary>
   /// Represents the relationship type of the navigation property of an entity
   /// </summary>
   public enum  NavigationPropertyType
   {
      Collection = RelationshipMultiplicity.Many,
      NullableReference = RelationshipMultiplicity.ZeroOrOne,
      RequiredReference = RelationshipMultiplicity.One
   }
}
