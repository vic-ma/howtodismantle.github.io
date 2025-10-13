using Peakboard.ExtensionKit;

namespace MeowExtension
{
    public class MeowExtension : ExtensionBase
    {
        public MeowExtension(IExtensionHost host) : base(host) { }
        
        protected override ExtensionDefinition GetDefinitionOverride()
        {
            return new ExtensionDefinition
            {
                ID = "MeowExtension",
                Name = "Meow Extension",
                Description = "A Peakboard extension for cats",
                Version = "1.2.0",
                Author = "Meow Development Team",
                Company = "Meow Inc.",
                Copyright = "Copyright © Meow Inc.",
            };
        }

        protected override CustomListCollection GetCustomListsOverride()
        {
            return new CustomListCollection
            {
                new CatCustomList(),
            };
        }
    }
}
