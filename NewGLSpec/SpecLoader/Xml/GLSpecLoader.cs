using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NewGLSpec.SpecLoader.Xml
{
    public class GLSpecLoader : IGLSpecLoader
    {

        public IGLSpecification LoadSpecification()
        {
            var context = new GLSpecification();
            using (var tr = new StreamReader("../../../../KhronosSpec/xml/gl.xml", Encoding.UTF8))
            {
                var doc = XDocument.Load(tr);

                var loaders =new IGLPartLoader<XElement>[]
                {
                    new GLTypedefinitionsLoader(), 
                    new GLEnumLoader(), 
                    new GLMethodLoader(), 
                    new GLFeatureLoader()
                };

                foreach (var loader in loaders)
                {
                    loader.LoadPart(context, doc.Root);
                }
            }

            return context;
        }
    }
}