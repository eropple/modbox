using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Ed.Modbox.ModLocators
{
    /// <summary>
    /// The default ModLocator. Takes a set of paths corresponding to search directories,
    /// in ascending order of priority (i.e., first to search should come _last_), and
    /// walks over them to find the first version of any given mod that fits.
    /// </summary>
    public class ModLocator : IModLocator
    {
        protected readonly List<String> SearchPaths;
        protected readonly List<Tuple<String, ModDefinition>> SearchPathChildren; 

        public ModLocator(params String[] paths)
            : this((IList<String>)paths) { }

        public ModLocator(IList<String> paths)
        {
            if (paths == null) throw new ArgumentNullException("paths");
            foreach (String path in paths)
            {
                if (Directory.Exists(path) == false)
                {
                    throw new ArgumentException(String.Format("Search path '{0}' " +
                        "does not exist.", path));
                }
            }

            // in data files, last = highest priority, but that sucks for code.
            this.SearchPaths = new List<String>(paths);
            this.SearchPaths.Reverse();

            this.SearchPathChildren = new List<Tuple<String, ModDefinition>>();
            foreach (String path in SearchPaths)
            {
                String[] children = Directory.GetFileSystemEntries(path);

                foreach (String child in children)
                {
                    String defName = Path.GetFileNameWithoutExtension(child);

                    if (ModDefinition.MightMatch(defName))
                    {
                        try
                        {
                            ModDefinition def = ModDefinition.Parse(defName);
                            this.SearchPathChildren.Add(Tuple.Create(child, def));
                        }
                        catch (Exception e)
                        {
                            Debug.Print("Exception in trying to parse a mod definition " +
                                        "from '{0}': {1}", child, e);
                        }
                    }
                }
            }
        }

        public String Locate(ModDefinition definition, Boolean allowZipMods)
        {
            // This method isn't quite as performance-friendly as it could be, but
            // given how often it's called I think it's never going to really be the
            // bottleneck.

            Debug.Assert(definition != null, "definition != null");

            foreach (Tuple<String, ModDefinition> child in this.SearchPathChildren)
            {
                if (definition.IsSatisfiedBy(child.Item2))
                {
                    if (allowZipMods && child.Item1.EndsWith(".zip", true, CultureInfo.CurrentCulture))
                        return child.Item1;
                }
            }

            return null;
        }
    }
}
