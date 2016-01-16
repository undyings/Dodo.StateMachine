using NitroBolt.Wui;
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
     public static HtmlResult<HElement> HView(object _state, JsonData[] jsons, HContext context)
    {
      var state = _state.As<SecondState>() ?? new SecondState();

      foreach (JsonData json in jsons)
      {
        switch (json.JPath("data", "command").ToString_Fair())
        {
          default:
            break;
        }
      }

      var page = Page();
      return new HtmlResult<HElement>
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
           ),
           h.Input(h.type("button"), new hdata { { "command", "new-order" }, { "container", "order" }, { "name", "order" } }, h.value("Сделать заказ типа " + "order"), h.onclick(";"))

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