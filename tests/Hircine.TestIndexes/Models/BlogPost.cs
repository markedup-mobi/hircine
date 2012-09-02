using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hircine.TestIndexes.Models
{
    public class BlogPost
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Author Author { get; set; }
        public DateTimeOffset TimePosted { get; set; }
        public DateTimeOffset LastEdit { get; set; }
        public IList<BlogComment> Comments { get; set; }
    }
}
