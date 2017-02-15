using BitDiamond.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Core.Utils
{
    public interface IUserContextAware
    {
        IUserContext UserContext { get; }
    }
}
