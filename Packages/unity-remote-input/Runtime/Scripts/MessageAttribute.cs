using System;

namespace Kukumberman.UnityRemoteInput
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MessageAttribute : Attribute
    {
        public readonly string Id;

        public MessageAttribute(string id)
        {
            Id = id;
        }
    }
}
