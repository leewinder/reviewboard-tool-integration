using System.IO;
using System.Xml;

namespace Stats_Runner.Utilities
{
    //
    // Decodes XML files from runs
    //
    public class Xml
    {
        public delegate bool OnXmlLoaded(XmlDocument root);

        //
        // Loads an XML file and calls a process function
        //
        public static bool LoadXml(string xmlContent, OnXmlLoaded onXmlLoaded)
        {
            // Doing this the long way so we can disable namespace errors present in the stub files
            XmlDocument xmlDoc = null;
            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent);
            }
            catch
            {
                return false;
            }

            try
            {
                // Check the content
                bool xmlAccepted = onXmlLoaded(xmlDoc);
                if (xmlAccepted == false)
                    return false;
            }
            catch
            {
                return false;
            }

            // We're OK
            return true;
        }
    }
}
