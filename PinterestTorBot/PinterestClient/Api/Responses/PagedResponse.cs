using System.Collections;
using System.Collections.Generic;

namespace PinterestTorBot.PinterestClient.Api.Responses
{
    public class PagedResponse<T> : IReadOnlyList<T>
    {
        private IReadOnlyList<T> Items { get; }

        public PagedResponse(IEnumerable<T> obj)
        {
            Items = new List<T>(obj);
        }

        public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => Items.Count;

        public T this[int index] => Items[index];
    }
}