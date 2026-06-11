using System;

namespace EmberToolkit.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
    public class DoNotRegisterService : Attribute
    {
    }
}
