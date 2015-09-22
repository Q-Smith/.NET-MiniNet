using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet
{
    public interface IChannelDecoder<T>
    {
        T Process(IBuffer buffer);
        void Reset();
    }
}
