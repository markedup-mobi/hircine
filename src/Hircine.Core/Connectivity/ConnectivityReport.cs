using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hircine.Core.Connectivity
{
    public class ConnectivityReport
    {
        public IList<ConnectivityResult> ConnectivityResults { get; set; }
        public int SuccessfulConnections {get { return ConnectivityResults.Count(x => x.CanConnect); }}
        public int FailedConnections { get { return ConnectivityResults.Count(x => !x.CanConnect); } }
    }

    public class ConnectivityResult
    {
        public string ConnectionString { get; set; }
        public bool CanConnect { get; set; }
    }
}
