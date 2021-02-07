using Cinchoo.Core.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Cinchoo.Core
{
    [ChoPlugIn("FileCreatedTimestampPlugIn", typeof(ChoCSharpCodeSnippetPlugIn), typeof(ChoNonEditableCSharpCodeSnippetPlugInBuilderProperty))]
    public class ChoFileCreatedTimestampPlugInBuilder : ChoNonEditableCSharpCodeSnippetPlugInBuilder 
    {
        public ChoFileCreatedTimestampPlugInBuilder()
        {
            CodeSnippet = new ChoCDATA(@"
            if (!args[0])
            {
                File.SetCreationTime(args[2], DateTime.Now);
            }
            else
            {
                return args[1];
            }
        ");
            Namespaces = "System.IO";
        }
    }
}
