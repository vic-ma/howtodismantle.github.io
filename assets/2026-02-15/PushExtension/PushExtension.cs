using Peakboard.ExtensionKit;

namespace PushExtension
{
    public class PushExtension : ExtensionBase
    {
        public PushExtension(IExtensionHost host) : base(host) { }
        
        protected override ExtensionDefinition GetDefinitionOverride()
        {
            return new ExtensionDefinition
            {
                ID = "PushExtension",
                Name = "Push Extension",
                Description = "A Peakboard extension for push messages",
                Version = "1.0",
                Author = "Michelle Wu",
                Company = "Dismantle Inc.",
                Copyright = "Copyright © Dismantle Inc.",
            };
        }

        protected override CustomListCollection GetCustomListsOverride()
        {
            return new CustomListCollection
            {
                new PushMessageCustomList(),
            };
        }
    }
}
