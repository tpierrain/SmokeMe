using System;

namespace SmokeMe
{
    /// <summary>
    /// Tell SmokeMe lib to ignore a Smoke Test (i.e.: a <see cref="ICheckSmoke"/> type).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IgnoreAttribute : Attribute
    {
    }
}