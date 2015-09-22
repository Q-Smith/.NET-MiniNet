using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet.Channels
{
    public class StringEncoder : IChannelEncoder<string>
    {
        private readonly Encoding encoding;

        public StringEncoder(Encoding encoding)
        {
            this.encoding = encoding;
        }

        public void Process(IBuffer buffer, string message)
        {
            // var len = Encoding.UTF8.GetByteCount(message.ToString());
            // BitConverter2.GetBytes(len, _buffer, 0);
            buffer.SetString(buffer.Size, message, encoding);
            //System.Console.WriteLine("Encoded message: " + message);
        }

        public void Reset()
        {
        }
    }
}
