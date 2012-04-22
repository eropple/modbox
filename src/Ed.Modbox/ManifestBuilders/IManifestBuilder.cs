using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpFileSystem.FileSystems;

namespace Ed.Modbox.ManifestBuilders
{
    /// <summary>
    /// Defines an interface to objects that look within a mod's file
    /// system and generate a manifest based on its contents. (The default
    /// version searches for a "manifest.xml" file and parses it.)
    /// </summary>
    public interface IManifestBuilder
    {
        ModManifest GetManifest(ModDefinition definition, ReadOnlyFileSystem fileSystem);
    }
}
