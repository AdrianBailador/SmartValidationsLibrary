using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SmartValidationsLibrary
{

        public static class Validator
        {
            public class ValidationException : Exception
            {
                public ValidationException(string message) : base(message) { }
            }

            public class UnsupportedRegionException : ValidationException
            {
                public UnsupportedRegionException(string region) : base($"Unsupported region: {region}") { }
            }

            private static Dictionary<Region, string> PhoneNumberPatterns = new Dictionary<Region, string>
        {
            { Region.USA, @"^\(?\d{3}\)?-?\d{3}-\d{4}$" },
            { Region.UK, @"^(\+44\s?7\d{3}|\(?07\d{3}\)?)\s?\d{3}\s?\d{4}$" },
            { Region.Spain, @"^(\+34|0034|34)?[\s|-]?[6|7|8|9][\s|-]?([0-9][\s|-]?){8}$" },
            { Region.Ireland, @"^08\d{8}$" }
        };

            public static Dictionary<string, string> CustomValidations = new Dictionary<string, string>();

            public static bool IsValidEmail(string email)
            {
                const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(email, EmailPattern, RegexOptions.IgnoreCase))
                    throw new ValidationException("Invalid email.");
                return true;
            }

            public static bool IsValidPhoneNumber(string phoneNumber, Region region)
            {
                if (PhoneNumberPatterns.TryGetValue(region, out string pattern))
                {
                    if (!Regex.IsMatch(phoneNumber, pattern))
                        throw new ValidationException($"Invalid {region} phone number.");
                    return true;
                }
                throw new UnsupportedRegionException(region.ToString());
            }

            public static bool IsValidDate(string date)
            {
                if (!DateTime.TryParse(date, out _))
                    throw new ValidationException("Invalid date.");
                return true;
            }

            public static bool IsValidCustom(string input, string validationName)
            {
                if (CustomValidations.TryGetValue(validationName, out string pattern))
                {
                    if (!Regex.IsMatch(input, pattern))
                        throw new ValidationException($"Invalid input for {validationName}.");
                    return true;
                }
                throw new ValidationException($"Custom validation {validationName} not found.");
            }

            public static void AddCustomValidation(string validationName, string pattern)
            {
                if (!CustomValidations.ContainsKey(validationName))
                    CustomValidations.Add(validationName, pattern);
            }
        }

        [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
        sealed class EmailValidationAttribute : Attribute
        {
            public bool IsValid(string email)
            {
                return Validator.IsValidEmail(email);
            }
        }

        [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
        sealed class PhoneNumberValidationAttribute : Attribute
        {
            private readonly Region _region;

            public PhoneNumberValidationAttribute(Region region)
            {
                _region = region;
            }

            public bool IsValid(string phoneNumber)
            {
                return Validator.IsValidPhoneNumber(phoneNumber, _region);
            }
        }
        [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
        sealed class CustomValidationAttribute : Attribute
        {
            private readonly string _validationName;

            public CustomValidationAttribute(string validationName)
            {
                _validationName = validationName;
            }

            public bool IsValid(string input)
            {
                return Validator.IsValidCustom(input, _validationName);
            }

            public string ValidationName => _validationName;
        }

}
