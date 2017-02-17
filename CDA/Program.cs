using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDA
{
    class Program
    {
        static void Main(string[] args)
        {
            var doc = new POCD_MT000040ClinicalDocument();
            doc.versionNumber = new INT() { value = "1" };
            var x = new System.Xml.Serialization.XmlSerializer(doc.GetType());
            x.Serialize(Console.Out, doc);
        }
    }
}
