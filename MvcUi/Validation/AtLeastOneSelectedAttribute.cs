using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MvcUi.Validation
{
    public class AtLeastOneSelectedAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string[] _propertyNames;

        // Pass the property names you want to validate
        public AtLeastOneSelectedAttribute(params string[] propertyNames)
        {
            _propertyNames = propertyNames;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var properties = validationContext.ObjectType.GetProperties()
                .Where(p => _propertyNames.Contains(p.Name) && p.PropertyType == typeof(bool));

            bool atLeastOneSelected = properties.Any(p => (bool)p.GetValue(validationContext.ObjectInstance));

            if (!atLeastOneSelected)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }

        // Enable client-side validation
        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-atleastone", ErrorMessage);
        }
    }

}
