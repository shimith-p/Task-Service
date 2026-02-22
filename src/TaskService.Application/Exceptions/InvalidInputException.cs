using TaskService.Application.DTOs;

namespace TaskService.Application.Exceptions
{
    public class InvalidInputException : Exception, IExceptionWithErrorResponseItem
    {
        /// <inheritdoc/>
        public List<ErrorResponseItemDto> Errors { get; set; } = [];

        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidInputException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidInputException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidInputException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidInputException(List<ErrorResponseItemDto> errors)
        {
            Errors = errors;
        }
    }
}