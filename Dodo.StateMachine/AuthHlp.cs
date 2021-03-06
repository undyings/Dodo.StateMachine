﻿using MetaTech.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dodo.StateMachine
{
  public static class AuthHlp
  {
    public static string UserName(this HttpContext context)
    {
      if (context != null && context.User != null && context.User.Identity != null)
        return context.User.Identity.Name.EmptyAsNull();
      return null;
    }
    public static void SetUserAndCookie(this HttpContext context, string login, bool isSetCookie = true)
    {
      if (login == null)
      {
        System.Web.Security.FormsAuthentication.SignOut();
        context.User = null;
      }
      else
      {
        if (isSetCookie)
        {
          var authTicket = new System.Web.Security.FormsAuthenticationTicket
            (
               1, //version
               login, // user name
               DateTime.Now,             //creation
               DateTime.Now.AddYears(50), //Expiration (you can set it to 1 month
               true,  //Persistent
               login
            ); // additional informations
          var encryptedTicket = System.Web.Security.FormsAuthentication.Encrypt(authTicket);

          var authCookie = new HttpCookie(System.Web.Security.FormsAuthentication.FormsCookieName, encryptedTicket);
 
          authCookie.Expires = authTicket.Expiration;
          authCookie.HttpOnly = true;

          context.Response.SetCookie(authCookie);
        }
        context.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(login), Array<string>.Empty);
      }
    }
  }
}