using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hircine.TestIndexes.Models;
using Hircine.TestIndexes.Reducers;
using Raven.Client.Indexes;

namespace Hircine.TestIndexes.Indexes
{
    public class WordCountPerAuthor : AbstractIndexCreationTask<BlogPost, WordCountStats>
    {
        public WordCountPerAuthor()
        {
            Map = posts => from post in posts
                           select new
                                      {
                                          AuthorId = post.Author.Id,
                                          AuthorName = post.Author.Name,
                                          TotalPosts = 1,
                                          TotalWordCount = post.Description.Split(' ').Count(),
                                          AverageWordCountPerPost = 0.0d
                                      };

            Reduce = results => from result in results
                                group result by new {result.AuthorId, result.AuthorName}
                                into g
                                select new
                                           {
                                               AuthorId = g.Key.AuthorId,
                                               AuthorName = g.Key.AuthorName,
                                               TotalPosts = g.Sum(x => x.TotalPosts),
                                               TotalWordCount = g.Sum(x => x.TotalWordCount),
                                               AverageWordCountPerPost = (g.Sum(x => x.TotalWordCount) / Math.Max(g.Sum(x => x.TotalPosts), 1))
                                           };
        }
    }
}
