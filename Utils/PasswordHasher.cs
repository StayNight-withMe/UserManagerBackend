using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication1.Abstractions;
using WebApplication1.Constants;
using WebApplication1.Models.Options;

namespace WebApplication1.Utils
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasherOptions _options;
        private readonly ILogger<PasswordHasher> _logger;

        public PasswordHasher(IOptionsMonitor<PasswordHasherOptions> passwordHashOptions, ILogger<PasswordHasher> logger)
        {
            _options = passwordHashOptions.CurrentValue;
            _logger = logger;
        }

        public string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(_options.SaltSize);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            using var argon2 = new Argon2id(passwordBytes)
            {
                DegreeOfParallelism = _options.Parallelism,
                MemorySize = _options.MemorySize,
                Iterations = _options.Iterations,
                Salt = salt
            };

            byte[] hash = argon2.GetBytes(_options.HashSize);

            return FormatArgon2IdHash(
               _options.MemorySize,
               _options.Iterations,
               _options.Parallelism,
               Convert.ToBase64String(salt),
               Convert.ToBase64String(hash));
        }

        public bool VerifyPassword(string password, string fullHash)
        {
            try
            {
                var parts = fullHash.Split(PasswordHasherConstants.SectionDelimiter);

                if (parts.Length != PasswordHasherConstants.ExpectedPartsCount)
                {
                    return false;
                }

                var parameters = parts[PasswordHasherConstants.ParametersPartIndex].Split(PasswordHasherConstants.ParameterDelimiter);

                int memory = int.Parse(parameters[PasswordHasherConstants.MemoryParamIndex].Split(PasswordHasherConstants.ValueDelimiter)[PasswordHasherConstants.ValuePartIndex]);
                int iterations = int.Parse(parameters[PasswordHasherConstants.IterationsParamIndex].Split(PasswordHasherConstants.ValueDelimiter)[PasswordHasherConstants.ValuePartIndex]);
                int parallelism = int.Parse(parameters[PasswordHasherConstants.ParallelismParamIndex].Split(PasswordHasherConstants.ValueDelimiter)[PasswordHasherConstants.ValuePartIndex]);

                byte[] salt = Convert.FromBase64String(parts[PasswordHasherConstants.SaltPartIndex]);
                byte[] storedHash = Convert.FromBase64String(parts[PasswordHasherConstants.HashPartIndex]);

                using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
                {
                    DegreeOfParallelism = parallelism,
                    MemorySize = memory,
                    Iterations = iterations,
                    Salt = salt
                };

                byte[] newHash = argon2.GetBytes(storedHash.Length);

                return CryptographicOperations.FixedTimeEquals(newHash, storedHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password verification");
                return false;
            }
        }
        private string FormatArgon2IdHash(int memory, int iterations, int parallelism, string saltBase64, string hashBase64)
        {
            var sb = new StringBuilder();

            sb.Append(PasswordHasherConstants.Argon2IdPrefix);
            sb.Append(PasswordHasherConstants.MemoryPrefix);
            sb.Append(memory);
            sb.Append(PasswordHasherConstants.ParameterDelimiter);
            sb.Append(PasswordHasherConstants.IterationsPrefix);
            sb.Append(iterations);
            sb.Append(PasswordHasherConstants.ParameterDelimiter);
            sb.Append(PasswordHasherConstants.ParallelismPrefix);
            sb.Append(parallelism);
            sb.Append(PasswordHasherConstants.SectionDelimiter);
            sb.Append(saltBase64);
            sb.Append(PasswordHasherConstants.SectionDelimiter);
            sb.Append(hashBase64);

            return sb.ToString();
        }
    }
}

