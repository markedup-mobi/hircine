using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hircine.TestIndexes.Models;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Hircine.TestIndexes.Indexes
{
    public class BlogPostsByAuthor : AbstractIndexCreationTask<BlogPost>
    {
        public BlogPostsByAuthor()
        {
            Map = posts => from post in posts
                           select new {post.Author.Id, post.Author.Name, post.TimePosted};

            Index(x => x.Author.Name, FieldIndexing.Default);
            Sort(x => x.TimePosted, SortOptions.Custom);
        }
    }
}
