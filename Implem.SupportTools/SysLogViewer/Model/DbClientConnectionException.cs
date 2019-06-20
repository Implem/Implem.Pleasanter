using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Implem.SupportTools.SysLogViewer.Model
{
    public class DbClientConnectionException:Exception
    {
        public DbClientConnectionException(string message) : base(message)
        {
        }

        public DbClientConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DbClientConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
