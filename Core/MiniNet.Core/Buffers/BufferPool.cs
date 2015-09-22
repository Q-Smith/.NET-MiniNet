using MiniNet.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet.Buffers
{
    public class BufferPool : IPool<IBuffer>
    {
        private byte[] data;
        private int blockSize;
        private ConcurrentStack<IBuffer> blocks;

        public int Size
        {
            get { return blocks.Count; }
        }

        public BufferPool(int blockSize, int numOfBuffers)
        {
            this.blocks = new ConcurrentStack<IBuffer>();
            this.data = new byte[blockSize * numOfBuffers];
            this.blockSize = blockSize;
            int offset = 0;
            for (int i = 0; i < numOfBuffers; i++)
            {
                this.blocks.Push(new Buffer(this.data, offset, blockSize));
                offset += this.blockSize;
            }
        }

        public IBuffer Pop()
        {
            IBuffer pop;
            if (!blocks.TryPop(out pop))
                throw new Exception("Out of buffers. You are either not releasing used buffers or have allocated fewer buffers than allowed number of connected clients.");

            return pop;
        }

        public void Push(IBuffer block)
        {
            Ensure.NotNull(block, "block");
            block.Reset();
            blocks.Push(block);
        }

        public void Resize(int newSize)
        {
            throw new NotImplementedException();
        }
    }
}
