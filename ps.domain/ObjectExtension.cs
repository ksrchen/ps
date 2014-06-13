using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ps.domain
{
    public static class ObjectExtension
    {

        public static string ToJSON(this Object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

    }
}
