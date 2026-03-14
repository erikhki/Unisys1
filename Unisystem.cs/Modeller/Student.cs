using System;
using System.Collections.Generic;
using System.Text;

namespace Unisystem.cs.Modeller;

public class Student
{
    public string StudentId;
    public string Navn;
    public string Epost;
    public List<Kurs> KursListe = new();
}
