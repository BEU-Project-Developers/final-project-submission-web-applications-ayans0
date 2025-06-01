using System;

namespace MovieManagement.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string? ErrorMessage { get; set; }
        public string? ErrorTitle { get; set; }

        public ErrorViewModel()
        {
            ErrorTitle = "Error";
            ErrorMessage = "An error occurred while processing your request. Please try again later.";
        }

        public ErrorViewModel(string message)
        {
            ErrorTitle = "Error";
            ErrorMessage = message;
        }

        public ErrorViewModel(string title, string message)
        {
            ErrorTitle = title;
            ErrorMessage = message;
        }
    }
}
