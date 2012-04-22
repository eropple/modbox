using System;
using System.Collections.Generic;
using Ed.Modbox.Prototyping;

namespace Ed.Modbox
{
    public class ModContainer
    {
        protected readonly IList<ModDefinition> RequiredMods;
        protected readonly ModContainerOptions Options;

        public ModContainer(IList<ModDefinition> requiredMods,
                            ModContainerOptions options)
        {
            if (options.ModLocator == null)
            {
                throw new ArgumentException("A ModLocator must be specified in your options object.");
            }

            this.RequiredMods = requiredMods;
            this.Options = options;
        }







        /// <summary>
        /// Gets a given mod from the container, by name.
        /// </summary>
        /// <param name="modName">Name of the mod to return.</param>
        /// <returns>The requested Mod object, or null if not found.</returns>
        public Mod this[String modName]
        {
            get { throw new NotImplementedException();  }
        }



        
    }
}
