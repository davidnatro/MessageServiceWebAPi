using System.ComponentModel.DataAnnotations;

namespace MessageService.Models
{
    /// <summary>
    /// Модель письма.
    /// </summary>
    public class Mail
    {
        public string Subject { get; set; }

        public string Message { get; set; }

        [Required]
        public string SenderId { get; set; }

        [Required]
        public string ReceiverId { get; set; }
    }
}