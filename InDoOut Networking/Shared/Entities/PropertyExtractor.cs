﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InDoOut_Networking.Shared.Entities
{
    internal class PropertyExtractor<StorageType, ReadType> where StorageType : class where ReadType : class
    {
        private IEnumerable<PropertyInfo> _cachedProperties = null;

        public StorageType StorageObject { get; set; } = null;

        public PropertyExtractor(StorageType storageObject)
        {
            StorageObject = storageObject;

            CacheExtractAttributes(storageObject);
        }

        public bool ExtractFrom(ReadType toExtractFrom)
        {
            var extractedAll = true;

            if (toExtractFrom != null)
            {
                foreach (var cachedProperty in _cachedProperties)
                {
                    var attribute = cachedProperty.GetCustomAttribute<ExtractPropertyAttribute>();
                    if (attribute != null)
                    {
                        var property = toExtractFrom.GetType().GetProperty(attribute.FromPropertyName);
                        if (property != null && property.PropertyType == cachedProperty.PropertyType)
                        {
                            var propertyValue = property.GetValue(toExtractFrom);

                            cachedProperty.SetValue(StorageObject, propertyValue);
                        }
                        else if (!attribute.IgnoreMissingProperty)
                        {
                            extractedAll = false;
                        }
                    }
                }
            }

            return extractedAll;
        }

        public bool ApplyTo(ReadType toWriteTo)
        {
            var appliedAll = true;

            if (toWriteTo != null)
            {
                foreach (var cachedProperty in _cachedProperties)
                {
                    var attribute = cachedProperty.GetCustomAttribute<ExtractPropertyAttribute>();
                    if (attribute != null)
                    {
                        var property = typeof(ReadType).GetProperty(attribute.ToPropertyName);
                        if (property != null && property.PropertyType == cachedProperty.PropertyType)
                        {
                            var propertyValue = cachedProperty.GetValue(StorageObject);
                            var setMethod = property.GetSetMethod(false) ?? property.GetSetMethod(true);

                            if (setMethod != null)
                            {
                                _ = setMethod.Invoke(toWriteTo, new[] { propertyValue });
                            }
                            else if (!attribute.IgnoreMissingProperty)
                            {
                                appliedAll = false;
                            }
                        }
                    }
                }
            }

            return appliedAll;
        }

        private void CacheExtractAttributes(StorageType storage)
        {
            if (storage != null)
            {
                var type = storage.GetType();

                _cachedProperties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(property => property.GetCustomAttribute<ExtractPropertyAttribute>() != null);
            }
        }
    }
}
