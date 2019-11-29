using System.Collections.Generic;

namespace Core
{
    public static class Dictionaries
    {
        public static readonly Dictionary<byte, (byte, byte)> LessonIndexesDictionary =
            new Dictionary<byte, (byte, byte)>
            {
                {1, (08, 30)}, {2, (10, 25)}, {3, (12, 20)}, {4, (14, 15)},
                {5, (16, 10)}, {6, (18, 05)}, {7, (20, 00)}
            };
    }
}