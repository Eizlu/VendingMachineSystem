using System.IO;
using System.Xml.Serialization;
using VendingMachineSystem.Core;

namespace VendingMachineSystem.DAL
{
    public class XmlRepository
    {
        public void UlozitDoXml<T>(T data, string nazevSouboru)
        {
            string cesta = Path.Combine(Directory.GetCurrentDirectory(), nazevSouboru);

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (StreamWriter writer = new StreamWriter(cesta))
            {
                serializer.Serialize(writer, data);
            }
        }
    }
}