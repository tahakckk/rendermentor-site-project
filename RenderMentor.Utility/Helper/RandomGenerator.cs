using System;

namespace RenderMentor.Utility.Helper
{
    public class RandomGenerator
    {
        public static int GenerateRandomNo(int min, int max)
        {
            Random _rdm = new Random();
            return _rdm.Next(min, max);
        }
    }
}
