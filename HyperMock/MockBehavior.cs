using System;

namespace HyperMock
{
    public enum MockBehavior
    {
        /// <summary>
        /// The undefined method calls will return the default value of the return type.
        /// </summary>
        Loose,

        /// <summary>
        /// The undefined method calls will raise an <see cref="InvalidOperationException"/>.
        /// </summary>
        Strict
    }
}
