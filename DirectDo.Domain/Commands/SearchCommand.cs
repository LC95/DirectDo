using DirectDo.Domain.Models;

namespace DirectDo.Domain.Commands
{
    public class SearchCommand : IControlCommand
    {
        public SearchCommand(string searchText)
        {
            SearchText = searchText;
        }

        public string SearchText { get; }
    }
}