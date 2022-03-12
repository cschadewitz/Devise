using System.IO;
using System.Linq;
using System.Reflection;

namespace Devise.Utilities
{
    static class TemplateResourceReader
    {
        public static string ReadTemplate(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = assembly.GetManifestResourceNames().Single(res => res.EndsWith(name + ".template"));
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
            
        }
    }
}
