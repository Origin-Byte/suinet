using System.ComponentModel;

namespace Suinet.Rpc.Types
{
    public class ObjectDataOptions
    {
        [DefaultValue(false)]
        public bool ShowBcs { get; set; }

        [DefaultValue(false)]
        public bool ShowContent { get; set; }

        [DefaultValue(false)]
        public bool ShowDisplay { get; set; }

        [DefaultValue(false)]
        public bool ShowOwner { get; set; }

        [DefaultValue(false)]
        public bool ShowPreviousTransaction { get; set; }

        [DefaultValue(false)]
        public bool ShowStorageRebate { get; set; }

        [DefaultValue(false)]
        public bool ShowType { get; set; }
    }
}
