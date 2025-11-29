using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fontendo.Extensions.FontBase;

namespace Fontendo.Interfaces
{
    public interface IFontendoFont
    {
        public ActionResult Load(string filename);
        public ActionResult Save(string filename);
        public Sheets GetSheets();
    }
}
