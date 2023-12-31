using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class VM_Snapshot : VM
{
    public VM_Snapshot(ModSnapshot model)
    {
        DateTaken = model.DateTaken;
    }

    public DateTime DateTaken { get; set; }

    public string DateTakenStr => ToLabelString(DateTaken);

    public static string ToLabelString(DateTime timestamp)
    {
        return $"{timestamp:yyyy-MM-dd-HH-mm-ss}";
    }
}
