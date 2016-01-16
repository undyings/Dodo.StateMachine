using Commune.Html;
using Commune.Html.Standart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dodo.StateMachine
{
  public class SeptStd
  {
    public static HButton IconButton(string caption, string fontIcon)
    {
      return std.Button(caption,
        new HBefore().FontFamily("sept").Content(fontIcon).VAlign(-2).MarginRight("4px")
      );
    }

    public static HCheckEdit CheckEdit(string dataName, bool value, params HStyle[] pseudoStyles)
    {
      HTone frameStyle = new HTone().Display("inline-block").Width("10px").Height("10px")
        .Border("1px", "solid", "#9a9ca0", "2px").CssAttribute("margin-top", "1px")
        .Background("#fefefe");

      HTone markStyle = new HTone().FontFamily("sept").FontSize("16px").Content(@"\2714")
        .CssAttribute("line-height", "10px").MarginLeft(-2);

      return new HCheckEdit(dataName, value, frameStyle, markStyle, pseudoStyles);
    }


  }
}