using DirectDo.Domain;

namespace DirectDo.Application
{
    public class DeleteOptions : BaseOption
    {
        public override string GetCommandType()
        {
            return DoCommandTypes.Delete;
        }
    }
}