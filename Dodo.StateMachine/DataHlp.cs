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
  public class DataHlp
  {
    public static List<int> GetSelectedLetterIds(JsonData json, TableLink letterTable)
    {
      List<int> selectedLetterIds = new List<int>();
      foreach (RowLink letter in letterTable.AllRows)
      {
        int letterId = letter.Get(LetterType.Id);
        if (json.GetText(string.Format("check_{0}", letterId)) == "True")
          selectedLetterIds.Add(letterId);
      }
      return selectedLetterIds;
    }

    public static RowLink LoadLetter(IDataLayer dbConnection, int messageId)
    {
      TableLink letterTable = TableLink.LoadTableLink(dbConnection, null,
        new FieldBlank[] { LetterType.Id, LetterType.Sender, LetterType.Recipient,
            LetterType.Time, LetterType.Subject, LetterType.Content,
            LetterType.Read, LetterType.Folder },
        new IndexBlank[] { LetterType.LetterById },
        "", "Select id, sender, recipient, time, subject, content, read, folder From letter",
        "id = @messageId",
        new DbParameter("messageId", messageId)
      );

      if (letterTable.AllRows.Length == 0)
        return null;

      return letterTable.AllRows[0];
    }

    public static Point GetPageInterval(int rowCount, int pageSize, int intervalSize, ref int pageNumber)
    {
      int pageCount = rowCount / pageSize;
      if (rowCount % pageSize != 0)
        pageCount++;
      if (pageNumber < 0 || pageNumber >= pageCount)
        pageNumber = 0;

      int firstPage = pageNumber - (intervalSize / 2);
      int endPage = pageNumber + (intervalSize - 1 - intervalSize / 2);
      if (firstPage < 0)
      {
        endPage -= firstPage;
        firstPage = 0;
      }
      if (endPage >= pageCount)
      {
        firstPage -= (endPage + 1 - pageCount);
        endPage = pageCount - 1;
      }
      if (firstPage < 0)
        firstPage = 0;

      return new Point(firstPage, endPage);
    }


  }
}