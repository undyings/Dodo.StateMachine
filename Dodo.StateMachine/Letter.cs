using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dodo.StateMachine
{
  public class LetterHlp
  {
    public static Letter[] AllLetters = new Letter[] {
      new Letter(1, "Sergey", "Undying sjfsofjowjfosjgf", "Модель взаимодействия человека и мира", ""),
      new Letter(2, "Mail.ru", "Undying", "Новые события и уведомления", ""),
      new Letter(3, "Chess", "Undying", "Is Wei Ui", "")
    };
  }

  public class Letter
  {
    public readonly int Id;
    public readonly string From;
    public readonly string To;
    public readonly string Subject;
    public readonly string Content;
    public readonly DateTime Time;

    public Letter(int id, string from, string to, string subject, string content)
    {
      this.Id = id;
      this.From = from;
      this.To = to;
      this.Subject = subject;
      this.Content = content;
      this.Time = DateTime.UtcNow;
    }
  }
}