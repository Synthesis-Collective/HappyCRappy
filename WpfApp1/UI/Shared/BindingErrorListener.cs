using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HappyCRappy;

public class BindingErrorListener : TraceListener
{
    public static void Register()
    {
        PresentationTraceSources.DataBindingSource.Listeners.Add(new BindingErrorListener());
    }

    public override void Write(string? message)
    {
    }

    public override void WriteLine(string? message)
    {
#if DEBUG
        throw new System.Exception(message);
#endif
    }
}
