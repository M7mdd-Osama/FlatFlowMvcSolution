using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FlatFlow.PL.Helpers
{
    public static class ModelStateDictionaryExtensions
    {
        public static bool IsValidField(this ModelStateDictionary modelState, string fieldName)
        {
            return modelState[fieldName]?.Errors.Count == 0;
        }
    }
}
