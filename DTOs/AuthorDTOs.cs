using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs
{
    public class AuthorDTOs
    {
        public class AuthorResponseDTO
        {
            public int AuthorId { get; set; }
            public string Name { get; set; }
            public string Biography { get; set; }
        }

        public class AuthorCreateDTO
        {
            [Required]
            public string Name { get; set; }
            public string Biography { get; set; }
        }
    }
}
