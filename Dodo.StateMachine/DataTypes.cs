using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Commune.Data;

namespace Dodo.StateMachine
{
  public class LetterType
  {
    public readonly static FieldBlank<int> Id = new FieldBlank<int>("Id", IntLongConverter.Default);
    public readonly static FieldBlank<string> Sender = new FieldBlank<string>("Sender");
    public readonly static FieldBlank<string> Recipient = new FieldBlank<string>("Recipient");
    public readonly static FieldBlank<DateTime> Time = new FieldBlank<DateTime>("Time");
    public readonly static FieldBlank<string> Subject = new FieldBlank<string>("Subject");
    public readonly static FieldBlank<string> Content = new FieldBlank<string>("Content");
    public readonly static FieldBlank<string> Folder = new FieldBlank<string>("Folder");
    public readonly static FieldBlank<bool> Read = new FieldBlank<bool>("Read", BoolLongConverter.Default);
    public readonly static FieldBlank<string> Filenames = new FieldBlank<string>("Filenames");
    public readonly static FieldBlank<byte[]> Filebytes = new FieldBlank<byte[]>("Filebytes");

    public readonly static SingleIndexBlank LetterById = new SingleIndexBlank("LetterById", Id);
  }
}