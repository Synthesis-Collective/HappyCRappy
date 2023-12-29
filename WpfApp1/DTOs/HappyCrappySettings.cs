using Mutagen.Bethesda.Skyrim;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class HappyCrappySettings
{
    public string DataFolderPathStr { get; set; } = string.Empty;
    public SkyrimRelease SkyrimGameType { get; set; } = SkyrimRelease.SkyrimSE;
}
