using System;

namespace Hircine.TestIndexes.Models
{
    public class BlogComment
    {
        public string CommentId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTimeOffset TimePosted { get; set; }
        public bool IsApproved { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}