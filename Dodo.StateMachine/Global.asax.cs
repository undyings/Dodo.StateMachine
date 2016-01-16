using Commune.Basis;
using Commune.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace Dodo.StateMachine
{
  public class Global : System.Web.HttpApplication
  {
    const string connectionStringFormat = "Data Source={0};Pooling=true;FailIfMissing=false;UseUTF16Encoding=True;";
    protected void Application_Start(object sender, EventArgs e)
    {
      string hostingPath = @"d:\DZHosts\LocalUser\undyings\www.communewui.somee.com";
      string appPath = hostingPath;
      if (!Directory.Exists(hostingPath))
      {
        appPath = ApplicationHlp.CheckAndCreateFolderPath(
          Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
          "Dodo.StateMachine");
      }
      else
      {
        //File.WriteAllText(Path.Combine(
        //  hostingPath, Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)), "test");
      }

      try
      {
        string logFolder = ApplicationHlp.CheckAndCreateFolderPath(appPath, "Logs");

        //Logger.Current = new Log(@"C:\Users\Сергей\Documents\IISExpress\Logs\Dodo.StateMachine\dodo.log", 2000000);
        Logger.EnableLogging(Path.Combine(logFolder, "dodo.log"), 2);

        string databaseFolder = ApplicationHlp.CheckAndCreateFolderPath(appPath, "Data");

        IDataLayer dbConnection = new SQLiteDataLayer(string.Format(
          connectionStringFormat, Path.Combine(databaseFolder, "mail.db3")));

        MailContext.Default.DbConnection = dbConnection;

        DatabaseModel.CheckAndCreateTables(dbConnection);

        Logger.AddMessage("Подключение к базе данных успешно создано");

        if (false)
        {
          string[] lines = File.ReadAllLines(Path.Combine(databaseFolder, "version.txt"),
            Encoding.GetEncoding(1251));
          string subject = "";
          StringBuilder text = new StringBuilder();
          DateTime time = DateTime.MinValue;
          foreach (string line in lines)
          {
            if (line.StartsWith("---"))
            {
              if (time != DateTime.MinValue)
              {
                dbConnection.GetScalar("",
                 "Insert Into letter (sender, recipient, time, subject, content) Values (@sender, @recipient, @time, @subject, @content)",
                 new DbParameter("sender", "second@mail.ru"),
                 new DbParameter("recipient", "test@mail.ru"),
                 new DbParameter("time", time),
                 new DbParameter("subject", subject),
                 new DbParameter("content", text.ToString())
               );
              }

              text.Clear();

              int day;
              int month;
              int year;
              if (int.TryParse(line.Substring(4, 2), out day) &&
                int.TryParse(line.Substring(7, 2), out month) &&
                int.TryParse(line.Substring(10, 4), out year))
              {
                time = new DateTime(year, month, day);
              }

              subject = line.Substring(14);
              continue;
            }

            if (text.Length == 0 && line.Length < 100)
              subject += line;

            text.AppendLine(string.Format("<p>{0}</p>", line));
          }
        }

        //dbConnection.GetScalar("",
        //   "Insert Into account (login, password) Values (@login, @password)",
        //   new DbParameter("login", "test@mail.ru"),
        //   new DbParameter("password", "test")
        // );

        //dbConnection.GetScalar("",
        //    "Insert Into account (login, password) Values (@login, @password)",
        //    new DbParameter("login", "reserve@mail.ru"),
        //    new DbParameter("password", "12345")
        //  );

        //dbConnection.GetScalar("",
        //    "Insert Into account (login, password) Values (@login, @password)",
        //    new DbParameter("login", "second@mail.ru"),
        //    new DbParameter("password", "12345")
        //  );

      }
      catch (Exception ex)
      {
        Logger.WriteException(ex, "Ошибка создания подключения к базе данных:");
      }

      
    }

    protected void Session_Start(object sender, EventArgs e)
    {

    }

    protected void Application_BeginRequest(object sender, EventArgs e)
    {

    }

    protected void Application_AuthenticateRequest(object sender, EventArgs e)
    {
      var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
      if (cookie != null)
      {
        try
        {
          var authTicket = FormsAuthentication.Decrypt(cookie.Value);
          this.Context.SetUserAndCookie(authTicket.Name, false);
        }
        catch
        {

        }
      }
    }

    protected void Application_Error(object sender, EventArgs e)
    {

    }

    protected void Session_End(object sender, EventArgs e)
    {

    }

    protected void Application_End(object sender, EventArgs e)
    {

    }
  }
}