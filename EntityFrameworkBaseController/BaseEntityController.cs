/// Copyright (c) 2013 Ioannis Christodoulou
/// Description: Generic Base Entity Controller class to be used
/// by inherited classes with specified System.Data.Entity and System.Data.Entity.Dbcontext instance types.

using BaseContext.Exceptions;
using BaseContext.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace BaseContext
{
    /// <summary> EF generic entity controller class to provide entity CRUD methods, to find PKs and load entities with navigation property collections,
    /// find and retrieve eagerly navigation entity collections and references. The inherited class will have methods to find primary key and values, check for navigation 
    /// property dependencies and existence, retrieve navigation properties and property names of given Entity type in the given DbContext, as well as set and remove
    /// new and existing navigation properties of the loaded TEntity instance bject from the database(store).
    /// </summary>
    /// <typeparam name="TEntity">The type of the Entity instance for which to provide the controller and the methods to handle pk, CRUD operation, and navigation properties.</typeparam>
    /// <typeparam name="TContext">The DbContext type used to query from the store</typeparam>
    public abstract class BaseEntityController<TEntity, TContext>
        where TEntity : class
        where TContext : DbContext, new()
    {

        protected internal TContext GetContext()
        {
            var context = new TContext();
            return context;
        }

        #region public CRUD functions

        /// <summary>Creates an entity and saves to the underlying store
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity Create(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity", "Argument cannot be null");
            using (var context = GetContext())
            {
                var pk = GetPrimaryKeyValue<TEntity>(entity, context);
                if (pk != null && !ObjectComparer.Compare(0, pk)) throw new EntityExistsException(typeof(TEntity).Name + " id already exists : " + pk.ToString());
                context.Set<TEntity>().Add(entity);
                context.SaveChanges();
                return entity;
            }
        }

        /// <summary>Updates an entity to the underlying store
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown if the entity is null.</exception>
        /// <exception cref="BaseContext.Exceptions.EntityNotFoundException">Thrown if the entity is not found in the underlying context.</exception>
        public TEntity Update(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity", "Argument cannot be null");
            using (var context = GetContext())
            {
                var pk = GetPrimaryKeyValue<TEntity>(entity, context); //get primary key    
                if (pk == null || pk.Equals(0)) throw new EntityNotFoundException("You have attempted to update " + typeof(TEntity).Name + " with id<" + pk + "> which does not exist.");
                context.Entry<TEntity>(entity).State = EntityState.Modified; //change state and update        
                context.SaveChanges();
                return entity;
            }
        }

        /// <summary>Removes an entity from the underlying store
        /// </summary>
        /// <param name="entity">The entity to remove</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the entity is null.</exception>
        /// <exception cref="BaseContext.Exceptions.EntityNotFoundException">Thrown if the entity is not found in the underlying context.</exception>
        /// <exception cref="BaseContext.Exceptions.EntityHasNavigationDependencyException">Thrown if the entity has navigation property entity dependencies.</exception>
        public void Remove(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity", "Argument cannot be null");
            using (var context = GetContext())
            {
                var pk = GetPrimaryKeyValue<TEntity>(entity, context);
                if (pk == null)
                    throw new EntityNotFoundException("You have attempted to remove " + typeof(TEntity).Name + " with id<" + pk + "> which does not exist.");
                if (HasForeignKeys(entity, context))
                    throw new EntityHasNavigationDependencyException("Could not remove " + entity.GetType().Name + " with id<" + pk + "> because its primary key is referenced in foreing keys.");
                context.Entry(entity).State = EntityState.Deleted;
                context.Set<TEntity>().Remove(entity);
                context.SaveChanges();
            }
        }

        /// <summary>Finds an entity with the given primary key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown if the id of the entity is null.</exception>
        public TEntity Find(object id)
        {
            if (id == null) throw new ArgumentNullException("id", "Argument cannot be null");
            else
            {
                int num;
                var result = int.TryParse(id.ToString(), out num);
                if (result)
                    if (num == 0) throw new ArgumentNullException("num", "Argument cannot be zero");
            }
            using (var context = GetContext())
            {
                TEntity instance = context.Set<TEntity>().Find(id);
                return instance;
            }
        }


        /// <summary>Removes an entity  with the given id from the underlying context
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <param name="id">The primary key of the entity</param>
        /// <param name="context">The underlying System.Data.Entity.DbContext derived context of the entity</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the entity or the underlying context is null.</exception>
        /// <exception cref="BaseContext.Exceptions.EntityNotFoundException">Thrown if the entity is not found in the underlying context.</exception>
        protected void Remove<Entity>(int id, TContext context) where Entity : class
        {
            if (id == 0) throw new ArgumentNullException("id", "Argument cannot be zero");
            if (context == null) throw new ArgumentNullException("context", "Argument cannot be null");
            using (context)
            {
                var entity = context.Set<Entity>().Find(id);
                if (entity == null)
                    throw new EntityNotFoundException("You have attempted to remove " + typeof(TEntity).Name + " with id<" + id + "> which does not exist.");
                context.Entry(entity).State = EntityState.Deleted;
                context.Set<Entity>().Remove(entity);
                context.SaveChanges();
            }

        }

        #endregion

        #region protected base functions

        /// <summary>Finds an entity with the given primary key and the given navigation entity data type eagerly loaded from the store
        /// </summary>
        /// <typeparam name="TNavigationEntity">The type of the navigation entity</typeparam>
        /// <param name="id">The primary key of the underlying entity</param>
        /// <param name="navigationPropertyType">The navigation property relationship type</param>
        /// <exception cref="BaseContext.Exceptions.NavigationPropertyNameNotFoundException">Thrown if no navigation property name of the given type and entity is found.</exception>
        /// <returns></returns>
        protected TEntity FindIncluding<TNavigationEntity>(object id, NavigationPropertyType navigationPropertyType) where TNavigationEntity : class
        {
            if (id == null) throw new ArgumentNullException("id", "Argument cannot be null");
            else
            {
                int num;
                var result = int.TryParse(id.ToString(), out num);
                if (result)
                    if (num == 0) throw new ArgumentNullException("num", "Argument cannot be zero");
            }
            using (var context = GetContext())
            {
                TEntity instance = context.Set<TEntity>().Find(id); //not including any navigation property
                var navigationPropertyName = GetNavigationPropertyName<TNavigationEntity>(instance, context, navigationPropertyType);
                if (navigationPropertyName != string.Empty)//check for entity dependencies in navigationEntity collection property
                {
                    if (navigationPropertyType.Equals(NavigationPropertyType.Collection)) //COLLECTION type
                    {
                        context.Entry<TEntity>(instance).Collection<TNavigationEntity>(navigationPropertyName).Load(); //load collection               
                        List<TNavigationEntity> list = new List<TNavigationEntity>();
                        var collectionEntry = context.Entry<TEntity>(instance).Collection<TNavigationEntity>(navigationPropertyName);
                        var isLoaded = collectionEntry.IsLoaded; //is collection loaded               
                        if (isLoaded == true)
                        {
                            foreach (var e in collectionEntry.CurrentValue)
                            {
                                list.Add(e); //add navigationEntity
                            }
                            context.Entry<TEntity>(instance).Collection<TNavigationEntity>(navigationPropertyName).CurrentValue = list;//set current value(s)
                        }
                    }
                    else if (navigationPropertyType.Equals(NavigationPropertyType.NullableReference) || navigationPropertyType.Equals(NavigationPropertyType.RequiredReference))//REFERENCE type
                    {
                        context.Entry<TEntity>(instance).Reference<TNavigationEntity>(navigationPropertyName).Load();
                        TNavigationEntity navigationEntity = context.Entry<TEntity>(instance).Reference<TNavigationEntity>(navigationPropertyName).CurrentValue;
                        context.Entry<TEntity>(instance).Reference<TNavigationEntity>(navigationPropertyName).CurrentValue = navigationEntity;
                    }
                }
                else
                {
                    throw new NavigationPropertyNameNotFoundException("No navigation property of type '" + NavigationPropertyType.Collection.ToString() + "' was found and defined on " + typeof(TEntity).Name + " entity.");
                }
                return instance;
            }
        }

        /// <summary>Sets the value of the navigation property of the given entity and the given navigation relationship type
        /// </summary>
        /// <typeparam name="TNavigationEntity"></typeparam>
        /// <param name="navigationEntity"></param>
        /// <param name="entity"></param>
        /// <param name="navigationPropertyType"></param>
        protected void SetNavigationProperty<TNavigationEntity>(TNavigationEntity navigationEntity, TEntity entity, NavigationPropertyType navigationPropertyType) where TNavigationEntity : class
        {
            if (navigationEntity == null) throw new ArgumentNullException("navigationEntity", "Argument cannot be null");
            if (entity == null) throw new ArgumentNullException("entity", "Argument cannot be null");

            using (var context = GetContext())
            {
                var navigationPropertyName = GetNavigationPropertyName<TNavigationEntity>(entity, context, navigationPropertyType);
                var childPK = GetPrimaryKeyValue(navigationEntity, GetContext());//get navigationEntity's primary key
                var parentPK = GetPrimaryKeyValue(entity, GetContext());//get entity's primary key
                var count = context.Set<TNavigationEntity>().AsNoTracking<TNavigationEntity>().Count();

                if (navigationPropertyName != string.Empty)//check for entity dependencies in navigationEntity collection property
                {
                    object propertyValue = null;
                    if (navigationPropertyType.Equals(NavigationPropertyType.NullableReference) || navigationPropertyType.Equals(NavigationPropertyType.RequiredReference))
                    {
                        var currentValue = context.Entry(entity).Reference<TNavigationEntity>(navigationPropertyName).CurrentValue;
                        propertyValue = currentValue;
                        if (currentValue != navigationEntity)//no match found, can replace property with new value
                            propertyValue = null;
                    }

                    var itemFound = propertyValue;
                    if (itemFound != null)//item already exists
                    {
                        throw new EntityHasNavigationDependencyException("Could not add " + navigationEntity.GetType().Name + " with id : "
                                       + childPK + " to navigation property '" + navigationPropertyName + "' of '"
                                       + typeof(TEntity).Name + "' with id : " + parentPK + " because it already exists.");
                    }

                    // check REFERENCE type used in the relationship
                    if (navigationPropertyType.Equals(NavigationPropertyType.NullableReference) || navigationPropertyType.Equals(NavigationPropertyType.RequiredReference))
                    {

                        if (count.Equals(0) || childPK.Equals(0))
                        {
                            context.Entry(navigationEntity).State = EntityState.Added;
                        }

                        context.Entry(entity).Reference<TNavigationEntity>(navigationPropertyName).CurrentValue = navigationEntity; //SET current balue of navigation property
                    }

                    context.Set<TEntity>().Attach(entity); //attach from-member part of relationship to context
                    context.SaveChanges();//save changes
                }
                else
                    throw new NavigationPropertyNameNotFoundException("No navigation property of type '" + navigationPropertyType.ToString()
                                                + "' was found and defined on '" + typeof(TEntity).Name + "'.");
            }

        }

        /// <summary>Adds the given navigation entity to the navigation property of the source entity
        /// </summary>
        /// <typeparam name="TNavigationEntity"></typeparam>
        /// <param name="navigationEntity"></param>
        /// <param name="entity"></param>
        /// <param name="navigationPropertyType"></param>
        protected void AddToNavigationProperty<TNavigationEntity>(TNavigationEntity navigationEntity, TEntity entity, NavigationPropertyType navigationPropertyType) where TNavigationEntity : class
        {
            if (navigationEntity == null) throw new ArgumentNullException("navigationEntity", "Argument cannot be null");
            if (entity == null) throw new ArgumentNullException("entity", "Argument cannot be null");

            using (var context = GetContext())
            {
                var parentPK = GetPrimaryKeyValue(entity, GetContext());//get entity's primary key
                var childPK = GetPrimaryKeyValue(navigationEntity, GetContext()); //get navigationEntity's primary key if any
                var count = context.Set<TNavigationEntity>().AsNoTracking<TNavigationEntity>().Count(); //get customer from database if any

                var navigationPropertyName = GetNavigationPropertyName<TNavigationEntity>(entity, context, navigationPropertyType);
                if (navigationPropertyName != string.Empty)
                {
                    bool exists = this.ExistsInNavigationProperty<TNavigationEntity>(navigationEntity, entity, navigationPropertyType); //member exists
                    if (exists == true)
                    {
                        throw new NavigationPropertyExistsException("Could not add " + navigationEntity.GetType().Name + " with id : "
                           + childPK + " to navigation property '" + navigationPropertyName + "' of customer '"
                           + typeof(TEntity).Name + "' with id : " + parentPK + " because it already exists.");
                    }

                    if (navigationPropertyType.Equals(NavigationPropertyType.Collection)) //COLLECTION type
                    {
                        if (count.Equals(0) || childPK.Equals(0)) //does not exist yet
                        {
                            context.Entry(navigationEntity).State = EntityState.Added; //added state
                        }
                        else
                        {
                            context.Entry(navigationEntity).State = EntityState.Unchanged; //unchanged, already persisted                                  
                        }

                        context.Entry(entity).Collection<TNavigationEntity>(navigationPropertyName).CurrentValue.Add(navigationEntity);
                    }
                    else if (navigationPropertyType.Equals(NavigationPropertyType.NullableReference) || navigationPropertyType.Equals(NavigationPropertyType.RequiredReference))//REFERENCE type
                    {
                        if (count.Equals(0) || childPK.Equals(0))
                        {
                            context.Entry(navigationEntity).State = EntityState.Added;
                        }

                        context.Entry(entity).Reference<TNavigationEntity>(navigationPropertyName).CurrentValue = navigationEntity; //SET current balue of navigation property
                    }

                    context.Set<TEntity>().Attach(entity);//attach to context
                    context.SaveChanges();//save changes
                }
                else
                {
                    throw new NavigationPropertyNameNotFoundException("No navigation property of type '" + navigationPropertyType.ToString() + "' was found and defined on customer '" + typeof(TEntity).Name + "'.");
                }
            }
        }

        /// <summary>Removes the given navigation entity from the navigation property of the given source entity
        /// </summary>
        /// <typeparam name="TNavigationEntity"></typeparam>
        /// <param name="navigationEntity"></param>
        /// <param name="entity"></param>
        /// <param name="navigationPropertyType"></param>
        protected void RemoveFromNavigationProperty<TNavigationEntity>(TNavigationEntity navigationEntity, TEntity entity, NavigationPropertyType navigationPropertyType) where TNavigationEntity : class
        {
            if (navigationEntity == null) throw new ArgumentNullException("navigationEntity", "Argument cannot be null");
            if (entity == null) throw new ArgumentNullException("entity", "Argument cannot be null");

            using (var context = GetContext())
            {
                var navigationPropertyName = GetNavigationPropertyName<TNavigationEntity>(entity, context, navigationPropertyType);
                if (navigationPropertyName != string.Empty)
                {
                    bool IsFound = this.ExistsInNavigationProperty<TNavigationEntity>(navigationEntity, entity, navigationPropertyType);
                    if (IsFound == false)//item was not found
                    {
                        var childPK = GetPrimaryKeyValue(navigationEntity, context);//get navigationEntity's primary key
                        var parentPK = GetPrimaryKeyValue(entity, context);//get entity's primary key

                        throw new NavigationPropertyNotFoundException("Could not remove " + navigationEntity.GetType().Name
                                       + " with id<" + childPK + "> from navigation property '" + navigationPropertyName + "' of customer '"
                                       + typeof(TEntity).Name + "' with id <" + parentPK + "> because it was not found in the property.");
                    }
                    //Check navigation property type
                    if (navigationPropertyType.Equals(NavigationPropertyType.Collection))
                    {
                        context.Entry(entity).Collection<TNavigationEntity>(navigationPropertyName).CurrentValue.Remove(navigationEntity);
                    }
                    else if (navigationPropertyType.Equals(NavigationPropertyType.NullableReference) || navigationPropertyType.Equals(NavigationPropertyType.RequiredReference))
                    {
                        context.Entry(entity).Reference<TNavigationEntity>(navigationPropertyName).CurrentValue = null;
                    }

                    context.Set<TEntity>().Attach(entity); //attach from-member part of relationship to context
                    context.SaveChanges();//save changes
                }
                else
                {
                    throw new NavigationPropertyNameNotFoundException("No navigation property of type '" + navigationPropertyType.ToString()
                                                + "' was found and defined on customer '" + typeof(TEntity).Name + "'.");
                }
            }
        }

        /// <summary>Determines if the given entity has any navigation property dependencies of the given navigation entity type
        /// </summary>
        /// <typeparam name="TNavigationEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected bool HasNavigationDependency<TNavigationEntity>(TEntity entity) where TNavigationEntity : class
        {
            if (entity == null) throw new ArgumentNullException("entity", "Argument cannot be null");
            bool hasDependency = false;
            using (var context = GetContext())
            {
                context.Set(typeof(TEntity)).Attach(entity); //attach entity to context's set
                var propertyNames = GetNavigationPropertyNames<TNavigationEntity>(entity, context); //get navigation property name          

                foreach (var entry in propertyNames)
                {
                    if (entry.Value.Equals(RelationshipMultiplicity.Many)) //collection type found
                    {
                        var elementType = context.Entry(entity).Collection(entry.Key).Query().ElementType;
                        if (elementType == typeof(TNavigationEntity)) //element type matches TChild type
                        {
                            var results = context.Entry<TEntity>(entity).Collection<TNavigationEntity>(entry.Key).Query();
                            if (results.Count() != 0)
                                hasDependency = true;
                        }
                    }
                    else if (entry.Value.Equals(RelationshipMultiplicity.ZeroOrOne) || entry.Value.Equals(RelationshipMultiplicity.One)) //reference type found
                    {
                        var elementType = context.Entry(entity).Reference(entry.Key).Query().ElementType;
                        if (elementType == typeof(TNavigationEntity))//element type matches TChild type
                        {
                            var results = context.Entry(entity).Reference<TNavigationEntity>(entry.Key).Query().SingleOrDefault();
                            if (results != null)
                                hasDependency = true; // customer has dependencies
                        }
                    }
                }
            }
            return hasDependency;
        }

        /// <summary>Determines if the given navigation entity exists in the navigation property of the source entity of the given navigation relationship type
        /// </summary>
        /// <typeparam name="TNavigationEntity"></typeparam>
        /// <param name="navigationEntity"></param>
        /// <param name="entity"></param>
        /// <param name="navigationPropertyType"></param>
        /// <returns></returns>
        protected bool ExistsInNavigationProperty<TNavigationEntity>(TNavigationEntity navigationEntity, TEntity entity, NavigationPropertyType navigationPropertyType) where TNavigationEntity : class
        {
            if (navigationEntity == null) throw new ArgumentNullException("navigationEntity", "Argument cannot be null");
            if (entity == null) throw new ArgumentNullException("entity", "Argument cannot be null");

            using (var context = GetContext())
            {
                bool exists = false;
                var navigationPropertyName = GetNavigationPropertyName<TNavigationEntity>(entity, context, navigationPropertyType);
                if (navigationPropertyName != string.Empty)
                {
                    //collection type
                    if (navigationPropertyType.Equals(NavigationPropertyType.Collection))
                    {
                        var collection = context.Entry<TEntity>(entity).Collection<TNavigationEntity>(navigationPropertyName);
                        collection.Load();//load from database

                        //compare current values element against navigationEntity
                        var currentValue = collection.CurrentValue.SingleOrDefault<TNavigationEntity>(e => (ObjectComparer.Compare<TNavigationEntity>(e, navigationEntity) == true));
                        if (currentValue != null)
                            exists = true;

                    }
                    //reference type
                    else if (navigationPropertyType.Equals(NavigationPropertyType.NullableReference) || navigationPropertyType.Equals(NavigationPropertyType.RequiredReference))
                    {
                        var currentValue = context.Entry(entity).Reference<TNavigationEntity>(navigationPropertyName).CurrentValue;
                        if (currentValue == navigationEntity)
                            exists = true;
                    }
                }
                else
                    throw new NavigationPropertyNameNotFoundException("No navigation property of type '" + navigationPropertyType.ToString()
                                                + "' was found and defined on customer '" + typeof(TEntity).Name + "'.");
                return exists;
            }
        }

        #endregion

        #region protected static

        /// <summary>Gets the primary key value of the given entity. This returns the first or default primary key value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected static object GetPrimaryKeyValue<T>(T entity, TContext context) where T : class
        {
            if (context == null) throw new ArgumentNullException("context", "Argument cannot be null");
            if (entity == null) throw new ArgumentNullException("entity", "Argument cannot be null");
            //find primary key value
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;//cast to get interface methods
            var objectSet = objectContext.CreateObjectSet<T>();
            var elementType = objectSet.EntitySet.ElementType;
            var keyMembers = elementType.KeyMembers;
            var primaryKey = keyMembers.FirstOrDefault();
            var propertyInfo = typeof(T).GetProperty(primaryKey.Name);
            return propertyInfo.GetValue(entity, null);
        }

        /// <summary>Gets the collection of navigation properties of the given entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected static ICollection<NavigationProperty> GetNavigationProperties(TEntity entity, TContext context)
        {
            if (context == null) throw new ArgumentNullException("context", "Argument cannot be null");
            if (entity == null) throw new ArgumentNullException("entity", "Argument cannot be null");
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            var objectSet = objectContext.CreateObjectSet<TEntity>();
            var navigationProperties = objectSet.EntitySet.ElementType.NavigationProperties;
            return navigationProperties;
        }

        /// <summary>Gets the navigation property string name of the given type, navigation relationship type, and belonging entity within the given context
        /// </summary>
        /// <typeparam name="TNavigationEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="context"></param>
        /// <param name="navigationPropertyType"></param>
        /// <returns></returns>
        protected static string GetNavigationPropertyName<TNavigationEntity>(TEntity entity, TContext context, NavigationPropertyType navigationPropertyType) where TNavigationEntity : class
        {
            if (entity == null) throw new ArgumentNullException("entity", "Argument cannot be null");
            if (context == null) throw new ArgumentNullException("context", "Argument cannot be null");

            //set relationship multiplicity 
            var navigationTypeToString = navigationPropertyType.ToString();
            RelationshipMultiplicity multiplicity = new RelationshipMultiplicity();

            if (navigationPropertyType.Equals(NavigationPropertyType.Collection))
                multiplicity = RelationshipMultiplicity.Many;
            else if (navigationPropertyType.Equals(NavigationPropertyType.NullableReference))
                multiplicity = RelationshipMultiplicity.ZeroOrOne;
            else if (navigationPropertyType.Equals(NavigationPropertyType.RequiredReference))
                multiplicity = RelationshipMultiplicity.One;

            context.Set(typeof(TEntity)).Attach(entity); //attach entity to context's set
            var propertyName = string.Empty;
            var navigationProperties = GetNavigationProperties(entity, context).Where(e => e.ToEndMember.RelationshipMultiplicity
               .Equals(multiplicity)); //get properties matching multiplicity        

            //Dictionary of property name/multiplicity pair         
            IDictionary<String, RelationshipMultiplicity> Dictionary = new Dictionary<String, RelationshipMultiplicity>();

            foreach (var s in navigationProperties)
            {
                Dictionary.Add(s.Name, s.ToEndMember.RelationshipMultiplicity); //add property navigationEntity
            }

            foreach (var pair in Dictionary) //get property type matching TChild type 
            {
                var endMemberMultiplicity = pair.Value;  //get end-member multiplicity
                var navigationPropertyName = pair.Key;       // get property name

                if (endMemberMultiplicity.Equals(RelationshipMultiplicity.Many)) //type is collection
                {
                    var elementType = context.Entry<TEntity>(entity).Collection(navigationPropertyName).Query().ElementType;
                    if (elementType == typeof(TNavigationEntity)) //match found
                        propertyName = pair.Key; //set navigation property name
                }
                else if (endMemberMultiplicity.Equals(RelationshipMultiplicity.ZeroOrOne) || endMemberMultiplicity.Equals(RelationshipMultiplicity.One))//type is reference
                {
                    var elementType = context.Entry<TEntity>(entity).Reference(navigationPropertyName).Query().ElementType;
                    if (elementType == typeof(TNavigationEntity))
                        propertyName = pair.Key;
                }
            }
            return propertyName;
        }

        /// <summary>Gets the navigation property names of given navigation entity type for the given source entity
        /// </summary>
        /// <typeparam name="TNavigationEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="context"></param>
        /// <returns></returns>            
        protected static IDictionary<String, RelationshipMultiplicity> GetNavigationPropertyNames<TNavigationEntity>(TEntity entity, TContext context) where TNavigationEntity : class
        {
            if (entity == null) throw new ArgumentNullException("entity", "Argument cannot be null");
            if (context == null) throw new ArgumentNullException("context", "Argument cannot be null");
            context.Set(typeof(TEntity)).Attach(entity); //attach entity to context's set         
            var propertyName = string.Empty;
            var navigationProperties = GetNavigationProperties(entity, context);//Get navigation properties collection
            List<String> names = new List<String>();
            IDictionary<String, RelationshipMultiplicity> dictionary = new Dictionary<String, RelationshipMultiplicity>(); //dictionary of name,multiplicity pair
            foreach (var s in navigationProperties)
            {
                dictionary.Add(s.Name, s.ToEndMember.RelationshipMultiplicity);
            }
            return dictionary;
        }

        /// <summary>Gets the dictionary collection of all navigation property names and multiplicity information of a given entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected static IDictionary<String, RelationshipMultiplicity> GetNavigationPropertyNames(TEntity entity, TContext context)
        {
            if (entity == null) throw new ArgumentNullException("entity", "Argument cannot be null");
            if (context == null) throw new ArgumentNullException("context", "Argument cannot be null");
            context.Set(typeof(TEntity)).Attach(entity); //attach entity to context's set         
            var propertyName = string.Empty;
            var navigationProperties = GetNavigationProperties(entity, context);//Get navigation properties collection
            List<String> names = new List<String>();//names list
            IDictionary<String, RelationshipMultiplicity> dictionary = new Dictionary<String, RelationshipMultiplicity>(); //dictionary of name,multiplicity pair
            foreach (var s in navigationProperties)
            {
                dictionary.Add(s.Name, s.ToEndMember.RelationshipMultiplicity);
            }
            return dictionary;
        }

        /// <summary>Determines if an entity has foreign key dependencies in other entities
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected static bool HasForeignKeys(TEntity entity, TContext context)
        {
            if (context == null) throw new ArgumentNullException("context", "Argument cannot be null");
            if (entity == null) throw new ArgumentNullException("entity", "Argument cannot be null");
            using (context)
            {
                var propertyNames = GetNavigationPropertyNames(entity, context); //get navigation property names of entity
                foreach (var item in propertyNames)
                {
                    var propertyName = item.Key;
                    var multiplicity = item.Value;
                    object currentValue;
                    object navigationEntity = null;
                    //get navigation entity value if any
                    if (multiplicity == RelationshipMultiplicity.Many)
                    {
                        navigationEntity = context.Entry(entity).Collection(propertyName).EntityEntry.Entity;
                        currentValue = context.Entry(entity).Collection(propertyName).CurrentValue;
                    }
                    else
                    {
                        navigationEntity = context.Entry(entity).Reference(propertyName).EntityEntry.Entity;
                        currentValue = context.Entry(entity).Reference(propertyName).CurrentValue;
                    }
                    if (currentValue != null)
                        return true;
                }
            }
            return false;
        }


        /// <summary>Gets the list of the primary key members of an entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected static List<object> GetPrimaryKeyValues<T>(T entity, TContext context) where T : class
        {
            if (context == null) throw new ArgumentNullException("context", "Argument cannot be null");
            if (entity == null) throw new ArgumentNullException("entity", "Argument cannot be null");
            //find primary key values
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;//cast to get interface methods
            var objectSet = objectContext.CreateObjectSet<T>();
            var elementType = objectSet.EntitySet.ElementType;
            var keyMembers = elementType.KeyMembers;
            var propertyValues = new List<object>();
            foreach (var member in keyMembers)
            {
                var propertyInfo = typeof(T).GetProperty(member.Name);
                var propertyValue = propertyInfo.GetValue(entity, null);
                propertyValues.Add(propertyValue);
            }
            return propertyValues;
        }

        #endregion
    }
}
