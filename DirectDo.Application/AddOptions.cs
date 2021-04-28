using System;

namespace DirectDo.Application
{

    public class BaseOption
    {
        public Guid ReqId { get; set; }
    }

    public class SearchOption : BaseOption
    {
    }

    public class DeleteOption : BaseOption
    {
        
    }
    
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
    }
}