using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedViewModelLibrary.Validation
{
    public class NoWhitespaceOnlyAttribute : ValidationAttribute, IClientModelValidator
    {
        public NoWhitespaceOnlyAttribute()
        {
            ErrorMessage = "The field cannot be empty or contain only whitespace.";
        }

        public override bool IsValid(object value)
        {
            if (value is null)
                return true; // allow null

            if (value is string str)
            {
                if (str.Length == 0) return true;              // allow empty string
                return !string.IsNullOrWhiteSpace(str);        // reject whitespace-only
            }

            return true; // allow non-string values (or handle differently if needed)
        }


        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-nowhitespaceonly", FormatErrorMessage(context.ModelMetadata.DisplayName ?? context.ModelMetadata.Name));
        }
    }
}
