using System;

namespace DirectDo.Application
{
    public abstract class BaseOption
    {
        public abstract string GetCommandType();
        public Guid ReqId { get; set; }
    }
}