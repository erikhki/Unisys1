using System;
using System.Collections.Generic;
using System.Text;

namespace Unisystem.cs.Modeller;

public class Kurs
{
    public string Kode;
    public string Navn;
    public int MaksPlasser;
    public List<Student> Deltakere = new();
}