using System;
using System.Threading.Tasks;
using DirectDo.Domain.Models;

namespace DirectDo.Domain
{
    public interface IClock
    {
        public Task BeginWaitAsync();

        public ValueTask AddAlertTimeAsync(TimeIndexer indexer);
        ValueTask RemoveAsync(TimeIndexer indexer);
    }
}