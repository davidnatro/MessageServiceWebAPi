using System.ComponentModel.DataAnnotations;

namespace MessageService.Models
{
    /// <summary>
    /// Модель пользователя.
    /// </summary>
    public class User
    {
        public string UserName { get; init; }
        
        [Required]
        public string Email { get; init; }
    }
}