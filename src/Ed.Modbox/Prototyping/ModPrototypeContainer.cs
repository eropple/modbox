using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Ed.Modbox.Exceptions;
using ICSharpCode.SharpZipLib.Zip;
using SharpFileSystem;
using SharpFileSystem.FileSystems;
using SharpFileSystem.SharpZipLib;
using File = SharpFileSystem.File;

namespace Ed.Modbox.Prototyping
{
    /// <summary>
    /// Analogous to the ModContainer, but for ModPrototypes. The ModPrototypeContainer
    /// builds prototypes from a list of 
    /// </summary>
    internal class ModPrototypeContainer
    {
        protected readonly IList<ModDefinition> Mods; 
        protected readonly ModContainerOptions Options;

        public readonly IList<ModPrototype> Prototypes; 

        internal ModPrototypeContainer(IList<ModDefinition> mods,
                                       ModContainerOptions options)
        {
            this.Mods = mods;
            this.Options = options;

            this.Prototypes = BuildPrototypes(mods, options);
        }

        private IList<ModPrototype> BuildPrototypes(IList<ModDefinition> defs, 
                                                    ModContainerOptions options)
        {


            List<ModPrototype> prototypes = new List<ModPrototype>(defs.Count);
            foreach (ModDefinition def in defs)
            {
                String path = Options.ModLocator.Locate(def, options.AllowZipMods);
                if (path == null)
                {
                    throw new ModNotFoundException(def);
                }

                IFileSystem fileSystem = null;

                if (path.EndsWith(".zip", true, CultureInfo.CurrentCulture))
                {
                    FileStream fs = System.IO.File.OpenRead(path);
                    fileSystem = SharpZipLibFileSystem.Open(fs);
                }
                else
                {
                }
            }
            return prototypes;
        }
    }
}
