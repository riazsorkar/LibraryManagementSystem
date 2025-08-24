using static LibraryManagementSystem.DTOs.RatingDTOs;

namespace LibraryManagementSystem.Repositories
{
    public interface IRatingRepository
    {
        Task<RatingResponseDTO> AddRatingAsync(CreateRatingDTO ratingDto, int userId);
        Task<bool> DeleteRatingAsync(int ratingId);
        Task<BookRatingSummaryDTO> GetBookRatingSummaryAsync(int bookId);
        Task<List<RatingResponseDTO>> GetUserRatingsAsync(int userId);
        Task<bool> HasUserRatedBookAsync(int userId, int bookId);
        Task<RatingResponseDTO?> UpdateRatingAsync(int ratingId, CreateRatingDTO ratingDto);
    }
}
