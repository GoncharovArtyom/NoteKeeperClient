using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteKeeperClient.RestClient
{
    public class RestClientException:Exception
    {
        public RestClientException(string message) : base(message) { }
        public RestClientException(string message, Exception innerException) : base(message, innerException) { }
    }
}
