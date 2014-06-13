using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace mpps.Filters
{
    public class P3PHeaderActionFilter : FilterAttribute, IActionFilter
    {
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> ExecuteActionFilterAsync(System.Web.Http.Controllers.HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken, Func<System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage>> continuation)
        {
            throw new NotImplementedException();
        }
    }
}