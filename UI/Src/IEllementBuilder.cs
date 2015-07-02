using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrainBit.UI
{
    public interface IEllementBuilder
    {
        List<UIEllement> Build();
    }
}
