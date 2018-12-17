using SP2016.Repository.Attributes;
using SP2016.Repository.Entities;
using SP2016.Repository.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace SP2016.Repository
{ 
    public abstract class Repository<TContext, TEntity> where TEntity : IEntity
    {
        /// <summary>
        /// Add a new entity
        /// </summary>
        /// <param name="entity">Entity to be added</param>
        public abstract void Add(TContext context, TEntity entity);

        /// <summary>
        /// Update an existing entity
        /// </summary>
        /// <param name="entity">Entity to be updated</param>
        public abstract void Update(TContext context, TEntity entity);

        /// <summary>
        /// Get item by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        public abstract TEntity GetEntityById(TContext context, int id);

        /// <summary>
        /// Get collection of all existing entities
        /// </summary>
        /// <param name="recursive">Include entities from sub-repositories</param>
        public abstract TEntity[] GetAllEntities(TContext context);

        /// <summary>
        /// Delete an entity
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        public abstract void Delete(TContext context, TEntity entity);

        protected IEnumerable<FieldToPropertyMapping> GetFieldMappings()
        {
            var properties = typeof(TEntity).GetProperties();

            foreach (var property in properties)
            {
                object[] spFieldMappingAttributes = property
                    .GetCustomAttributes(typeof(FieldMappingAttribute), false);

                FieldMappingAttribute mapping = spFieldMappingAttributes
                    .Cast<FieldMappingAttribute>()
                    .FirstOrDefault();

                if (mapping is null) continue;

                yield return new FieldToPropertyMapping(
                    mapping.FieldName, property, mapping.Converter, mapping.IsReadOnly);
            }
        }
    }
}
