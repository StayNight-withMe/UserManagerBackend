using System.Text;

namespace WebApplication1.Constants
{
    public static class PasswordHasherConstants
    {
        public const string SectionDelimiter = "$";
        public const string ParameterDelimiter = ",";
        public const string ValueDelimiter = "=";
        
        public const int ExpectedPartsCount = 3;
        
        public const int ParametersPartIndex = 0;
        public const int SaltPartIndex = 1;
        public const int HashPartIndex = 2;
        
        public const int MemoryParamIndex = 0;
        public const int IterationsParamIndex = 1;
        public const int ParallelismParamIndex = 2;
        
        public const int ValuePartIndex = 1;
        
        public const string Argon2IdPrefix = ""; 
        public const string MemoryPrefix = "m=";
        public const string IterationsPrefix = "t=";
        public const string ParallelismPrefix = "p=";
    }
}

