using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace favoshelf.Data
{
    public interface INavigateParameter
    {
        Task<IReadOnlyList<FolderListItem>> GetItemList();

        string GetFolderName();
    }
}
