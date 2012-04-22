using SharpFileSystem.FileSystems;

namespace Ed.Modbox.Prototyping
{
    /// <summary>
    /// The second stage of mod loading. ModDefinitions are turned into
    /// ModPrototypes, themselves with their own list of ModDefinitions
    /// within their Manifest file. Dependency resolution is handled by
    /// checking all ModPrototype files and walking them backwards until
    /// all dependencies are satisfied.
    /// </summary>
    internal class ModPrototype
    {
        public readonly ModDefinition Definition;
        public readonly ReadOnlyFileSystem FileSystem;
        public readonly ModManifest Manifest;

        public ModPrototype(ModDefinition definition, ReadOnlyFileSystem fileSystem,
                            ModManifest manifest)
        {
            Definition = definition;
            FileSystem = fileSystem;
            Manifest = manifest;
        }
    }
}
