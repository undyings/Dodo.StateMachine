//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Commune.Basis;
//using NitroBolt.Wui;
//using System.Drawing;

//namespace Commune.Html
//{
//  public class HCheckEdit2 : ExtensionContainer, IHtmlControl
//  {
//    readonly bool value;
//    readonly HStyle[] pseudoClasses;
//    public HCheckEdit2(string dataName, bool value, params HStyle[] pseudoClasses) :
//      base("HCheckEdit", dataName)
//    {
//      this.value = value;
//      this.pseudoClasses = pseudoClasses;
//    }

//    static readonly HBuilder h = null;

//    public HElement ToHtml(string cssClassName, StringBuilder css)
//    {
//      string checkClassName = string.Format("{0}_check", Name);
//      string frameClassName = string.Format("{0}_frame", Name);

//      HTone innerStyle = new HTone().Display("none");
//      HtmlHlp.AddClassToCss(css, checkClassName, innerStyle.CssExtensions);

//      HTone frameStyle = new HTone().Display("inline-block").Width("10px").Height("10px")
//        .Border("1px", "solid", "#9a9ca0", "2px").CssAttribute("margin-top", "1px")
//        .Background("#fefefe");
//      HtmlHlp.AddClassToCss(css, frameClassName, frameStyle.CssExtensions);

//      //HTone frameCheckedStyle = new HTone().Background(Color.Red);
//      //HtmlHlp.AddExtensionsToCss(css,
//      //  frameCheckedStyle.CssExtensions,
//      //  ".{0} input[type=checkbox]:checked ~ .{1} ",
//      //  cssClassName, frameClassName
//      //);

//      HTone markStyle = new HTone().FontFamily("sept").FontSize("16px").Content(@"\2714")
//        .CssAttribute("line-height", "10px").MarginLeft(-2);
//      HtmlHlp.AddExtensionsToCss(css, 
//        markStyle.CssExtensions,
//        ".{0} input[type=checkbox]:checked ~ .{1}::before ",
//        cssClassName, frameClassName
//      );

//      {
//        DefaultExtensionContainer defaults = new DefaultExtensionContainer(this);
//        defaults.Display("inline-block");
//        defaults.OnClick(string.Format(
//          "e.preventDefault(); $('.{0}').is(':checked') ? $('.{0}').prop('checked', false) : $('.{0}').prop('checked', true);",
//          checkClassName)
//        );
//      }

//      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);
//      foreach (HStyle pseudo in pseudoClasses)
//        HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

//      List<object> checkElements = new List<object>();
//      checkElements.Add(h.@class(checkClassName));
//      checkElements.Add(h.type("checkbox"));
//      checkElements.Add(h.data("name", Name));
//      checkElements.Add(h.data("id", checkClassName));
//      if (value)
//        checkElements.Add(h.@checked());

//      return h.Div(HtmlHlp.ContentForHElement(this, cssClassName,
//        h.Input(checkElements.ToArray()),
//        h.Div(h.@class(frameClassName))
//      ));
//    }
//  }
//}
