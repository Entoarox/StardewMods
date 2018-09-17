using System.Collections.Generic;

namespace Entoarox.ShopExpander
{
    internal class ModConfig
    {
        public List<Reference> Objects = new List<Reference>
        {
            new Reference("Robin", 388, 111, "year=1"),
            new Reference("Robin", 390, 111, "year=1"),
            new Reference("Robin", 388, 333, "earthquake"),
            new Reference("Robin", 390, 333, "earthquake"),
            new Reference("Robin", 388, 999, "year>1"),
            new Reference("Robin", 390, 999, "year>1"),

            new Reference("Pierre", 472, 10, "year=1,spring"),
            new Reference("Pierre", 473, 10, "year=1,spring"),
            new Reference("Pierre", 474, 10, "year=1,spring"),
            new Reference("Pierre", 475, 10, "year=1,spring"),
            new Reference("Pierre", 427, 10, "year=1,spring"),
            new Reference("Pierre", 429, 10, "year=1,spring"),
            new Reference("Pierre", 477, 10, "year=1,spring"),

            new Reference("Pierre", 480, 10, "year=1,summer"),
            new Reference("Pierre", 482, 10, "year=1,summer"),
            new Reference("Pierre", 483, 10, "year=1,summer"),
            new Reference("Pierre", 484, 10, "year=1,summer"),
            new Reference("Pierre", 479, 10, "year=1,summer"),
            new Reference("Pierre", 302, 10, "year=1,summer"),
            new Reference("Pierre", 456, 10, "year=1,summer"),
            new Reference("Pierre", 455, 10, "year=1,summer"),

            new Reference("Pierre", 487, 10, "year=1,fall"),
            new Reference("Pierre", 488, 10, "year=1,fall"),
            new Reference("Pierre", 490, 10, "year=1,fall"),
            new Reference("Pierre", 299, 10, "year=1,fall"),
            new Reference("Pierre", 301, 10, "year=1,fall"),
            new Reference("Pierre", 492, 10, "year=1,fall"),
            new Reference("Pierre", 491, 10, "year=1,fall"),
            new Reference("Pierre", 493, 10, "year=1,fall"),
            new Reference("Pierre", 425, 10, "year=1,fall"),

            new Reference("Pierre", 472, 50, "year>1,spring"),
            new Reference("Pierre", 473, 50, "year>1,spring"),
            new Reference("Pierre", 474, 50, "year>1,spring"),
            new Reference("Pierre", 475, 50, "year>1,spring"),
            new Reference("Pierre", 427, 50, "year>1,spring"),
            new Reference("Pierre", 429, 50, "year>1,spring"),
            new Reference("Pierre", 477, 50, "year>1,spring"),
            new Reference("Pierre", 476, 50, "year>1,spring"),

            new Reference("Pierre", 480, 50, "year>1,summer"),
            new Reference("Pierre", 482, 50, "year>1,summer"),
            new Reference("Pierre", 483, 50, "year>1,summer"),
            new Reference("Pierre", 484, 50, "year>1,summer"),
            new Reference("Pierre", 479, 50, "year>1,summer"),
            new Reference("Pierre", 302, 50, "year>1,summer"),
            new Reference("Pierre", 456, 50, "year>1,summer"),
            new Reference("Pierre", 455, 50, "year>1,summer"),
            new Reference("Pierre", 485, 50, "year>1,summer"),

            new Reference("Pierre", 487, 50, "year>1,fall"),
            new Reference("Pierre", 488, 50, "year>1,fall"),
            new Reference("Pierre", 490, 50, "year>1,fall"),
            new Reference("Pierre", 299, 50, "year>1,fall"),
            new Reference("Pierre", 301, 50, "year>1,fall"),
            new Reference("Pierre", 492, 50, "year>1,fall"),
            new Reference("Pierre", 491, 50, "year>1,fall"),
            new Reference("Pierre", 493, 50, "year>1,fall"),
            new Reference("Pierre", 425, 50, "year>1,fall"),
            new Reference("Pierre", 489, 50, "year>1,fall")
        };
    }
}
