using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hircine.TestIndexes.Models
{
    public class Author
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTimeOffset DateJoined { get; set; }
    }
}
