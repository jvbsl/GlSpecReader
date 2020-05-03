using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NewGLSpec.Extensions;

namespace NewGLSpec.SpecLoader.Xml
{
    public class GLEnumLoader : IGLPartLoader<XElement>
    {
        public static ulong ConvertToUInt64(string val)
        {
            if (val == null)
                throw new ArgumentNullException();
            return (ulong)(long)new System.ComponentModel.Int64Converter().ConvertFromString(val);
        }
        
        public void LoadPart(IGLSpecification context, XElement rootNode)
        {
            const string UNKNOWN_GROUP_NAME = "UNKNOWN_GROUP";
            var enumTypes = context.Enums;
            
            var unknownEnumGroup = new GLEnum(UNKNOWN_GROUP_NAME, null);
            
            enumTypes.Add(UNKNOWN_GROUP_NAME, unknownEnumGroup);
            
            foreach (var e in rootNode.Elements("enums"))
            {
                var vendorName = e.Attribute("vendor")?.Value;
                var parentGroups = e.Attribute("group");
                foreach (var enumEntry in e.Elements("enum"))
                {
                    var enumEntryName = enumEntry.Attribute("name")?.Value;
                    var enumEntryValue = enumEntry.Attribute("value")?.Value;
                    var parentGroupValue = parentGroups?.Value;
                    var enumGroupValue = enumEntry.Attribute("group")?.Value;
                    IEnumerable<string> enumGroups = (enumGroupValue)?.Split(',');
                    if (enumGroups != null)
                    {
                        if (parentGroupValue != null && !enumGroups.Contains(parentGroupValue))
                        {
                            enumGroups = enumGroups.Append(parentGroupValue);
                        }
                    }
                    else
                    {
                        enumGroups = new[] {parentGroupValue ?? UNKNOWN_GROUP_NAME};
                    }
                        
                    if (enumEntryName == null || enumEntryValue == null)
                        throw new ArgumentNullException();
                        
                    foreach (var enumGroup in enumGroups)
                    {
                        if (!enumTypes.TryGetValue(enumGroup, out var enumType))
                        {
                            enumType = new GLEnum(enumGroup, vendorName);
                            enumTypes.Add(enumGroup, enumType);
                        }
                            
                        enumType.AddEntry(enumEntryName, ConvertToUInt64(enumEntryValue));
                    }
                }
            }
        }
    }
}