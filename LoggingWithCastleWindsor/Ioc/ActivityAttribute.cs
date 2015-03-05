using System;

namespace LoggingWithCastleWindsor.Ioc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ActivityAttribute : Attribute
    {
        public ActivityAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
