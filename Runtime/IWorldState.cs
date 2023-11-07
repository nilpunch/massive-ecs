using System;

namespace Massive
{
    public interface IWorldState
    {
        void SaveFrame();
        void Rollback(int frames);
    }
}
