namespace SmartValidationsLibrary
{
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
            public static bool ValidateEntity<T>(T entity)
            {
                foreach (var property in typeof(T).GetProperties())
                {
                    foreach (Attribute attribute in property.GetCustomAttributes(true))
                    {
                        if (attribute is EmailValidationAttribute emailAttribute && !emailAttribute.IsValid(property.GetValue(entity)?.ToString()))
                        {
                            throw new Validator.ValidationException("Invalid email in entity.");
                        }
                        else if (attribute is PhoneNumberValidationAttribute phoneAttribute && !phoneAttribute.IsValid(property.GetValue(entity)?.ToString()))
                        {
                            throw new Validator.ValidationException($"Invalid phone number in entity.");
                        }
                        else if (attribute is CustomValidationAttribute customAttribute && !customAttribute.IsValid(property.GetValue(entity)?.ToString()))
                        {
                            throw new Validator.ValidationException($"Invalid value in entity for custom validation {customAttribute.ValidationName}.");
                        }
                    }
                }
                return true;
            }
        }
    }

}