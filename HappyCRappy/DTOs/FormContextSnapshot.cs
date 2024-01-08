using Mutagen.Bethesda.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class FormContextSnapshot
{
    public ModKey SourceModKey { get; set; }
    public string SerializationString { get; set; } = string.Empty;
    public SerializationType SerializationType { get; set; } = SerializationType.JSON;
}
