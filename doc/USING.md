# Using Modbox #

*Please note that these notes are in part the design/spec for modbox, in that*
*they were originally written before modbox itself was written. At first, some*
*of the details might be a little off. Feel free to file issues against the*
*project for documentation problems.*

Each **mod** is a representation of a virtual file system. The ModContainer
will search mods in a hierarchy defined either as the list of path strings
passed into the mod container or via a set of (int, mod_path_string) tuples.
ModContainer handles dependency 

## Setting up modbox ##

ModContainer is instantiated like so:

    var options = new ModContainerOptions();
    var userAppData = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "YourApplication",
        "mods"
    );
    var commonAppData = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        "YourApplication",
        "mods"
    );
    var basePath = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
        "mods"
    );

    // the below settings are the defaults, just put here for clarity
    options.ManifestBuilder = ManifestBuilder.Default;
    
    // required options
    options.ModLocator = ModLocator.FromPaths(basePath, commonAppData, userAppData);

    // get your list of mods...
    var mods = new List<ModDefinition>();
    // you could just have your base mod in whatever code you're
    // using to pick up a mod list, but I prefer making it explicit
    mods.Add(new ModDefinition("basemod", "1.0"); // lowest priority mod
    mods.AddAll(ModDefinition.Parse(File.ReadAllLines("modlist.txt"));

    return new ModContainer(mods, options);

### Package Types ###

Mod packages are provided to the container in string format. These formats
tell the container which virtual file system to use for each loaded mod. The
supported formats are defined below:

- **C:\path\to\a\modname-1.0** - defines a file system based on the given
directory. The virtual file system is rooted at this directory (therefore
protecting mods from attempting to pull data from outside of the mod
package via ".."). Uses sharpfilesystem's **SubFileSystem**.

- **C:\path\to\a\anothermod-1.5.zip** - defines a file system based on the
archive file provided. While sharpfilesystem includes support for many
different archive formats, modbox only supports ZIP files at present. The
virtual file system is rooted at the top level of the ZIP file. Uses
sharpfilesystem's **SharpZipLibFileSystem**; please note that this mod
system loads the *entire* mod into memory and so is unsuitable for mods
that involve media. (My own use of zip mods is for user content that only
contain script files; mods with media, including my base mod, use
directories.

All mods' file systems are wrapped within sharpfilesystem's
**ReadOnlyFileSystem** to ensure read-only access.

### Package Manifest ###

Mod packages should have a *manifest.xml* in their root. (If you don't want to
use XML, then implement IManifestBuilder and set ModContainerOptions.ManifestBuilder
to an instance of your IManifestBuilder.) A manifest file looks like this:

    <?xml version="1.0" encoding="UTF-8">
    <mod name="com.edropple.modbox.ExampleMod" 
         version="1.0">
        
        <deps>
            <!-- requires exactly FooMod 1.0 -->
            <dep name="com.edropple.modbox.FooMod" version="1.0" />
            <!-- requires any BarMod within the 2.x series -->
            <dep name="com.github.eropple.BarMod" version="2.x" />
            <!-- requires at least BazMod 1.5, but not 2.0 or higher  -->
            <dep name="org.bitbucket.eropple.BazMod" version="1.5+" />
        </deps>

    </mod>

Important notes regarding the package manifest:

- All mods' names must be unique. I decided to go with a Java-esque package
path, but you can name your mods however you want so long as they are
unique. ModContainer will not look at two mods with the same name and
different versions and just take the newer one; this is by design. Pick
one. :-)

- Versions are major.minor. Major and minor may be any numerals, with no
length restriction, but may not include any characters other than 0 to 9.
These are treated internally as integers and any leading zeroes are ignored;
a mod with a version of "2.09" will satisfy "2.8+".

### Mod Loading ###

The ModLocator field set in your ModContainerOptions object uses an
IModLocator; objects that implement the IModLocator interface handle turning
ModDefinition objects into Mods, which wrap the file system object. The
default one can be most easily accessed by ModLocator.FromPaths, which
takes a list of paths in ascending order of precedence in which to check
for mods. The default FileLocator checks those paths for mods of both
formats listed above under **Package Types**. For example:

    var options = new ModContainerOptions();
    options.ModLocator = ModLocator.FromPaths(basePath, commonAppData, userAppData);

#### basePath mods: ####

    basePath/basemod-1.0/
    basePath/gamepatch-1.0.zip

#### commonAppData mods: ####

    commonAppData/com.edropple.modbox.BarMod-2.1
    commonAppData/org.bitbucket.eropple.BazMod-1.7.zip

#### userAppData mods: ####

    userAppData/com.edropple.modbox.FooMod-1.0/
    userAppData/com.github.eropple.BarMod-2.0/
    userAppData/org.bitbucket.eropple.BazMod-1.5.zip

#### mod list: ####

    basemod 1.0
    gamepatch 1.x
    com.edropple.modbox.FooMod 1.0
    com.github.eropple.BarMod 2.1
    org.bitbucket.eropple.BazMod 1.5+

#### resolution: ####

- **basemod**: loaded from basePath
- **gamepatch**: loaded from basePath
- **FooMod**: loaded from userAppData
- **BarMod**: loaded from commonAppData
- **BazMod**: loaded from userAppData (first one that qualified)

### A Word on Dependency Resolution and Mods ###
It's probably obvious, but the loose coupling provided by the "1.x" and "1.5+"
versioning should generally be used in mods to specify a range of mods. You
should have a pretty good reason to specify one version of a mod as a
dependency.