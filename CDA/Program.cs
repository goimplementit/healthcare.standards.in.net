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
            doc.component = new POCD_MT000040Component2();
            var structuredBody = new POCD_MT000040StructuredBody();
            doc.component.Item = structuredBody;
            var list = new LinkedList<POCD_MT000040Component3>();
            var c3 = new POCD_MT000040Component3();
            var section = new POCD_MT000040Section();

            var listentries = new LinkedList<POCD_MT000040Entry>();
            var e = new POCD_MT000040Entry();
            var encounter = new POCD_MT000040Encounter();
            var id = new II();
            id.root = "ldskfjlkj";
            var listOfIDs = new List<II>();
            listOfIDs.Add(id);
            encounter.id = listOfIDs.ToArray();
            e.Item = encounter;
            listentries.AddFirst(e);
            section.entry = listentries.ToArray();
            c3.section = section;
            list.AddFirst(c3);
            structuredBody.component = list.ToArray();
            x.Serialize(Console.Out, doc);

            //For everest samples - see https://everest.codeplex.com/wikipage?title=Everest%20CDA%20Tutorial%20%E2%80%93%20Part%201%20%E2%80%93%20The%20Header&referringTitle=Documentation
        }
    }
}
