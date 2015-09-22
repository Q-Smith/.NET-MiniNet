using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet.Channels
{
    public class StringDecoder : IChannelDecoder<string>
    {
        private readonly Encoding encoding;

        public StringDecoder(Encoding encoding)
        {
            this.encoding = encoding;
        }

        public string Process(IBuffer buffer)
        {
            // _msgLength = _bytesLeftForCurrentMsg = BitConverter.ToInt32(_buffer, 0);
            // Encoding.GetString(_buffer, 0, _msgLength)
            var message = buffer.GetString(0, buffer.Size, encoding);
            //System.Console.WriteLine("Decoded message: " + message);
            return message;
        }

        public void Reset()
        {
        }
    }
}
