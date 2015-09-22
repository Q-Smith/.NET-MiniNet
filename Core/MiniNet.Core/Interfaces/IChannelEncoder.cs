using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet
{
    public interface IChannelEncoder<T>
    {
        void Process(IBuffer buffer, T message);
        void Reset();
    }
}
