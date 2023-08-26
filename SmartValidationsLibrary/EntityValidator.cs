
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    namespace SmartValidationsLibrary
    {
        public static class EntityValidator
        {
            public static ValidationResult ValidateEntity<T>(T entity)
            {
                foreach (var property in typeof(T).GetProperties())
                {
                    foreach (Attribute attribute in property.GetCustomAttributes(true))
                    {
                        if (attribute is IValidationAttribute validationAttribute)
                        {
                            var result = validationAttribute.Validate(property.GetValue(entity));
                            if (!result.IsValid)
                                return result;
                        }
                    }
                }

                return new ValidationResult { IsValid = true };
            }
        }
    }