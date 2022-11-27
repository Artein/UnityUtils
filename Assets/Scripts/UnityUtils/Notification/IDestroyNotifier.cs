using System;

namespace UnityUtils.Notification
{
    public interface IDestroyNotifier<out T>
    {
        event Action<T> Destroying;
    }
}