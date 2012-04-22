using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ed.Modbox.ManifestBuilders;
using Ed.Modbox.ModLocators;

namespace Ed.Modbox
{
    /// <summary>
    /// A collection of options passed into the ModContainer upon
    /// startup.
    /// </summary>
    public class ModContainerOptions
    {
        public ModContainerOptions()
        {
            this.ManifestBuilder = ManifestBuilders.ManifestBuilder.Default;
            this.AllowZipMods = true;
        }

        public IManifestBuilder ManifestBuilder { get; set; }
        public IModLocator ModLocator { get; set; }
        public Boolean AllowZipMods { get; set; }
    }
}
