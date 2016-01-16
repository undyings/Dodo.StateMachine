using Commune.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dodo.StateMachine
{
  public class DatabaseModel
  {
    public static void CheckAndCreateTables(IDataLayer mailConnection)
    {
      if (!SQLiteHlp.TableExist(mailConnection, "", "account"))
      {
        mailConnection.GetScalar("",
          @"CREATE TABLE account (
              id        integer PRIMARY KEY AUTOINCREMENT NOT NULL,
              login     varchar(50) NOT NULL UNIQUE,
              password  varchar(50)
            );

            CREATE UNIQUE INDEX account_login_index
              ON account
              (login);
          "
        );
      }

      if (!SQLiteHlp.TableExist(mailConnection, "", "letter"))
      {
        mailConnection.GetScalar("",
          @"
            CREATE TABLE letter (
              id         integer PRIMARY KEY AUTOINCREMENT NOT NULL,
              sender     varchar(50),
              recipient  varchar(50),
              time       datetime NOT NULL,
              subject    varchar(50),
              content    text,
              folder     varchar(25),
              status     boolean NOT NULL DEFAULT false,
              filenames  text,
              filebytes  blob,
              read       integer NOT NULL DEFAULT 0
            );

            CREATE INDEX letter_recipient_folder_read_time_index
              ON letter
              (recipient, folder, read, time);

            CREATE INDEX letter_recipient_folder_time_index
              ON letter
              (recipient, folder, time);

            CREATE INDEX letter_sender_folder_time_index
              ON letter
              (sender, folder, time);
            "
         );
      }
    }
  }
}