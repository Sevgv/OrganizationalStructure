using FluentValidation.Results;
using System.Text;

namespace OrganizationalStructure.Extensions;

public static class ListValidationFailureExtensions
{
    public static string GetStringErrorMessange(this List<ValidationFailure> messages)
    {
        var builder = new StringBuilder();
        foreach (var failure in messages)
            builder.Append($"Property {failure.PropertyName} failed validation. Error was: {failure.ErrorMessage}, ");
        builder.Remove(builder.Length - 2, 2).Append(".");
        return builder.ToString();
    }
}
