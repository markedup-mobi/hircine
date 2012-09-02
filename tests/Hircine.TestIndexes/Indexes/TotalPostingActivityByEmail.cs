using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hircine.TestIndexes.Models;
using Hircine.TestIndexes.Reducers;
using Raven.Client.Indexes;

namespace Hircine.TestIndexes.Indexes
{
    public class TotalPostingActivityByEmail : AbstractMultiMapIndexCreationTask<PostCountStats>
    {
        public TotalPostingActivityByEmail()
        {
            AddMap<BlogPost>(posts => from post in posts
                                      select new
                                                 {
                                                     Email = post.Author.Email,
                                                     PostCount = 1,
                                                     LastActivity = post.LastEdit
                                                 });

            AddMap<BlogPost>(posts => from post in posts
                                      from comment in post.Comments
                                      where comment.Email != null
                                      select new
                                                 {
                                                     Email = comment.Email,
                                                     PostCount = 1,
                                                     LastActivity = comment.TimePosted
                                                 });

            Reduce = results => from result in results
                                group result by result.Email
                                into g
                                select new
                                           {
                                               Email = g.Key,
                                               PostCount = g.Sum(x => x.PostCount),
                                               LastActivity = g.Max(x => x.LastActivity)
                                           };
        }
    }
}
