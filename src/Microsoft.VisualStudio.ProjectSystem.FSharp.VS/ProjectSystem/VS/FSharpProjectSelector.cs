﻿using System;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using Microsoft.VisualStudio.Packaging;
using Microsoft.VisualStudio.ProjectSystem.VS.Generators;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.ProjectSystem.VS
{
    [ComVisible(true)]
    [Guid(SelectorGuid)]
    [ClassRegistration(SelectorGuid, AssemblyQualifiedSelectorTypeName)]
    public sealed class FSharpProjectSelector : IVsProjectSelector
    {
        public const string SelectorGuid = "E720DAD0-1854-47FC-93AF-E719B54B84E6";
        public const string AssemblyQualifiedSelectorTypeName = "Microsoft.VisualStudio.ProjectSystem.VS.FSharpProjectSelector, Microsoft.VisualStudio.ProjectSystem.FSharp.VS";
        private const string MSBuildXmlNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

        public void GetProjectFactoryGuid(Guid guidProjectType, string pszFilename, out Guid guidProjectFactory)
        {
            var doc = XDocument.Load(pszFilename);
            GetProjectFactoryGuid(doc, out guidProjectFactory);
        }

        internal static void GetProjectFactoryGuid(XDocument doc, out Guid guidProjectFactory)
        {
            var nsm = new XmlNamespaceManager(new NameTable());
            nsm.AddNamespace("msb", MSBuildXmlNamespace);

            // If the project has either a Project-level SDK attribute or an Import-level SDK attribute, we'll open it with the new project system.
            // Check both namespace-qualified and unqualified forms to include projects with and without the xmlns attribute.
            var hasProjectElementWithSdkAttribute = doc.XPathSelectElement("/msb:Project[@Sdk]", nsm) != null || doc.XPathSelectElement("/Project[@Sdk]") != null;
            var hasImportElementWithSdkAttribute = doc.XPathSelectElement("/*/msb:Import[@Sdk]", nsm) != null || doc.XPathSelectElement("/*/Import[@Sdk]") != null;

            if (hasProjectElementWithSdkAttribute || hasImportElementWithSdkAttribute)
            {
                guidProjectFactory = Guid.Parse(FSharpProjectSystemPackage.ProjectTypeGuid);
                return;
            }

            guidProjectFactory = Guid.Parse(FSharpProjectSystemPackage.LegacyProjectTypeGuid);
        }
    }
}
