using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ifb.httpclient
{
    public interface  IDecorator
    {
        public void GetDescription(HttpStatusCode StatusCode);

    }

    public class NonAuthoritativeInformation : IDecorator
    {
        public void GetDescription(HttpStatusCode StatusCode)
        {
            if (StatusCode == HttpStatusCode.NonAuthoritativeInformation)
            {
                throw (new System.Exception("Error Code 203. Sent Data is invalid"));
            }
        }
    }
}
