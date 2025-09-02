using System.Collections.Concurrent;

namespace LibraryManagementSystem.Services
{
    public interface ITokenBlacklistService
    {
        Task<bool> IsTokenBlacklistedAsync(string token);
        Task BlacklistTokenAsync(string token, DateTime expiry);
        Task CleanExpiredTokensAsync();
    }

    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new();
        private readonly ILogger<TokenBlacklistService> _logger;

        public TokenBlacklistService(ILogger<TokenBlacklistService> logger)
        {
            _logger = logger;
            // Start background task to clean expired tokens
            Task.Run(async () => await CleanExpiredTokensPeriodically());
        }

        public Task<bool> IsTokenBlacklistedAsync(string token)
        {
            return Task.FromResult(_blacklistedTokens.ContainsKey(token));
        }

        public Task BlacklistTokenAsync(string token, DateTime expiry)
        {
            _blacklistedTokens[token] = expiry;
            _logger.LogInformation("Token blacklisted. Expires at: {Expiry}", expiry);
            return Task.CompletedTask;
        }

        public Task CleanExpiredTokensAsync()
        {
            var now = DateTime.UtcNow;
            var expiredTokens = _blacklistedTokens
                .Where(kv => kv.Value <= now)
                .Select(kv => kv.Key)
                .ToList();

            foreach (var token in expiredTokens)
            {
                _blacklistedTokens.TryRemove(token, out _);
            }

            _logger.LogInformation("Cleaned {Count} expired tokens", expiredTokens.Count);
            return Task.CompletedTask;
        }

        private async Task CleanExpiredTokensPeriodically()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromHours(1)); // Clean every hour
                await CleanExpiredTokensAsync();
            }
        }
    }
}