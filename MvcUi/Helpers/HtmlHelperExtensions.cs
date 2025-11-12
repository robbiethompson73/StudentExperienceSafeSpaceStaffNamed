using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Encodings.Web;
using System.ComponentModel.DataAnnotations;

namespace MvcUi.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent LabelWithRequiredIndicatorFor<TModel, TValue>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression)
        {
            // Get metadata for the property using IModelMetadataProvider
            var metadata = htmlHelper.MetadataProvider.GetMetadataForProperty(typeof(TModel), GetPropertyName(expression));

            // Render the label
            var label = htmlHelper.LabelFor(expression, new { @class = "form-label" }).ToHtmlString();

            // Check if the Required attribute is applied explicitly using reflection
            var isRequired = IsPropertyRequired(typeof(TModel), GetPropertyName(expression));

            // Append asterisk if required
            if (isRequired)
            {
                label = label.Replace("</label>", "<span class=\"text-danger\"> *</span></label>");
            }

            return new HtmlString(label);
        }

        public static IHtmlContent DisplayNameWithRequiredIndicatorFor<TModel, TValue>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression)
        {
            var metadata = htmlHelper.MetadataProvider.GetMetadataForProperty(typeof(TModel), GetPropertyName(expression));

            var displayName = metadata.DisplayName ?? metadata.PropertyName;

            // Check if the Required attribute is applied explicitly using reflection
            var isRequired = IsPropertyRequired(typeof(TModel), GetPropertyName(expression));

            var result = isRequired
                ? $"{displayName} <span class=\"text-danger\">*</span>"
                : displayName;

            return new HtmlString(result);
        }

        // Helper method to extract property name from expression
        private static string GetPropertyName<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("Expression must be a member expression", nameof(expression));
            }

            return memberExpression.Member.Name;
        }

        // Reflection-based check for the Required attribute on a property
        private static bool IsPropertyRequired(Type modelType, string propertyName)
        {
            var property = modelType.GetProperty(propertyName);
            if (property == null)
            {
                return false;
            }

            // Check if the RequiredAttribute is applied
            return property.GetCustomAttributes()
                            .Any(attr => attr is RequiredAttribute || attr.GetType().Name == "MustBeTrueAttribute");
        }

        // Helper extension to render IHtmlContent as string
        private static string ToHtmlString(this IHtmlContent content)
        {
            using var writer = new StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }
    }
}
