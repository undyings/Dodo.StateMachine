using System;
using System.Collections.Generic;
using System.Web;
using NitroBolt.Wui;
using MetaTech.Library;
using System.IO;


namespace Dodo.StateMachine
{
  public class HSync : HWebSynchronizeHandler
  {
    public HSync()
      : base(new Dictionary<string, Func<object, JsonData[], HContext, HtmlResult<HElement>>> 
        { 
          { "index", Mail.HView },
          { "default", Mail.HView },
          { "second", Second.HView },
          { "auth", AuthView.HView },
          { "mail", Mail.HView }
        })
    {
 
    }
  }
}
