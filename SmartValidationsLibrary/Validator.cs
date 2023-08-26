using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SmartValidationsLibrary
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }

    public interface IValidationAttribute
    {
        ValidationResult Validate(object value);
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    sealed class EmailValidationAttribute : Attribute, IValidationAttribute
    {
        public ValidationResult Validate(object value)
        {
            return Validator.IsValidEmail(value?.ToString());
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    sealed class PhoneNumberValidationAttribute : Attribute, IValidationAttribute
    {
        private readonly Region _region;

        public PhoneNumberValidationAttribute(Region region)
        {
            _region = region;
        }

        public ValidationResult Validate(object value)
        {
            return Validator.IsValidPhoneNumber(value?.ToString(), _region);
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    sealed class CustomValidationAttribute : Attribute, IValidationAttribute
    {
        private readonly string _validationName;

        public CustomValidationAttribute(string validationName)
        {
            _validationName = validationName;
        }

        public ValidationResult Validate(object value)
        {
            return Validator.IsValidCustom(value?.ToString(), _validationName);
        }
    }
   

    public static class Validator
    {
        private static readonly Dictionary<Region, string> PhoneNumberPatterns = new Dictionary<Region, string>
        {
            { Region.USA, @"^\(?\d{3}\)?-?\d{3}-\d{4}$" },
            { Region.UK, @"^(\+44\s?7\d{3}|\(?07\d{3}\)?)\s?\d{3}\s?\d{4}$" },
            { Region.Spain, @"^(\+34|0034|34)?[\s|-]?[6|7|8|9][\s|-]?([0-9][\s|-]?){8}$" },
            { Region.Ireland, @"^08\d{8}$" }
        };

        public static readonly Dictionary<string, string> CustomValidations = new Dictionary<string, string>();

        public static ValidationResult IsValidEmail(string email)
        {
            const string EmailPattern = @"^(?=.{1,256})(?=.{1,64}@.{1,255}$)(?=.{1,64}[^@]{1,255}$)((?=.{1,64}[^\W_])(?=.{1,64}[\w\d-])(?=.{1,64}[\w\d.'-])[a-zA-Z\d][\w\d.'-]*[a-zA-Z\d]|\""[!#-[\]-~]*\"")@(?=.{1,255}[^\W_])(?=.{1,255}[\w\d-])(?=.{1,255}[\w\d.])[a-zA-Z\d][\w\d.-]*[a-zA-Z\d]\.([a-zA-Z]{2,19}|(xn--[a-zA-Z\d]+))$";


            if (Regex.IsMatch(email, EmailPattern))
                return new ValidationResult { IsValid = true };

            return new ValidationResult { IsValid = false, ErrorMessage = "Invalid email." };
        }

        public static ValidationResult IsValidPhoneNumber(string phoneNumber, Region region)
        {
            if (PhoneNumberPatterns.TryGetValue(region, out string pattern))
            {
                if (Regex.IsMatch(phoneNumber, pattern))
                    return new ValidationResult { IsValid = true };

                return new ValidationResult { IsValid = false, ErrorMessage = $"Invalid {region} phone number." };
            }

            return new ValidationResult { IsValid = false, ErrorMessage = $"Unsupported region: {region}" };
        }

        public static ValidationResult IsValidDate(string date)
        {
            if (DateTime.TryParse(date, out _))
                return new ValidationResult { IsValid = true };

            return new ValidationResult { IsValid = false, ErrorMessage = "Invalid date." };
        }

        public static ValidationResult IsValidCustom(string input, string validationName)
        {
            if (CustomValidations.TryGetValue(validationName, out string pattern))
            {
                if (Regex.IsMatch(input, pattern))
                    return new ValidationResult { IsValid = true };

                return new ValidationResult { IsValid = false, ErrorMessage = $"Invalid input for {validationName}." };
            }

            return new ValidationResult { IsValid = false, ErrorMessage = $"Custom validation {validationName} not found." };
        }

        public static void AddCustomValidation(string validationName, string pattern)
        {
            if (!CustomValidations.ContainsKey(validationName))
                CustomValidations.Add(validationName, pattern);
            else
                throw new InvalidOperationException($"Validation name {validationName} already exists.");
        }
    }

}
