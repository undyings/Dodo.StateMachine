using SoftTech.Wui;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Web;
using MetaTech.Library;
using System.Xml.XPath;
using System.Xml.Linq;

namespace Dodo.StateMachine
{
  public class Second
  {
    public static SoftTech.Wui.HtmlResult<HElement> HView(object _state, JsonData json, HContext context)
    {
      var state = _state.As<SecondState>() ?? new SecondState();

      if (json != null)
      {
        switch (json.JPath("data", "command").ToString_Fair())
        {
          default:
            break;
        }
      }

      var page = Page();
      return new SoftTech.Wui.HtmlResult<HElement>
      {
        Html = page,
        State = state,
      };
    }
    private static HElement Page()
    {
      var page = h.Html
      (
        h.Head(          
          h.Element("title", "Dodo. State machine")
        ),
        h.Body
        (
           h.Div
           (
             DateTime.UtcNow
           )

        )
      );
      return page;
    }



    static readonly HBuilder h = null;

  }
  class SecondState
  {
    public SecondState()
    {
    }
    public MainState With()
    {
      return new MainState();
    }
  }
}