using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using NewGLSpec.Extensions;

namespace NewGLSpec.SpecLoader.Xml
{
    public class GLTypedefinitionsLoader : IGLPartLoader<XElement>
    {
        public void LoadPart(IGLSpecification context, XElement root)
        {
            var basicTypes = context.BasicTypes;
            var types = root.Elements("types");
            foreach(var tps in types)
            {
                foreach (var t in tps.Elements("type"))
                {
                    var typeDef = t.GetXmlText();

                    var multiTypeDefs = PreprocessRegion(context, typeDef);
                    if (multiTypeDefs == null)
                        continue;
                    foreach (var (region, condition) in multiTypeDefs)
                    {
                        if (!ExtractTypeDef(region, out typeDef, out var typeName))
                            continue;
                        var basicType = new GLTypeDef(typeName, new GLTypeReference(context, typeDef));

                        switch (condition)
                        {
                            case "TRUE":
                                basicTypes.Add(typeName, basicType);
                                break;
                            case "FALSE":
                                break;
                            default:
                                AddConditionalTypeDef(context, basicType, condition);
                                break;
                        }
                    }
                }
            }
        }

        private static Regex _functionPointerRegex = new Regex("typedef (?<returnParam>[^\\(]+)\\(\\s*\\*(?<name>\\w+)\\s*\\)\\s*\\((?<parameters>[^\\)]*)\\)", RegexOptions.Compiled);
        private static bool ExtractFunctionTypeDef(string region, out string type, out string name)
        {
            type = null;
            name = null;
            var match = _functionPointerRegex.Match(region);
            if (!match.Success)
                return false;
            
            type = match.Groups["returnParam"].Value + "(" + match.Groups["parameters"] + ")";
            name = match.Groups["name"].Value;
            return true;
        }
        private static bool ExtractTypeDef(string region, out string type, out string name)
        {
            const string typedefPrefix = "typedef ";
            
            type = null;
            name = null;
            region = region.Trim(' ', ';');

            if (region.Contains('(') && region.Contains(')'))
            {
                return ExtractFunctionTypeDef(region, out type, out name);
            }

            int lastPart = Array.FindLastIndex(region.ToCharArray(), (c) => !char.IsLetterOrDigit(c));
            if (lastPart == -1)
                return false;
            lastPart++;
            name = region.Substring(lastPart).Trim();

            type = region.Substring(0, lastPart).Trim();
            if (type.StartsWith(typedefPrefix))
                type = type.Substring(typedefPrefix.Length).Trim();
            return true;
        }

        private static void AddConditionalTypeDef(IGLSpecification context, GLTypeDef basicType, string condition)
        {
            var basicTypes = context.BasicTypes;
            GLConditionalTypeDef conditionalTypeDef;
            if (basicTypes.TryGetValue(basicType.Name, out var alreadyExisting))
            {
                conditionalTypeDef = alreadyExisting as GLConditionalTypeDef;
                if (conditionalTypeDef == null)
                {
                    throw new KeyNotFoundException();
                }
            }
            else
            {
                conditionalTypeDef = new GLConditionalTypeDef(basicType.Name);
                                    
                basicTypes.Add(basicType.Name, conditionalTypeDef);
            }
                                
            conditionalTypeDef.TypeRefs.Add(new GLConditionalTypeDef.ConditionalTypeRef(basicType, condition));
        }

        private static (string region, string condition)[] PreprocessRegion(IGLSpecification context, string region)
        {
            var splt = region.Split('\n', '#');

            if (splt.Length == 1)
                return new[] { (splt[0], "TRUE") };

            List<(string, string)> regions = new List<(string, string)>();
            bool isPreprocessor = false;
            string currentRegion = "";
            string currentCondition = "";
            foreach (var s in splt)
            {
                if (string.IsNullOrEmpty(s))
                {
                    isPreprocessor = true;
                    continue;
                }

                if (isPreprocessor)
                {
                    isPreprocessor = false;
                    if (s.StartsWith("include"))
                        return null; // TODO: not implemented
                    string newCondition;
                    if (s.StartsWith("ifdef"))
                    {
                        newCondition = "defined " + s.Substring("ifdef".Length + 1);
                    }
                    else if (s.StartsWith("if"))
                    {
                        newCondition = s.Substring("if".Length + 1);
                    }
                    else if (s.StartsWith("else"))
                    {
                        newCondition = "!" + currentCondition;
                    }
                    else if (s.StartsWith("elif"))
                    {
                        newCondition = "(! " + currentCondition + ") && (" + s.Substring("elif".Length + 1) + ")";
                    }
                    else if (s.StartsWith("endif"))
                    {
                        newCondition = "";
                    }
                    else
                    {
                        throw new NotSupportedException(s);
                    }

                    
                    if (!string.IsNullOrWhiteSpace(currentRegion) && !string.IsNullOrWhiteSpace(currentCondition))
                    {
                        regions.Add((currentRegion, currentCondition));
                        currentRegion = "";
                    }
                    currentCondition = newCondition;
                }
                else
                {
                    currentRegion += s;
                }
            }

            return regions.ToArray();
        }

    }
}