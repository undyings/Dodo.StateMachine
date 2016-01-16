using NitroBolt.Wui;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Web;
using MetaTech.Library;
using System.Xml.XPath;
using System.Xml.Linq;
using System.IO;
using Commune.Basis;
using System.Text;
using Commune.Html;
using Commune.Html.Standart;
using System.Drawing;
using System.Collections.Specialized;

namespace Dodo.StateMachine
{
  public class Mail
  {
    public static HtmlResult<HElement> HView(object _state, JsonData[] jsons, HContext context)
    {
      MailState state = _state.As<MailState>();
      if (state == null)
      {
        state = new MailState();
        Logger.AddMessage("Пересоздан state");
      }

      //Logger.AddMessage("Mail.Execute: {0}, {1}, {2}", 
      //  context.HttpContext.User.Identity.Name, context.HttpContext.Request.QueryString, jsons.Length);
 
      foreach (JsonData json in jsons)
      {
        try
        {
          Logger.AddMessage("Json: {0}", json);
          state.lastJson = json;
          HElement cachePage = Page(state, context.HttpContext);

          hevent eventh = cachePage.FindEvent(json, true);
          Logger.AddMessage("Event.Find: {0}", eventh != null);
          if (eventh != null)
          {
            eventh.Execute(json);
          }
        }
        catch (Exception ex)
        {
          Logger.WriteException(ex);
        }

      }

      var page = Page(state, context.HttpContext);
      return new HtmlResult<HElement>
      {
        Html = page,
        State = state,
        RefreshPeriod = TimeSpan.FromHours(1)
      };
    }

    static readonly HBuilder h = null;

    //readonly static ExtensionContainer buttonStyle = new ExtensionContainer("buttonStyle", "mail-button-style").
    //  LinearGradient("to bottom", Color.FromArgb(201, 201, 201), Color.FromArgb(241, 241, 241));

    static HElement Page2(MailState state, HttpContext httpContext)
    {
      return h.Html
      (
        h.Head(
          h.Element("title", "Dodo.State machine")
          //h.LinkCss("font/font.css"),
          //h.LinkScript("ckeditor/ckeditor.js")
//          h.Script(@"
//               function CK_updateAll()
//               {
//                 try
//                 {
//                   for (instance in CKEDITOR.instances)
//                     CKEDITOR.instances[instance].updateElement();
//                 }
//                 catch(ex)
//                 {
//                   console.log(ex);
//                 }
//               }
//            ")
        ),
        h.Body(h.Div("test")
        )
      );
    }

    static HElement Page(MailState state, HttpContext httpContext)
    {
      try
      {
        string viewKind = httpContext.Get("view") ?? "inbox";
        int? displayPage = httpContext.GetUInt("page");
        int? messageId = httpContext.GetUInt("id");

        //Logger.AddMessage("ViewKind: {0}, {1}", viewKind, state.MessageSendError);

        bool guest = httpContext.UserName() == null;

        string title;
        IHtmlControl mainPanel = new HPanel("main",
          std.RowPanel(
            new HHoverDropdown(
              new HDropStyle().
                DropList(new HTone()
                  .Left("0px").Top("20px")
                  .Padding(4)
                ),
              new HButton("Все проекты"),
              new HLink(@"https://r.mail.ru/n211655019", "Авто"),
              new HLink(@"https://r.mail.ru/n211655021", "Гороскопы"),
              new HLink(@"https://r.mail.ru/n211655026", "Погода")
            ).Hide(true),
            std.DockFill(),
            //new HPanel(
            //  new HPanel().Size(1200, 10)
            //).Width("100%").WidthLimit("100px", "0").Overflow("hidden"),
            ViewHlp.GetAuthPanel(MailContext.Default.DbConnection, httpContext, state)
          ),
          new HXPanel().HeightLimit("3em", "").Background(Color.FromArgb(22, 141, 226)),
          //ViewHlp.GetButtonPanel(httpContext, state, viewKind).Hide(guest),
          new HXPanel(
            new HPanel(ViewHlp.GetLeftPanel(httpContext, state, viewKind)),
            ViewHlp.GetCenterPanel(httpContext, state, viewKind, displayPage, messageId, out title)
            //new HPanel("left",
            //  ViewHlp.GetLeftGrid(state, viewKind)
            //).Height("40em").Padding(0, 5, 5, 20).Background(Color.WhiteSmoke).Border("2px", "solid", Color.DarkGray, ""),
            //new HPanel("center",
            //  ViewHlp.GetCenterPanel(httpContext, state, viewKind, page, messageId)
            //).Width("100%").WidthLimit("", "0").Background(Color.WhiteSmoke).Border("2px", "solid", Color.DarkGray, "")
          ).HeightLimit("600px", "").Hide(guest),
          new HPanel("test",
            new HSpoiler("SPOILER", new HSpan("Lorem ipsum dolor sit amet, consectetur adipisicing elit. Amet aspernatur dolorem ducimus eaque! Alias assumenda, blanditiis dignissimos facilis fuga ipsam molestias necessitatibus nostrum numquam raesentium quo sed sit! Aut, vitae?"))
          ),
          new HPanel("debug", 
            new HSpan(string.Format("{0}{1}{1}{2}", state.lastJson, Environment.NewLine, state.Authorization))).
            Align(null, null).Border("10px", "solid", Color.Gray, "3px").Background(Color.WhiteSmoke).
            Hide(!state.Authorization),
          std.OperationState(state.Operation).Top("2em").Right("0")
        ).WidthLimit("800px", "").EditContainer("allData");

        StringBuilder css = new StringBuilder();

        //HtmlHlp.AddClassToCss(css, buttonStyle);

        HElement mainElement = mainPanel.ToHtml("main", css);

        if (guest)
          title = "Авторизация - Почта";

        return h.Html
        (
          h.Head(
          ),
          h.Body(
            h.Element("title", title),
            h.LinkCss("font/font.css"),
            h.LinkScript("ckeditor/ckeditor.js"),
            h.Script(@"
               function CK_updateAll()
               {
                 try
                 {
                   for (instance in CKEDITOR.instances)
                     CKEDITOR.instances[instance].updateElement();
                 }
                 catch(ex)
                 {
                   console.log(ex);
                 }
               }
            "),
            h.Css(h.Raw(css.ToString())
            ),
            mainElement
//            h.Script(@"
//               function CK_updateAll()
//               {
//                 for (instance in CKEDITOR.instances)
//                   CKEDITOR.instances[instance].updateElement();
//               }
//            ")

            //h.Script(new HAttribute("data-sample", "1"), @"CKEDITOR.replace( 'editor1' );")
          )
        );
      }
      catch (Exception ex)
      {
        Logger.WriteException(ex);
        throw;
      }
    }
  }

  public enum OperationState
  {
    None,
    Processing,
    Error,
    Completed
  }

  public class MailState
  {
    public bool Authorization = true;
    public string MailText = "Hellow world!";
    public JsonData lastJson = null;
    public readonly WebOperation Operation = new WebOperation();
    public string MessageSendError = "";
  }
}