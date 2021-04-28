using DirectDo.Domain;

namespace DirectDo.Application
{
    public class LookOptions : BaseOption
    {
        public string Text { get; set; }
        public override string GetCommandType()
        {
            return DoCommandTypes.Lookup;
        }
    }
}