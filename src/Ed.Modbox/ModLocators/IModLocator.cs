using System;

namespace Ed.Modbox.ModLocators
{
    /// <summary>
    /// Used by the ModContainer to find and prepare ModPrototypes by getting
    /// a file system path.
    /// </summary>
    public interface IModLocator
    {
        /// <summary>
        /// Finds the mod path that properly matches against the requested
        /// ModDefinition. Should return null if none is found, so checking is
        /// required.
        /// </summary>
        /// <param name="definition">The mod for which you are searching.</param>
        /// <param name="allowZipMods">If true, allows the use of ZIP archives as mods.</param>
        /// <returns>
        /// a file system path that points to the mod in question. I
        /// don't use a FileSystemPath here because of potential confusion
        /// related to mods on different drive partitions in Windows.
        /// </returns>
        String Locate(ModDefinition definition, Boolean allowZipMods);
    }
}
