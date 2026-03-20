using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GF
{
    public interface Response
    {
        long code { get; }
        bool status { get; }
        string message { get; }
    }
}