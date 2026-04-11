using System.Collections.Generic;

namespace Moeen.Api.Core.Contracts
{
    public class PagedResult<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => PageSize == 0 ? 0 : (int)System.Math.Ceiling((double)TotalCount / PageSize);
        public IReadOnlyList<T> Items { get; set; } = new List<T>();

        public bool HasPrevious => PageIndex > 0;
        public bool HasNext => PageIndex < TotalPages - 1;
    }
}