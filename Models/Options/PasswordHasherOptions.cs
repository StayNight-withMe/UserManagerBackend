namespace WebApplication1.Models.Options
{
    public class PasswordHasherOptions
    {
        public int SaltSize { get; set; } = 16;
        public int HashSize { get; set; } = 32;
        public int Parallelism { get; set; } = 8;
        public int MemorySize { get; set; } = 65536;
        public int Iterations { get; set; } = 4;
    }
}

