using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveGameManager.Interfaces
{
    public interface ISettingsService
    {
        public bool ReplaceEnabled { get; set; }
        public bool LoadEnabled { get; set; }
        public bool DeleteEnabled { get; set; }
    }
}
