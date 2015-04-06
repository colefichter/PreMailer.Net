using System.Linq;
using CsQuery;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PreMailer.Net.Tests
{
    [TestClass]
    public class CssElementStyleResolverTests
    {
        [TestMethod]
        public void GetAllStylesForElement()
        {
            var tableDomObject = CQ.CreateFragment("<td id=\"tabletest\" class=\"test\" bgcolor=\"\"></td>");
            var nodewithoutselector = tableDomObject.FirstElement();

            var clazz = new StyleClass();
            clazz.Attributes["background-color"] = CssAttribute.FromRule("background-color: red");

            var result = CssElementStyleResolver.GetAllStyles(nodewithoutselector, clazz);


            //CF April 6, 2015: Replacing the width/height dedicated attributes on image tags is causing problems for us. Let's just remove this feature.
            // See CssStyleEquivalence class.

            //Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, result.Count());

            Assert.AreEqual("style", result.ElementAt(0).AttributeName);

            //CF April 6, 2015: Replacing the width/height dedicated attributes on image tags is causing problems for us. Let's just remove this feature.
            // See CssStyleEquivalence class.

            //Assert.AreEqual("bgcolor", result.ElementAt(1).AttributeName);
        }
    }
}
