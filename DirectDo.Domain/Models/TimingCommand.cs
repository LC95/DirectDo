using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectDo.Domain.Models
{
    public interface IControlCommand : INotification
    {
        public Guid Id { get; }
    }

}