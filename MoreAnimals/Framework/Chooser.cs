using System;
using System.Collections.Generic;
using System.Linq;

namespace Entoarox.MorePetsAndAnimals.Framework
{
    /// <summary>Handles choosing from a set of available values.</summary>
    internal class Chooser
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The underlying random number generator.</summary>
        public Random Random { get; private set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public Chooser()
        {
            this.Random = new Random();
        }

        /// <summary>Reset the chooser.</summary>
        /// <param name="seed">A number used to calculate a starting value for the pseudo-random number sequence.</param>
        public void Reset(int seed)
        {
            this.Random = new Random(seed);
        }

        /// <summary>Randomly choose one of the available options.</summary>
        /// <typeparam name="T">The option type.</typeparam>
        /// <param name="options">The available options.</param>
        public T Choose<T>(T[] options)
        {
            return options[this.Random.Next(options.Length)];
        }

        /// <summary>Randomly choose one of the available options.</summary>
        /// <typeparam name="T">The option type.</typeparam>
        /// <param name="options">The available options.</param>
        /// <param name="distribution">The distribution of previous values.</param>
        public T Choose<T>(T[] options, IDictionary<T, int> distribution)
        {
            // filter to least common values
            int minCount = distribution.Values.Min();
            options = options
                .Where(opt => distribution.TryGetValue(opt, out int count) && count == minCount)
                .ToArray();

            // choose
            return this.Choose(options);
        }

        /// <summary>Randomly choose one of the available options.</summary>
        /// <typeparam name="T">The option type.</typeparam>
        /// <param name="options">The available options.</param>
        /// <param name="previous">The previous values.</param>
        public T Choose<T>(T[] options, Func<IEnumerable<T>> previous)
        {
            // get distribution
            IDictionary<T, int> distribution = options.ToDictionary(p => p, p => 0);
            foreach (T value in previous())
            {
                if (distribution.ContainsKey(value))
                    distribution[value]++;
            }

            // choose
            return this.Choose(options, distribution);
        }
    }
}
