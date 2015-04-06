﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PreMailer.Net.Tests
{
	[TestClass]
	public class PreMailerTests
	{
        [TestMethod]
        public void MoveCssInline_DoNotCreateDedicatedAttributes()
        {
            string input = "<html><head><style type=\"text/css\">.test { height: 100px; width: 100px; }</style></head><body><img class=\"test\" /></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input, false);

            Assert.IsTrue(premailedOutput.Html.Contains("style=\"height: 100px;width: 100px"));
            
            Assert.IsFalse(premailedOutput.Html.Contains("height=\"100"));
            Assert.IsFalse(premailedOutput.Html.Contains("width=\"100"));
        }

        [TestMethod]
        public void MoveCssInline_DoNotRemoveDedicatedAttributes()
        {
            string input = "<html><head><style type=\"text/css\">.test { height: 100px; width: 100px; }</style></head><body><img class=\"test\" width=\"50\" height=\"49\" /></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input, false);

            Assert.IsTrue(premailedOutput.Html.Contains("style=\"height: 100px;width: 100px"));
            
            Assert.IsFalse(premailedOutput.Html.Contains("height=\"100"));
            Assert.IsFalse(premailedOutput.Html.Contains("width=\"100"));

            Assert.IsTrue(premailedOutput.Html.Contains("height=\"49\""));
            Assert.IsTrue(premailedOutput.Html.Contains("width=\"50\""));
        }

		[TestMethod]
		public void MoveCssInline_RespectExistingStyleElement()
		{
			string input = "<html><head><style type=\"text/css\">.test { height: 100px; }</style></head><body><div class=\"test\" style=\"width: 100px;\">test</div></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.Contains("<div class=\"test\" style=\"height: 100px;width: 100px"));
		}

		[TestMethod]
		public void MoveCssInline_InlineStyleElementTakesPrecedence()
		{
			string input = "<html><head><style type=\"text/css\">.test { width: 150px; }</style></head><body><div class=\"test\" style=\"width: 100px;\">test</div></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.Contains("<div class=\"test\" style=\"width: 100px"));
		}

		[TestMethod]
		public void MoveCssInline_CssWithHigherSpecificity_AppliesMoreSpecificCss()
		{
			string input = "<html><head><style type=\"text/css\">#high-imp.test { width: 42px; } .test { width: 150px; }</style></head><body><div id=\"high-imp\" class=\"test\">test</div></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.Contains("style=\"width: 42px\""));
		}

		[TestMethod]
		public void MoveCssInline_CssWithHigherSpecificityInSeparateStyleTag_AppliesMoreSpecificCss()
		{
			string input = "<html><head><style type=\"text/css\">.target { width: 42px; }</style><style type=\"text/css\">.outer .inner .target { width: 1337px; }</style></head><body><div class=\"outer\"><div class=\"inner\"><div class=\"target\">test</div></div></div></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.Contains("style=\"width: 1337px\""));
		}

		[TestMethod]
		public void MoveCssInline_IgnoreStyleElement_DoesntApplyCss()
		{
			string input = "<html><head><style type=\"text/css\">.target { width: 42px; }</style><style type=\"text/css\" id=\"ignore\">.target { width: 1337px; }</style></head><body><div class=\"outer\"><div class=\"inner\"><div class=\"target\">test</div></div></div></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false, ignoreElements: "#ignore");

			Assert.IsTrue(premailedOutput.Html.Contains("style=\"width: 42px\""));
		}

		[TestMethod]
		public void MoveCssInline_SupportedPseudoSelector_AppliesCss()
		{
			string input = "<html><head><style type=\"text/css\">li:first-child { width: 42px; }</style></head><body><ul><li>target</li><li>blargh></li></ul></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsTrue(premailedOutput.Html.Contains("<li style=\"width: 42px\">"));
		}

		[TestMethod]
		public void MoveCssInline_SupportedjQuerySelector_AppliesCss()
		{
			string input = "<html><head><style type=\"text/css\">li:first { width: 42px; }</style></head><body><ul><li>target</li><li>blargh></li></ul></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsTrue(premailedOutput.Html.Contains("<li style=\"width: 42px\">target</li>"));
		}

		[TestMethod]
		public void MoveCssInline_UnsupportedSelector_AppliesCss()
		{
			string input = "<html><head><style type=\"text/css\">p:first-letter { width: 42px; }</style></head><body><p>target</p></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsTrue(premailedOutput.Html.Contains("<p>target</p>"));
		}

		[TestMethod]
		public void MoveCssInline_KeepStyleElementsIgnoreElementsMatchesStyleElement_DoesntRemoveScriptTag()
		{
			string input = "<html><head><style id=\"ignore\" type=\"text/css\">li:before { width: 42px; }</style></head><body><div class=\"target\">test</div></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, removeStyleElements: false, ignoreElements: "#ignore");

			Assert.IsTrue(premailedOutput.Html.Contains("<style id=\"ignore\" type=\"text/css\">"));
		}

		[TestMethod]
		public void MoveCssInline_MultipleSelectors_HonorsIndividualSpecificity()
		{
			string input = "<html><head><style type=\"text/css\">p,li,tr.pub-heading td,tr.pub-footer td,tr.footer-heading td { font-size: 12px; line-height: 16px; } td.disclaimer p {font-size: 11px;} </style></head><body><table><tr class=\"pub-heading\"><td class=\"disclaimer\"><p></p></td></tr></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsTrue(premailedOutput.Html.Contains("<p style=\"font-size: 11px;line-height: 16px\"></p>"));
		}

		[TestMethod]
		public void MoveCssInline_ImportantFlag_HonorsImportantFlagInStylesheet()
		{
			string input = "<style>div { color: blue !important; }</style><div style=\"color: red\">Red</div>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsTrue(premailedOutput.Html.Contains("<div style=\"color: blue"));
		}

		[TestMethod]
		public void MoveCssInline_ImportantFlag_HonorsImportantFlagInline()
		{
			string input = "<style>div { color: blue !important; }</style><div style=\"color: red !important\">Red</div>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsTrue(premailedOutput.Html.Contains("<div style=\"color: red"));
		}

		[TestMethod]
		public void MoveCssInline_AbsoluteBackgroundUrl_ShouldNotBeCleanedAsComment()
		{
			string input = "<style>div { background: url('http://my.web.site.com/Content/email/content.png') repeat-y }</style><div></div>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsTrue(premailedOutput.Html.Contains("<div style=\"background: url('http://my.web.site.com/Content/email/content.png') repeat-y\"></div>"));
		}

		public void MoveCssInline_SupportedMediaAttribute_InlinesAsNormal()
		{
			string input = "<html><head><style type=\"text/css\" media=\"screen\">div { width: 100% }</style></head><body><div>Target</div></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsTrue(premailedOutput.Html.Contains("<div style=\"width: 100%;\">Target</div>"));
		}

		[TestMethod]
		public void MoveCssInline_UnsupportedMediaAttribute_IgnoresStyles()
		{
			string input = "<html><head><style type=\"text/css\" media=\"print\">div { width: 100% }</style></head><body><div>Target</div></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsTrue(premailedOutput.Html.Contains("<div>Target</div>"));
		}

        //CF April 6, 2015: Replacing the width/height dedicated attributes on image tags is causing problems for us. Let's just remove this feature.
        // See CssStyleEquivalence class.

        //[TestMethod]
        //public void MoveCssInline_AddBgColorStyle()
        //{
        //    string input = "<html><head><style type=\"text/css\">.test { background-color:#f1f1f1; }</style></head><body><table><tr><td class=\"test\" bgcolor=\"\"></td></tr></table></body></html>";

        //    var premailedOutput = PreMailer.MoveCssInline(input, false);

        //    Assert.IsTrue(premailedOutput.Html.Contains("<td class=\"test\" style=\"background-color: #f1f1f1\" bgcolor=\"#f1f1f1\">"));
        //}

        [TestMethod]
        public void MoveCssInline_AddBgColorStyle_IgnoreElementWithBackgroundColorAndNoBgColor()
        {
            string input = "<html><head><style type=\"text/css\">.test { background-color:#f1f1f1; }</style></head><body><table><tr><td class=\"test\"></td></tr></table></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input, false);

            Assert.IsTrue(premailedOutput.Html.Contains("<td class=\"test\" style=\"background-color: #f1f1f1\""));
        }

	}
}
