using System;
using System.Collections.Generic;
using System.Text;

namespace EmberToolkit.Common.Interfaces.Unity.UI.Handlers
{
    public interface IViewPortTypeHandler
    {
        Type EnumType { get; }

        Array TypeValues { get; }

    }

    public enum textEnum { Test, TestTwo, TestThree };
}
