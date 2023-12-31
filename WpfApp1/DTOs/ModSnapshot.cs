using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class ModSnapshot
{
    public DateTime DateTaken { get; set; }
    public List<FormSnapshot> Snapshots { get; set; } = new();
}
