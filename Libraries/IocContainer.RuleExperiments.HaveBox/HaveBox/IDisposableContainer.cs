using System;

namespace HaveBox
{
    public interface IDisposableContainer : IContainer, IDisposable
    {
        void DisposeInstance(IDisposable instance);
    }
}
