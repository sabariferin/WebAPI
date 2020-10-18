using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMIWebAPI.DataSource
{
    public class HttpStatusException:Exception
    {
        public HttpStatusCode Status { get; private set; }

        public HttpStatusException(HttpStatusCode status, string msg) : base(msg)
        {
            Status = status;
        }
    }
}
