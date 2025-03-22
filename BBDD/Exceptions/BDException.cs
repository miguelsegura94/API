using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBDD.Exceptions
{

    public class BDException : Exception
    {
        public BDException(string message) : base(message)
        {
        }

        public BDException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }

}
