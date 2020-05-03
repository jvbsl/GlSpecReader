using System;
using System.Linq;
using System.Xml.Linq;
using NewGLSpec.Extensions;

namespace NewGLSpec.SpecLoader.Xml
{
    public class GLMethodLoader : IGLPartLoader<XElement>
    {
        public void LoadPart(IGLSpecification context, XElement rootNode)
        {
            var methodTypes = context.Methods;

            var commands = rootNode.Elements("commands");
            foreach (var c in commands)
            {
                foreach (var command in c.Elements("command"))
                {
                    var prototypeBase = command.Element("proto");
                    var methodName = prototypeBase.Element("name")?.Value;
                    var returnTypeName = prototypeBase.Element("ptype")?.Value
                                         ?? prototypeBase.GetXmlText(element => string.Empty);//string.Join("", prototypeBase.Elements().Select(x => x.Value));
                    
                    if (methodName == null)
                        throw new ArgumentNullException();

                    returnTypeName = returnTypeName.Trim();
                    
                    if (string.IsNullOrEmpty(returnTypeName))
                        throw new ArgumentException();

                    var returnType = context.GetType(returnTypeName);
                    
                    var method = new GLMethod(returnType, methodName);

                    foreach (var p in command.Elements("param"))
                    {
                        var group = p.Attribute("group")?.Value;

                        var paramName = p.Element("name")?.Value;

                        var paramTypeName = p.GetXmlText(element => element.Name == "name" ? string.Empty : element.Value);
                        
                        var paramType = context.GetType(paramTypeName);
                        
                        method.Parameters.Add(new GLParameter(paramType, paramName));
                    }
                    
                    methodTypes.Add(methodName, method);
                }
            }
        }
    }
}