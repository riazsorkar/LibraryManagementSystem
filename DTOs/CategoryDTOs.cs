using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs
{
    public class CategoryDTOs
    {
        public class CategoryResponseDTO
        {
            public int CategoryId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public class CategoryCreateDTO
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}
