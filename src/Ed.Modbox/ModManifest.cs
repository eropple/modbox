using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using SharpFileSystem;

namespace Ed.Modbox
{
    /// <summary>
    /// The code representation of a mod manifest. Pulled out of a 
    /// </summary>
    public class ModManifest
    {
        public readonly String Name;
        public readonly Int32 Major;
        public readonly Int32 Minor;

        public readonly ReadOnlyCollection<ModDefinition> Dependencies;

        public ModManifest(String name, Int32 major, Int32 minor, 
                           ReadOnlyCollection<ModDefinition> dependencies)
        {
            Name = name;
            Major = major;
            Minor = minor;
            Dependencies = dependencies;
        }



        /// <summary>
        /// Parses a manifest from XML.
        /// </summary>
        /// <param name="root">The root "mod" node.</param>
        public static ModManifest FromXml(XmlElement root)
        {
            try
            {
                if (root.Name != "mod")
                {
                    throw new InvalidDataException(String.Format("Given '{0}' node, expected 'mod'.",
                        root.Name));
                }

                if (root.HasAttribute("name") == false ||
                    root.HasAttribute("version") == false)
                {
                    throw new InvalidDataException("Attributes 'name' and 'version' are required.");
                }

                String name = root.Attributes["name"].Value;
                String version = root.Attributes["version"].Value;

                ModDefinition definition = ModDefinition.Parse(name, version);
                if (definition.MatchingRule != ModMatchingRule.Exact)
                {
                    throw new InvalidDataException("The version number in a manifest must be an " +
                        "exact one; none of the form '2.x' or '1.5+' are allowed.");
                }

                Int32 major = definition.Major;
                Int32 minor = definition.Minor;

                XmlElement depsNode = root["deps"];

                List<ModDefinition> deps = new List<ModDefinition>();

                if (depsNode != null)
                {
                    foreach (XmlElement e in depsNode)
                    {
                        if (e.HasAttribute("name") == false ||
                            e.HasAttribute("version") == false)
                        {
                            throw new InvalidDataException("Attributes 'name' and 'version' are " + 
                                "required on all dependencies.");
                        }

                        deps.Add(ModDefinition.Parse(e.Attributes["name"].Value,
                                                     e.Attributes["version"].Value));
                    }
                }

                return new ModManifest(name, major, minor, 
                                       new ReadOnlyCollection<ModDefinition>(deps));
            }
            catch (Exception e)
            {
                throw new ArgumentException("There was a problem with parsing " +
                    "the provided XML node. Check the inner exception for details.", e);
            }
        }
    }
}
