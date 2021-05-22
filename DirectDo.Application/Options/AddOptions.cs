using System.Data;
using DirectDo.Domain;

namespace DirectDo.Application
{
    /// <summary>
    /// 添加计时选项
    /// </summary>
    public class AddOptions : BaseOption
    {
        public bool Sound { get; set; }

        public string At { get; set; }

        public string After { get; set; }

        public int Times { get; set; } = 1;

        public string Message { get; set; }

        public override string GetCommandType()
        {
            return DoCommandTypes.Add;
        }
    }
}