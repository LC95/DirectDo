using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DirectDo.Domain.Models
{
    public interface IAlertService
    {
        public void Add(TimingCommand timingCommand);

        public IEnumerable<TimingCommand> Search();

        public Task RunAlertAsync(CancellationToken cancellationToken);
    }
}