using System.Collections.Generic;

namespace Moeen.Shared.Responses.ContentSharing
{
    public class SearchContentResponse
    {
        public List<PostDto> Posts { get; set; } = new List<PostDto>();
    }
}