using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ed.Modbox
{
    /// <summary>
    /// High-level definition of a mod, as used in the mod list and in the
    /// mod manifest for dependencies. Translated into Mods by IModLocators.
    /// </summary>
    public class ModDefinition
    {
        public readonly String Name;
        public readonly Int32 Major;
        public readonly Int32 Minor;
        public readonly ModMatchingRule MatchingRule;

        public ModDefinition(String name, Int32 major, 
                             Int32 minor, ModMatchingRule matchingRule)
        {
            Name = name;
            Major = major;
            Minor = minor;
            MatchingRule = matchingRule;
        }
    }

    /// <summary>
    /// Definition of matching type for mod definitions.
    /// </summary>
    public enum ModMatchingRule
    {
        /// <summary>
        /// Only this exact version of the mod will work. Defined by
        /// a version string of the form "1.0".
        /// </summary>
        Exact,

        /// <summary>
        /// Any minor version of the given major version will work. Defined by
        /// a version string of the form "2.x", which will match "2.0", "2.2",
        /// and so on.
        /// </summary>
        AnyMinor,

        /// <summary>
        /// This minor version, or any later minor version of this major
        /// version, will work. Defined by a version string of the form "1.5+",
        /// which will work for "1.5", "1.7", etc.
        /// </summary>
        MinorOrGreater
    }
}
