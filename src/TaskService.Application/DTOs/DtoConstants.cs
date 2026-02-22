using System;
using System.Collections.Generic;
using System.Text;

namespace TaskService.Application.DTOs
{
    public static class DtoConstants
    {
        /// <summary>
        /// UUID 7 regex pattern: 128-bit identifier
        /// </summary>
        public const string IdValidationRegex = "^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-7[0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$";

        /// <summary>
        /// Regex validating that the string does not contain any html tag
        /// </summary>
        public const string ContainsNoHtmlTagValidationRegex = "^((?![<>]).)*$";

        /// <summary>
        /// Error message returned when a string contains any html tag
        /// </summary>
        public const string ContainsHtmlTagErrorMessage = "No html tags allowed";
    }
}
