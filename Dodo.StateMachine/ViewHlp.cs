using NitroBolt.Wui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetaTech.Library;
using Commune.Basis;
using Commune.Html;
using System.Drawing;
using Commune.Html.Standart;
using Commune.Data;

namespace Dodo.StateMachine
{
  public class ViewHlp
  {
    static readonly HBuilder h = null;

    //public static HButton IconButton(string caption, string fontIcon)
    //{
    //  return std.Button(caption,
    //    new HBefore().FontFamily("sept").Content(fontIcon).VAlign(-2).MarginRight("4px")
    //  );
    //  //return std.Button(caption, new HLabel("",
    //  //  new HBefore().FontFamily("sept").Content(fontIcon).VAlign(-2))
    //  //);
    //}

    public static IHtmlControl MoreComboButton(MailState state, TableLink letterTable)
    {
      return std.ComboButton(
        new HBefore().FontFamily("sept").Content(@"\2261").VAlign(-2).MarginRight("3px").MarginLeft(-1),
        "Ещё", false,
        new HLabel("Отметьте галочками нужные письма").Unselectable().Color(Color.Gray).Padding(4, 8).NoWrap(),
        std.Separator(),
        new HButton("Пометить прочитанным")
          .Event("letter_to_read", "mailListData", delegate(JsonData json)
          {
            List<int> selectedLetterIds = DataHlp.GetSelectedLetterIds(json, letterTable);
            if (selectedLetterIds.Count == 0)
            {
              state.Operation.Error = "Нет выбранных писем";
              return;
            }
            dbConnection.GetScalar("", string.Format(
              "update letter Set read = 1 Where id in ({0})",
              StringHlp.Join(", ", "{0}", selectedLetterIds)
            ));
          }),
        new HButton("Пометить непрочитанным")
          .Event("letter_to_unread", "mailListData", delegate(JsonData json)
          {
            List<int> selectedLetterIds = DataHlp.GetSelectedLetterIds(json, letterTable);
            if (selectedLetterIds.Count == 0)
            {
              state.Operation.Error = "Нет выбранных писем";
              return;
            }
            dbConnection.GetScalar("", string.Format(
              "update letter Set read = 0 Where id in ({0})",
              StringHlp.Join(", ", "{0}", selectedLetterIds)
            ));
          })
      );

      //return new HDropdown2(
      //  false,
      //  new HDropStyle().
      //    DropList(new HTone()
      //      .Right("0px").Top("31px")
      //      .Background(Color.FromArgb(247, 247, 247))
      //      .Border("1px", "solid", Color.FromArgb(224, 224, 224), "2px")
      //      .CssAttribute("border-top", "none")
      //      .Padding(4, 0)
      //    ).
      //    RootWhenDropped(new HTone()
      //      .Background(Color.FromArgb(247, 247, 247))
      //      .Border("1px", "solid", Color.FromArgb(224, 224, 224), "0px")
      //      .CssAttribute("border-bottom", "none")
      //    ).
      //    AnyItem(new HTone().Padding(4, 8)).
      //    SelectedItem(new HTone()
      //      .Color(Color.FromArgb(245, 255, 230))
      //      .Background(Color.FromArgb(99, 99, 99))
      //    ),
      //  new HButton("Выделенные письма",
      //    new HAfter().Content(@"▼").FontSize("60%").MarginLeft(4).MarginRight(-2).VAlign(1),
      //    new HHover().Border("1px", "solid", Color.FromArgb(170, 170, 170), "2px").
      //      LinearGradient("to top right", Color.FromArgb(204, 204, 204), Color.FromArgb(234, 234, 234))
      //  )
      //  .Padding(6, 12)
      //  .LinearGradient("to top right", Color.FromArgb(221, 221, 221), Color.FromArgb(241, 241, 241))
      //  .Border("1px", "solid", Color.FromArgb(187, 187, 187), "2px"),
      //  new HLabel("Отметьте галочками нужные письма", 
      //    new HHover().Background(Color.FromArgb(247, 247, 247)).Color(Color.Gray)
      //  ).Color(Color.Gray),
      //  std.Separator(),
      //  new HButton("Пометить прочитанным"),
      //  new HButton("Пометить непрочитанным")
      //);

      //return new HDropdown(
      //  new HButton("Выделенные письма",
      //    new HAfter().Content(@"▼").FontSize("60%").MarginLeft(4).VAlign(1)).Padding(6, 12),
      //  //std.Button("Выделенные письма",
      //  //  new HAfter().Content(@"▼").FontSize("60%").MarginLeft(4).VAlign(1)
      //  //),
      //  false,
      //  new HLabel("Пометить прочитанным").Padding(4, 8),
      //  new HLabel("Пометить непрочитанным").Padding(4, 8)
      //).Background(Color.FromArgb(247, 247, 247))
      //.Border("1px", "solid", Color.FromArgb(187, 187, 187), "2px")
      //.Padding(4, 0)
      //.CssAttribute("border-top", "none")
      //.Right("0px").Top("30px");
    }

    public static IHtmlControl GetAuthPanel(IDataLayer mailConnection, HttpContext context, MailState state)
    {
      if (context.UserName() == null)
      {
        return std.RowPanel(
          new HTextEdit("loginEdit").Placeholder("Введите логин...").MarginRight(10),
          new HPasswordEdit("passwordEdit").MarginRight(10),
          std.Button("Войти", 0, 12).
            Event("loginUser", "loginData", delegate(JsonData json)
            {
              string login = json.GetText("loginEdit");
              string password = json.GetText("passwordEdit");

              if (StringHlp.IsEmpty(login))
              {
                state.Operation.Error = "Не задан логин";
                return;
              }

              if (StringHlp.IsEmpty(password))
              {
                state.Operation.Error = "Не задан пароль";
                return;
              }

              int count = DatabaseHlp.RowCount(mailConnection, "", "account",
                "login = @login and password = @password",
                new DbParameter("login", login), new DbParameter("password", password));

              if (count == 0)
              {
                state.Operation.Error = "Неверный логин или пароль";
                return;
              }

              context.SetUserAndCookie(login);
            })

        ).EditContainer("loginData").Padding(6);
      }

      return std.RowPanel(
        new HLabel(context.UserName()).MarginRight(10),
        std.Button("Выйти", 0, 12).
          Event("logoutUser", "authData", delegate(JsonData json)
          {
            context.SetUserAndCookie(null);
          })
      ).EditContainer("logoutData").Padding(6);
    }

    public readonly static DisplayName[] AllModes = new DisplayName[] {
      new DisplayName("inbox", "Входящие"), new DisplayName("sent", "Отправленные"), new DisplayName("trash", "Корзина")
    };

    //public static HXPanel GetButtonPanel(HttpContext httpContext, MailState state, string viewKind)
    //{
    //  string user = httpContext.UserName();

    //  if (viewKind == "inbox")
    //  {
    //    return new HXPanel("inboxButtonPanel",
    //      new HLink("/?view=messageNew", ViewHlp.IconButton("Написать письмо", @"\270e")).Color(Color.Black),
    //      new HPanel(
    //        new HPanel().Width("64px")
    //      ),
    //      ViewHlp.IconButton("Удалить", @"\25a9").
    //        //LinearGradient("to bottom", Color.FromArgb(201, 201, 201), Color.FromArgb(241, 241, 241)).
    //        Margin(0, 10).VAlign(null),
    //      new HPanel(new HPanel().Width("10px")),
    //      ViewHlp.IconButton("Спам", @"\2639").VAlign(null),
    //      new HPanel(new HCheckEdit("testCheckEdit", state.Authorization).Margin(0, 10)).VAlign(null),
    //      new HPanel(new HTextEdit("testTextEdit", "").Placeholder("Введите сообщение...")).VAlign(null),
    //      new HPanel(new HComboEdit<string>("testComboEdit", "второй", null, null, "первый", "второй").
    //        Margin(0, 10)).VAlign(null),
    //      new HPanel(
    //        std.Button("Отправить"). //, new HImage(@"/Images/icss_pseudo-class.png")).
    //          ToolTip("Отправить Json на сервер").
    //          Event("send", "testData", delegate (JsonData json)
    //          {
    //            state.Authorization = json.GetData("testCheckEdit").ToString_Fair() == "True";
    //          })
    //      ).VAlign(null),
    //      new HPanel(
    //        new HPanel().Size(1200, 10)
    //      ).Width("100%"), //.WidthLimit("100px", "0").Overflow("hidden"),
    //      new HPanel(std.Button("Вид")).Hide(!state.Authorization)
    //    ).EditContainer("testData").Padding("10px").Background(Color.Wheat);
    //  }

    //  if (viewKind == "messageNew")
    //  {
    //    return std.RowPanel(
    //      std.Button("Сохранить").MarginLeft(250).MarginRight(10).
    //        OnClick("CK_updateAll();").
    //        Event("sendMail", "allData",
    //        delegate(JsonData json)
    //        {
    //          try
    //          {
    //            string recipient = json.GetData("recipientTextEdit").ToString_Fair();
    //            string subject = json.GetData("subjectTextEdit").ToString_Fair();
    //            string mailText = json.GetData("mailText").ToString_Fair();
    //            TraceHlp.AddMessage("Json.SendMail: {0}, {1}, {2}", recipient, subject, mailText);
    //            state.lastJson = json;

    //            Logger.AddMessage("Recipient: '{0}'", recipient);

    //            if (StringHlp.IsEmpty(recipient))
    //            {
    //              state.Operation.Error = "Не заполнено поле 'Кому'";
    //              Logger.AddMessage("Ошибка валидации");
    //              return;
    //            }

    //            object result = MailContext.Default.DbConnection.GetScalar("",
    //              "Insert Into letter (sender, recipient, time, subject, content) Values (@sender, @recipient, @time, @subject, @content)",
    //              new DbParameter("sender", user),
    //              new DbParameter("recipient", recipient),
    //              new DbParameter("time", DateTime.UtcNow),
    //              new DbParameter("subject", subject),
    //              new DbParameter("content", mailText)
    //            );

    //            Logger.AddMessage("Письмо сохранено: {0}", result);

    //            state.Operation.Completed = true;
    //          }
    //          catch (Exception ex)
    //          {
    //            TraceHlp.WriteException(ex);
    //            state.Operation.Error = "Непредвиденная ошибка сохранения в базу данных";
    //          }
    //        }),
    //      new HLink(@"/?view=inbox", std.Button("Отмена")).Color(Color.Black)
    //    );
    //  }

    // return new HXPanel(new HSpan(viewKind));
    //}

    public static IHtmlControl GetLeftPanel(HttpContext httpContext, MailState state, string viewKind)
    {
      string user = httpContext.UserName();

      return new HPanel(
        std.RowPanel(
          new HLink("/?view=messageNew", SeptStd.IconButton("Написать письмо", @"\270e")).MarginLeft(10).Color(Color.Black)
        ).Padding("10px"),
        new HPanel(
          new HTableGrid("viewKind",
            new HRowStyle()
              .AnyRow(new HTone().Padding("4px"))
              .ALink(delegate(DisplayName mode)
              {
                return string.Format("/?view={0}", mode.Name);
              }),
            new HColumn<DisplayName>("name", delegate(DisplayName mode)
              {
                HLabel cell = new HLabel(mode.Display).FontBold(mode.Name == viewKind).Width("120px");
                return cell;
              }
            ),
            new HColumn<DisplayName>("count", delegate(DisplayName mode)
              {
                if (mode.Name == "inbox")
                {
                  int count = DatabaseHlp.RowCount(dbConnection, "",
                    "letter", "recipient = @address and folder is null and read = 0",
                    new DbParameter("address", user));
                  return new HLabel(count.ToString()).FontBold(mode.Name == viewKind).Align(false).Width("60px");
                }
                return new HLabel("");
              }
            )
          ).Rows(AllModes).Padding(5, 10).Border("2px", "solid", Color.LightGray, "")
        ).Padding(0, 5, 5, 20)
      );
    }

    static IDataLayer dbConnection
    {
      get
      {
        return MailContext.Default.DbConnection;
      }
    }

    public static IHtmlControl GetCenterPanel(HttpContext httpContext, MailState state, 
      string viewKind, int? displayPage, int? messageId, out string title)
    {
      string user = httpContext.UserName();

      title = string.Format(" - {0} - Почта", user);

      if (viewKind == "inbox" || viewKind == "sent" || viewKind == "trash")
      {
        if (viewKind == "inbox")
          title = "Входящие" + title;
        else if (viewKind == "sent")
          title = "Отправленные" + title;
        else
          title = "Корзина" + title;

        int pageNumber = displayPage != null ? displayPage.Value - 1 : 0;
 
        int limit = 15;

        bool isSent = viewKind == "sent";

        string condition = "recipient = @address and folder is null";
        if (viewKind == "sent")
        {
          condition = "sender = @address and folder is null";
        }
        if (viewKind == "trash")
        {
          condition = "(recipient = @address or sender = @address) and folder is not null";
        }

        int letterCount = DatabaseHlp.RowCount(dbConnection, "", "letter", condition, new DbParameter("address", user));

        Point pageInterval = DataHlp.GetPageInterval(letterCount, limit, 5, ref pageNumber);

        TableLink letterTable = TableLink.LoadTableLink(MailContext.Default.DbConnection, null,
          new FieldBlank[] { LetterType.Id, LetterType.Sender, LetterType.Recipient,
            LetterType.Time, LetterType.Subject, LetterType.Read },
          new IndexBlank[] { LetterType.LetterById },
          "", "Select id, sender, recipient, time, subject, read From letter",
          string.Format("{0} order by time desc limit {1} offset {2}", condition, limit, pageNumber * limit),
          new DbParameter("address", user)
        );

        List<IHtmlControl> paginatorControls = new List<IHtmlControl>(7);
        for (int i = pageInterval.X; i <= pageInterval.Y; ++i)
        {
          if (i == pageNumber)
          {
            paginatorControls.Add(
              new HSpan((i + 1).ToString()).Padding(6, 8).Background("#ededed").Display("inline-block")
            );
            continue;
          }
          paginatorControls.Add(
            new HLink(string.Format("/?view={0}&page={1}", viewKind, i + 1), (i + 1).ToString())
              .Padding(6, 8).Display("inline-block").Color(Color.Black).CssAttribute("text-decoration", "none")
          );
        }

        ICollection<UniversalKey> keys = letterTable.KeysForIndex(LetterType.LetterById);

        return new HPanel(
          std.RowPanel(
             SeptStd.IconButton("Удалить", @"\25a9").MarginRight(10)
               .Event("delete_letter", "mailListData", delegate(JsonData json)
               {
                 List<int> selectedIds = DataHlp.GetSelectedLetterIds(json, letterTable);
                 if (selectedIds.Count == 0)
                 {
                   state.Operation.Error = "Нет выбранных писем";
                   return;
                 }
                 dbConnection.GetScalar("", string.Format(
                   "delete from letter Where id in ({0})",
                   StringHlp.Join(", ", "{0}", selectedIds)
                 ));
               }),
             SeptStd.IconButton(viewKind == "trash" ? "Восстановить" : "Корзина", @"\2639").MarginRight(10)
               .Event("move_letter_to_trash", "mailListData", delegate(JsonData json)
               {
                 List<int> selectedIds = DataHlp.GetSelectedLetterIds(json, letterTable);
                 if (selectedIds.Count == 0)
                 {
                   state.Operation.Error = "Нет выбранных писем";
                   return;
                 }
                 dbConnection.GetScalar("", string.Format(
                   "update letter Set folder = {0} Where id in ({1})",
                   viewKind != "trash" ? "'trash'" : "null",
                   StringHlp.Join(", ", "{0}", selectedIds)
                 ));
               }),
          //OnClick("$('input:checkbox').removeAttr('checked');"),
             ViewHlp.MoreComboButton(state, letterTable),
             std.DockFill(),
             new HPanel(paginatorControls.ToArray()).Border("1px", "solid", "#bbb", "2px").VAlign(null).NoWrap(),
             new HPanel().Width("30px"),
             std.Button("Вид")
             .OnClick("alert(navigator.userAgent);")
          //.OnClick(string.Format(
          //  "$('.{0}').is(':checked') ? $('.{0}').prop('checked', false) : $('.{0}').prop('checked', true);",
          //  "check_280"))
          ).Padding(10, 5),
          new HPanel(
            new HTableGrid("letter",
              new HRowStyle()
                .AnyRow(new HTone().Padding("4px"))
                .ALink(delegate(RowLink letter)
                {
                  return string.Format("/?view=messageView&id={0}", letter.Get(LetterType.Id));
                })
                .Hover(new HTone().Background(Color.Wheat))
                .Even(new HTone().Background(Color.WhiteSmoke))
                .AnyCell(delegate(IHtmlColumn column, RowLink letter, IHtmlControl cell)
                {
                  cell.Padding(5);
                  if (column.Name != "select")
                    cell.FontBold(!letter.Get(LetterType.Read));
                  return cell;
                  //return new HLink(
                  //  string.Format("/?view=messageView&id={0}", letter.Get(LetterType.Id)),
                  //  cell.FontBold(!letter.Get(LetterType.Read))
                  //);

                }),
          //.AnyCell(new HTone().Padding(5)),
              new HColumn<RowLink>("select", delegate(RowLink letter)
                {
                  string dataName = string.Format("check_{0}", letter.Get(LetterType.Id));
                  return SeptStd.CheckEdit(dataName, false).MarginRight(5).Color(Color.Black);
                  //return new HPanel(
                  //  new HCheckEdit(string.Format("check_{0}", letter.Get(LetterType.Id)), false)
                  //    .OnClick(string.Format(
                  //      "e.preventDefault(); $('.{0}').is(':checked') ? $('.{0}').prop('checked', false) : $('.{0}').prop('checked', true);",
                  //      "check_280"))
                  //).Width("40px").Padding(5).VAlign(null);
                  //).OnClick("e.preventDefault();");
                  //.OnClick("e.stopPropagation();").VAlign(null);
                  //).TagAttribute("onclick", "e.preventDefault();");
                  //return new HCheckEdit(dataName, false)
                  //  .Width("40px").Padding(5);
                  //.OnClick("return false;");
                  //  .OnClick(string.Format(
                  //    "e.preventDefault(); $('#{0}').is(':checked') ? $('#{0}').prop('checked', false) : $('#{0}').prop('checked', true);",
                  //    dataName)
                  //  );
                }
              ), //.Width("40px"),
              new HColumn<RowLink>("from", delegate(RowLink letter)
                {
                  return new HLabel(letter.Get(LetterType.Sender))
                    .CssAttribute("word-wrap", "break-word")
                    .Width("150px");
                }
              ).Hide(isSent),
              new HColumn<RowLink>("to", delegate(RowLink letter)
                {
                  return new HLabel(letter.Get(LetterType.Recipient))
                    .CssAttribute("word-wrap", "break-word")
                    .Width("150px");
                }
              ).Hide(!isSent),
              new HColumn<RowLink>("subject", delegate(RowLink letter)
                {
                  //return std.DockFill(
                  //  new HLabel(letter.Get(LetterType.Subject))
                  //    .FontBold(!letter.Get(LetterType.Read))
                  //);
                  //return new HLabel(letter.Get(LetterType.Subject))
                  //  //.NoWrap()
                  //  //.Overflow()
                  //  .CssAttribute("white-space", "normal")
                  //  .CssAttribute("word-wrap", "break-word")
                  //  //.Width("400px")
                  //  .Width("calc(100% - 330px)")
                  //  .FontBold(!letter.Get(LetterType.Read));
                  //return new HPanel(new HLabel(letter.Get(LetterType.Subject))
                  //  .FontBold(!letter.Get(LetterType.Read))
                  //  //.Width("400px")
                  //  //.WidthFill()
                  //  .CssAttribute("white-space", "normal")
                  //  .CssAttribute("word-wrap", "break-word")
                  //).Width("calc(100% - 290px)");
                  //return new HPanel(new HLabel(letter.Get(LetterType.Subject))
                  //  .FontBold(!letter.Get(LetterType.Read))).Width("100%").WidthLimit("", "0")
                  //  .CssAttribute("white-space", "normal")
                  //  .CssAttribute("word-break", "break-all");
                  return new HLabel(letter.Get(LetterType.Subject))
                    .Width("100%");
                }
              ).WidthFill(),
              new HColumn<RowLink>("date", delegate(RowLink letter)
                {
                  return new HLabel(letter.Get(LetterType.Time).ToString("dd.MM.yyyy"))
                    .Width("100px").Align(false);
                }
              )
            ).Rows(letterTable.AllRows)
          ).MarginLeft(5).Border("2px", "solid", Color.LightGray, "")
        ).EditContainer("mailListData").Width("100%"); //.WidthLimit("", "0");
      }

      if (viewKind == "messageView")
      {
        title = "Просмотр письма" + title;

        if (state.Operation.Completed)
          return GetInfoMessage(state.Operation.Error);

        if (messageId == null)
          return GetInfoMessage("Неверный формат запроса");

        RowLink letter = DataHlp.LoadLetter(dbConnection, messageId.Value);

        if (letter == null)
          return GetInfoMessage("Сообщение не найдено");

        if (letter.Get(LetterType.Sender) != user && letter.Get(LetterType.Recipient) != user)
          return GetInfoMessage("Вы не имеете прав на просмотр этого сообщения");

        if (!letter.Get(LetterType.Read))
        {
          dbConnection.GetScalar("", "update letter Set read = 1 Where id = @messageId",
            new DbParameter("messageId", messageId.Value));
        }

        return new HPanel(
          std.RowPanel(
            new HLink(string.Format(@"/?view=messageNew&id={0}", messageId.Value),
              SeptStd.IconButton("Ответить", @"\21fd").MarginRight(10)
            ).Color(Color.Black),
            SeptStd.IconButton("Удалить", @"\25a9").MarginRight(10)
              .Event("delete_letter", "mailListData", delegate (JsonData json)
              {
                dbConnection.GetScalar("", "delete from letter Where id = @messageId",
                  new DbParameter("messageId", messageId.Value)
                );

                state.Operation.Error = "Сообщение успешно удалено";
                state.Operation.Completed = true;
              }),
            SeptStd.IconButton(viewKind == "trash" ? "Восстановить" : "Корзина", @"\2639").MarginRight(10)
              .Event("move_letter_to_trash", "mailListData", delegate (JsonData json)
              {
                dbConnection.GetScalar("", string.Format(
                  "update letter Set folder = {0} Where id = @messageId",
                  letter.Get(LetterType.Folder) != "trash" ? "'trash'" : "null"),
                  new DbParameter("messageId", messageId.Value)
                );

                state.Operation.Error = "Сообщение перемещено в корзину";
                state.Operation.Completed = true;
              }),
            std.DockFill(),
            std.Button("Вид")
          ).Padding(10, 5),
          new HPanel(
            std.RowPanel(
              new HPanel(new HPanel().Size(90, 90).
                Border("2px", "solid", Color.DarkGray, "50%").Background(Color.LightGray)).VAlign(null),
              new HPanel(
                new HPanel(new HLabel(letter.Get(LetterType.Subject)).FontSize("125%").FontBold(true)).Padding(5),
                new HPanel(new HLabel(letter.Get(LetterType.Sender)).FontBold(true)).Padding(5),
                new HPanel(
                  new HLabel(string.Format("Кому: {0}", letter.Get(LetterType.Recipient))).Color(Color.Gray)
                ).Padding(5),
                new HPanel(
                  new HLabel(letter.Get(LetterType.Time).ToString("dd MMMM yyyy hh:mm")).Color(Color.Gray)
                ).Padding(5)
              ).Padding(15)
            ).CssAttribute("border-bottom", "1px solid #E6E6E6"),
            new HPanel(new HTextView(letter.Get(LetterType.Content))).Overflow()
          ).Border("2px", "solid", Color.LightGray, "2px").MarginLeft(5).Padding(0, 15)
        ).Width("100%").WidthLimit("", "0");
      }

      if (viewKind == "messageNew")
      {
        title = "Новое письмо" + title;

        if (state.Operation.Completed)
          return GetInfoMessage("Письмо успешно отправлено");

        string letterRecipient = "";
        string letterSubject = "";
        string letterText = "";
        if (messageId != null)
        {
          RowLink letter = DataHlp.LoadLetter(dbConnection, messageId.Value);
          if (letter != null && letter.Get(LetterType.Sender) != user && letter.Get(LetterType.Recipient) != user)
            letter = null;

          if (letter != null)
          {
            letterRecipient = letter.Get(LetterType.Sender);
            letterSubject = string.Format("Re: {0}", letter.Get(LetterType.Subject));
            letterText = string.Format("<blockquote>{0}</blockquote>", letter.Get(LetterType.Content));
          }
        }

        return new HPanel(
          std.RowPanel(
            std.Button("Отправить").MarginRight(10).
              OnClick("CK_updateAll();").
              Event("sendMail", "mailData",
              delegate(JsonData json)
              {
                try
                {
                  string recipient = json.GetText("recipientTextEdit");
                  string subject = HtmlHlp.BreakLongestWords(json.GetText("subjectTextEdit"));
                  string mailText = json.GetText("mailText");
                  TraceHlp.AddMessage("Json.SendMail: {0}, {1}, {2}", recipient, subject, mailText);
                  state.lastJson = json;

                  Logger.AddMessage("Recipient: '{0}'", recipient);

                  if (StringHlp.IsEmpty(recipient))
                  {
                    state.Operation.Error = "Не заполнено поле 'Кому'";
                    Logger.AddMessage("Ошибка валидации");
                    return;
                  }

                  object result = MailContext.Default.DbConnection.GetScalar("",
                    "Insert Into letter (sender, recipient, time, subject, content) Values (@sender, @recipient, @time, @subject, @content)",
                    new DbParameter("sender", user),
                    new DbParameter("recipient", recipient),
                    new DbParameter("time", DateTime.UtcNow),
                    new DbParameter("subject", subject),
                    new DbParameter("content", mailText)
                  );

                  Logger.AddMessage("Письмо сохранено: {0}", result);

                  state.Operation.Completed = true;
                }
                catch (Exception ex)
                {
                  TraceHlp.WriteException(ex);
                  state.Operation.Error = "Непредвиденная ошибка сохранения в базу данных";
                }
              }),
            new HLink(@"/?view=inbox", std.Button("Отмена")).Color(Color.Black)
          ).Padding(10, 5),
          new HPanel("centerMessage",
            std.RowPanel(
              new HLabel("Кому").Overflow().MarginLeft("10px").Width("50px"),
              std.DockFill(new HTextEdit("recipientTextEdit", letterRecipient))
            ).Padding("5px"),
            std.RowPanel(
              new HLabel("Тема").MarginLeft("10px").Width("50px"),
              std.DockFill(new HTextEdit("subjectTextEdit", letterSubject))
            ).Padding(0, 5, 5, 5),
            //new HXPanel(
            //  new HPanel(new HLabel("Кому").Overflow().MarginLeft("10px").Width("50px")),
            //  new HPanel(new HLabel("Кому").Overflow().MarginLeft("10px").Width("50px")),
            //  new HPanel(new HTextEdit("recipientTextEdit", "").Width("100%")).WidthFill()
            //).Padding("5px"),
            //new HXPanel(
            //  new HPanel(new HLabel("Тема").MarginLeft("10px").Width("50px")),
            //  new HPanel(new HLabel("Тема").MarginLeft("10px").Width("50px")),
            //  new HPanel(new HTextEdit("subjectTextEdit", "").Width("100%")).WidthFill()
            //).Padding(0, 5, 5, 5),
            new HPanel(
              new HElementControl(
                h.TextArea(
                  new HAttribute("id", "editor1"),
                  h.@class("ckeditor"),
              //new HAttribute("name", "editor1"),
              //new HAttribute("data-sample", "1"),
                  h.data("name", "mailText"),
                  h.Attribute("js-init", "CKEDITOR.replace(this[0], { height: '400px', language: 'ru' })"),
                  letterText
                ), "ckeditor"
              )
            ).Padding(5)
          ).MarginLeft(5).Border("2px", "solid", Color.LightGray, "")
        ).EditContainer("mailData");
      }

      return GetInfoMessage(viewKind);
    }

    public static IHtmlControl GetInfoMessage(string message)
    {
      return new HPanel(
        new HPanel(new HLabel("Stub").Padding(7, 12).Color(Color.Transparent)).Padding(10),
        new HPanel(
          new HLabel(message).FontSize("150%")
        ).Padding(5, 10).Border("2px", "solid", Color.LightGray, "2px").HeightLimit("600px", "")
      ).Width("100%"); //.WidthLimit("", "0");
    }

  //  public static HElement AuthorizationPanel(HttpContext context, Dictionary<string, string> passwordByLogin)
  //  {
  //    bool isAuth = context.User != null && context.User.Identity.IsAuthenticated;

  //    if (isAuth)
  //    {
  //      string userName = context.User.Identity.Name;
  //      return h.Table(
  //        h.TBody(
  //          h.Tr(
  //            h.Td(
  //              h.Span(context.User.Identity.Name)
  //            ),
  //            h.Td(
  //              h.Input(h.type("button"), h.value("Выйти"), h.onclick(";"),
  //                h.eventh("logout", delegate
  //                {                  
  //                  context.SetUserAndCookie(null);
  //                  Logger.AddMessage("Logout: {0}", context.User != null);
  //                })
  //              )
  //            )
  //          )
  //        )
  //      );
  //    }

  //    return h.Table(
  //      h.data("name", "login-box"),
  //      h.TBody(
  //        h.Tr(
  //          h.Td(
  //            h.Input(h.type("text"), h.data("name", "login"))
  //          ),
  //          h.Td(
  //            h.Input(h.type("password"), h.data("name", "password"))
  //          ),
  //          h.Td(
  //            h.Input(h.type("button"), h.value("Войти"), h.onclick(";"),
  //              h.eventh("login", "container", "login-box", delegate(string container, JsonData json)
  //              {
  //                string login = json.JPath("data", "login").ToString_Fair();
  //                string password = json.JPath("data", "password").ToString_Fair();
  //                Logger.AddMessage("Login: {0}, {1}", login, password);
  //                if (passwordByLogin.Find(login) == password)
  //                  context.SetUserAndCookie(login);
  //                else
  //                {
  //                  Logger.AddMessage("Ошибочный логин или пароль: {0}, {1}", login, password);
  //                  //display error
  //                }
  //              })
  //            )
  //          )
  //        )
  //      )
  //    );
  //  }
  }
}