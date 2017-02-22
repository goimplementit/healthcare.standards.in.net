using IHE.XDS.DK.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHE.XDS.DK
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = new WebService.Security();
            var h = new WebService.header();
            var hs = new WebService.hsuidHeader();
            var r = new ProvideAndRegisterDocumentSetRequestType();
            new WebService.DocumentRepository_PortTypeClient().DocumentRepository_ProvideAndRegisterDocumentSetb(s, ref h, hs, r);
        }
    }
}
