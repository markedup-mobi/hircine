using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hircine.TestIndexes.Reducers
{
    public class WordCountStats
    {
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public int TotalPosts { get; set; }
        public int TotalWordCount { get; set; }
        public double AverageWordCountPerPost { get; set; }
    }
}
