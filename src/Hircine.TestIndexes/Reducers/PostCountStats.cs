using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hircine.TestIndexes.Reducers
{
    public class PostCountStats
    {
        public string Email { get; set; }
        public int PostCount { get; set; }
        public DateTimeOffset LastActivity { get; set; }
    }
}
