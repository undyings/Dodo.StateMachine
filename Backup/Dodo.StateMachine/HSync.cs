using System;
using System.Collections.Generic;
using System.Web;
using SoftTech.Wui;

namespace Dodo.StateMachine
{
  public class HSync : HWebSynchronizeHandler
  {
    public HSync()
      : base(new Dictionary<string, Func<object, JsonData, HContext, HtmlResult<HElement>>> 
        { 
          { "index", Main.HView },
          { "default", Main.HView },
          { "second", Second.HView },
        })
    {
    }
  }
}
