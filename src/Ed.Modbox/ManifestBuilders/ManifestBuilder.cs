using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using SharpFileSystem;
using SharpFileSystem.FileSystems;

namespace Ed.Modbox.ManifestBuilders
{
    public class ManifestBuilder : IManifestBuilder
    {
        public static readonly ManifestBuilder Default = new ManifestBuilder();

        public readonly String ManifestFileName;

        public ManifestBuilder()
            : this("manifest.xml")
        {
        }
        public ManifestBuilder(String manifestFileName)
        {
            ManifestFileName = manifestFileName;
        }


        public ModManifest GetManifest(ModDefinition definition, ReadOnlyFileSystem fileSystem)
        {
            var manifestFile = FileSystemPath.Root.AppendFile("manifest.xml");

            if (fileSystem.Exists(manifestFile) == false)
            {
                throw new FileNotFoundException("Could not find manifest file for {0}.",
                    definition.ToString(false));
            }

            String input = fileSystem.OpenFile(manifestFile, FileAccess.Read).ReadAllText();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(input);

            XmlElement node = xmlDoc["mod"];

            if (node == null)
            {
                throw new InvalidDataException("No 'mod' element found in manifest.xml.");
            }

            return ModManifest.FromXml(node);
        }
    }
}
