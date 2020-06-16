using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NekoPlayer.Networking
{
    public interface IResult
    {
        T ToResult<T>();
        object ToResult();
    }
}
