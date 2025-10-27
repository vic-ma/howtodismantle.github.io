using Peakboard.ExtensionKit;

namespace PushMessageExtension
{
    public class PushMessageExtension : ExtensionBase
    {
        public PushMessageExtension(IExtensionHost host) : base(host) { }
        
        protected override ExtensionDefinition GetDefinitionOverride()
        {
            return new ExtensionDefinition
            {
                ID = "PushMessageExtension",
                Name = "Push Message Extension",
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
