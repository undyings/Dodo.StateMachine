using Commune.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dodo.StateMachine
{
  public class MailContext
  {
    public readonly static MailContext Default = new MailContext();

    public IDataLayer DbConnection = null;
  }
}