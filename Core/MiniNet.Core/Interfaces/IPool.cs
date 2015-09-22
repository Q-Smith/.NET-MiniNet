using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet
{
    public interface IPool<T>
    {
        int Size { get; }
        T Pop();
        void Push(T item);
        void Resize(int newSize);
    }
}
