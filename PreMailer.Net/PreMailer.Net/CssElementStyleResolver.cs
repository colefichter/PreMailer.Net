using System.Collections.Generic;
using CsQuery;

namespace PreMailer.Net
{
   public class CssElementStyleResolver
   {
        public static IEnumerable<AttributeToCss> GetAllStyles(IDomObject domElement, StyleClass styleClass)
        {
            var attributeCssList = new List<AttributeToCss>
                {
                    new AttributeToCss {AttributeName = "style", CssValue = styleClass.ToString()}
                };

            //CF April 6, 2015: Replacing the width/height dedicated attributes on image tags is causing problems for us. Let's just remove this feature.
            //attributeCssList.AddRange(CssStyleEquivalence.FindEquivalent(domElement, styleClass));

            return attributeCssList;
        }
    }
}