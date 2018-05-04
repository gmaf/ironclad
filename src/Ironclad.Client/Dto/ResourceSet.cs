// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    using System.Collections;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a set of resources.
    /// </summary>
    /// <typeparam name="T">The type of resource.</typeparam>
    /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
#pragma warning disable CA1710
    [JsonObject]
    public class ResourceSet<T> : IEnumerable<T>
    {
        [JsonProperty]
        private readonly List<T> contents;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceSet{T}"/> class.
        /// </summary>
        public ResourceSet()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceSet{T}"/> class.
        /// </summary>
        /// <param name="start">The zero-based start ordinal of the subset.</param>
        /// <param name="totalSize">The total size of the set.</param>
        /// <param name="subset">The subset.</param>
        public ResourceSet(int start, int totalSize, IEnumerable<T> subset)
        {
            this.Start = start;
            this.TotalSize = totalSize;
            this.contents = new List<T>(subset);
        }

        /// <summary>
        /// Gets the zero-based start ordinal of the set.
        /// </summary>
        /// <value>The zero-based start ordinal of the set.</value>
        [JsonProperty]
        public int Start { get; private set; }

        /// <summary>
        /// Gets the size of the set.
        /// </summary>
        /// <value>The size of the set.</value>
        public int Size => this.contents.Count;

        /// <summary>
        /// Gets the total size of the set.
        /// </summary>
        /// <value>The total size of the set.</value>
        [JsonProperty]
        public int TotalSize { get; private set; }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator() => this.contents.GetEnumerator();

#pragma warning disable SA1600
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
