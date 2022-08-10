using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shortid;

namespace iro4cli
{
    /// <summary>
    /// Represents all include visitors.
    /// </summary>
    public partial class IroVisitor : iroBaseVisitor<IroVariable>
    {
        /// <summary>
        /// Visits a single include node and returns an attribute.
        /// </summary>
        public override IroVariable VisitInclude([NotNull] iroParser.IncludeContext context)
        {
            return new IroAttribute("include_" + ShortId.Generate(RAND_ID_LEN),
                                    new IroInclude(context.IDENTIFIER()[1].GetText()));
        }
    }
}
