using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Unisystem.cs.Modeller;

List<Student> studenter = new();
List<Ansatt> ansatte = new();
List<Kurs> kursliste = new();
List<Bok> bøker = new();
List<Lån> lånListe = new();
int nesteLånId = 1;

SeedData();
//Enkel meny for å navigere i systemet. Bruker switch-case for å håndtere brukerens valg.
bool kjører = true;
while (kjører)
{
    Console.WriteLine("\nMeny");
    Console.WriteLine("[1] Opprett kurs");
    Console.WriteLine("[2] Meld student til kurs");
    Console.WriteLine("[3] Vis kurs og deltakere");
    Console.WriteLine("[4] Søk etter kurs");
    Console.WriteLine("[5] Søk etter bok");
    Console.WriteLine("[6] Lån bok");
    Console.WriteLine("[7] Returner bok");
    Console.WriteLine("[8] Registrer bok");
    Console.WriteLine("[9] Avslutt");
    Console.Write("Valg: ");
    string valg = Console.ReadLine();

    switch (valg) //Håndterer brukerens valg og kaller de respektive metodene for hver funksjonalitet.
    {
        case "1": OpprettKurs(); break;
        case "2": MeldStudentTilKurs(); break;
        case "3": VisKurs(); break;
        case "4": SøkKurs(); break;
        case "5": SøkBok(); break;
        case "6": LånBok(); break;
        case "7": ReturnerBok(); break;
        case "8": RegistrerBok(); break;
        case "0": kjører = false; break;
        default: Console.WriteLine("Ugyldig Valg."); break;

    }


}
void OpprettKurs() //Lar brukeren opprette et nytt kurs ved å angi kurskode, navn og maks plasser.
{
    Console.Write("Kurskode: ");
    string kode = Console.ReadLine();

    Console.Write("Navn: ");
    string navn = Console.ReadLine();

    Console.Write("Maks plasser: ");
    int maks = int.Parse(Console.ReadLine());

    kursliste.Add(new Kurs { Kode = kode, Navn = navn, MaksPlasser = maks });
    Console.WriteLine("Kurs opprettet");
}
void MeldStudentTilKurs() // Lar brukeren enkelt melde studenten til et kurs ved å angi student-ID og kurskode.
{
    Console.Write("StudentID: ");
    string id = Console.ReadLine();

    var student = studenter.FirstOrDefault(s => s.StudentId == id);
    if (student == null)
    {
        Console.WriteLine("Fant ikke student.");
        return;
    }
    Console.Write("Kurskode: ");
    string kode = Console.ReadLine();

    var kurs = kursliste.FirstOrDefault(k => k.Kode == kode);
    if (kurs == null)
    {
        Console.WriteLine("Fant ikke kurs."); // sjekker om kurset eksisterer, meldingen vises hvis det ikke gjør det.
        return;
    }
    if (kurs.Deltakere.Count > kurs.MaksPlasser)
    {
        Console.WriteLine("Kurset er fullt."); // legger til en sperre for å forhindre at plassene overskrides.
        return;
    }
    kurs.Deltakere.Add(student);
    student.KursListe.Add(kurs);

    Console.WriteLine("Student meldt på kurs."); // bekrefter at studenten er meldt på kurset.
}

void VisKurs() //oversikt over alle kursene og deres deltakere, inkludert hvor mange plasser som er fylt opp i forhold til maks plasser.
{
    foreach (var k in kursliste)
    {
        Console.WriteLine($"{k.Kode} - {k.Navn} - ({k.Deltakere.Count}/{k.MaksPlasser})");
        foreach (var s in k.Deltakere)
            Console.WriteLine(" - " + s.Navn);
    }
}
void SøkKurs() // Lar brukeren søke etter kurs ved å angi en del av kurskoden eller navnet, og viser alle kurs som matcher søket.
{
    Console.Write("Søk: ");
    string søk = Console.ReadLine().ToLower();

    var resultater = kursliste.Where(k => k.Kode.ToLower().Contains(søk) || k.Navn.ToLower().Contains(søk));

    foreach (var k in resultater)
        Console.WriteLine($"{k.Kode} - {k.Navn}");
}
void SøkBok()
{
    Console.Write("Søk: ");
    string søk = Console.ReadLine().ToLower();

    var resultater = bøker.Where(b =>
        b.Tittel.ToLower().Contains(søk) ||
        b.Forfatter.ToLower().Contains(søk));

    foreach (var b in resultater)
    {
        Console.WriteLine($"{b.Id}: {b.Tittel} ({b.Antall} eksemplarer)");

        var aktive = lånListe.Where(l => l.Bok.Id == b.Id && l.Aktiv);
        var historikk = lånListe.Where(l => l.Bok.Id == b.Id && !l.Aktiv);

        Console.WriteLine("  Aktive lån:");
        foreach (var l in aktive)
            Console.WriteLine($"   - lånt av {l.Låntaker} {l.Lånedato:d}");

        Console.WriteLine("  Historikk:");
        foreach (var l in historikk)
            Console.WriteLine($"   - returnert {l.ReturnertDato:d} av {l.Låntaker}");
    }
}
void LånBok()
{
    Console.Write("E-post: ");
    string epost = Console.ReadLine();

    var bruker = (object)studenter.FirstOrDefault(s => s.Epost == epost)
        ?? ansatte.FirstOrDefault(a => a.Epost == epost);

    if (bruker == null)
    {
        Console.WriteLine("Fant ikke bruker.");
        return;
    }

    Console.Write("Bok-ID: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Ugyldig ID.");
        return;
    }

    var bok = bøker.FirstOrDefault(b => b.Id == id);
    if (bok == null)
    {
        Console.WriteLine("Fant ikke bok.");
        return;
    }

    int aktive = lånListe.Count(l => l.Bok.Id == id && l.Aktiv);

    if (aktive >= bok.Antall)
    {
        Console.WriteLine("Ingen tilgjengelige eksemplarer.");
        return;
    }

    var nyttLån = new Lån(bok, bruker);
    lånListe.Add(nyttLån);

    Console.WriteLine("Lån registrert.");

    Console.WriteLine("\nAktive lån:");
    foreach (var lån in lånListe.Where(l => l.Aktiv))
        Console.WriteLine($"- {lån.Låntaker}: \"{lån.Bok.Tittel}\" (lånt {lån.Lånedato:d})");
}

void ReturnerBok()
{
    var aktiveLån = lånListe.Where(l => l.Aktiv).ToList();

    if (aktiveLån.Count == 0)
    {
        Console.WriteLine("Ingen aktive lån.");
        return;
    }

    Console.WriteLine("Aktive lån:");
    for (int i = 0; i < aktiveLån.Count; i++)
    {
        var lån = aktiveLån[i];
        Console.WriteLine($"{i + 1}) {lån.Låntaker} – \"{lån.Bok.Tittel}\" (lånt {lån.Lånedato:d})");
    }

    Console.Write("Velg lån som skal returneres: ");
    if (!int.TryParse(Console.ReadLine(), out int valg) || valg < 1 || valg > aktiveLån.Count)
    {
        Console.WriteLine("Ugyldig valg.");
        return;
    }

    var valgtLån = aktiveLån[valg - 1];
    valgtLån.Returner();

    Console.WriteLine($"Boken \"{valgtLån.Bok.Tittel}\" er nå returnert av {valgtLån.Låntaker}.");
}
void RegistrerBok() // lar brukeren registrere en ny bok i biblioteket ved å oppgi ID, tittel, forfatter og antall eksemplarer.
{
    Console.Write("ID: "); //brukeren må oppgi ID for den nye boken.
    int id = int.Parse(Console.ReadLine());

    Console.Write("Tittel: "); //brukeren må oppgi tittel for den nye boken.
    string tittel = Console.ReadLine();

    Console.Write("Forfatter: "); //brukeren må oppgi forfatter for den nye boken.
    string forfatter = Console.ReadLine();

    Console.Write("Antall eksemplarer: "); //brukeren må oppgi antall eksemplarer for den nye boken.
    int antall = int.Parse(Console.ReadLine());

    bøker.Add(new Bok { Id = id, Tittel = tittel, Forfatter = forfatter, Antall = antall });

    Console.WriteLine("Bok registrert."); //bekrefter at den nye boken er registrert i systemet.
}
void SeedData() //funksjon for å fylle systemet med studenter
{
    studenter.Add(new Student
    {
        StudentId = "S1",
        Navn = "Erik Olsen",
        Epost = "erik@uia.no",
        KursListe = new List<Kurs> { new Kurs { Kode = "KJ101", Navn = "Introduksjon til programmering", MaksPlasser = 30 } }
    });
    studenter.Add(new Student
    {
        StudentId = "S3",
        Navn = "Sindre Kvame",
        Epost = "sindre@uia.no",
        KursListe = new List<Kurs> { new Kurs { Kode = "IS105", Navn = "Datasystemer og systemarkitektur", MaksPlasser = 20 } }
    });
    studenter.Add(new Student
    {
        StudentId = "S4",
        Navn = "Benjamin Baxter",
        Epost = "benjamin@uia.no",
        KursListe = new List<Kurs> { new Kurs { Kode = "IS105", Navn = "Datasystemer og systemarkitektur", MaksPlasser = 20 } }
    });
    studenter.Add(new Student
    {
        StudentId = "S5",
        Navn = "Girthan Kumarasamy",
        Epost = "girthan@uia.no",
        KursListe = new List<Kurs> { new Kurs { Kode = "IS118", Navn = "Programmering og prosjektsamarbeid", MaksPlasser = 25 } }
    });
    studenter.Add(new Student
    {
        StudentId = "S6",
        Navn = "Mathias Struck",
        Epost = "mathias@gmail.com",
        KursListe = new List<Kurs> { new Kurs { Kode = "IS118", Navn = "Programmering og prosjektsamarbeid", MaksPlasser = 25 } }
    });
    studenter.Add(new Student
    {
        StudentId = "S7",
        Navn = "Ulrik Hovden",
        Epost = "ulrik@hotmail.com",
        KursListe = new List<Kurs> { new Kurs { Kode = "IS100", Navn = "The role og digitalization within society", MaksPlasser = 15 } }
    });
    studenter.Add(new Student
    {
        StudentId = "S8",
        Navn = "Vegard Hansen",
        Epost = "vegard@outlook.com",
        KursListe = new List<Kurs> { new Kurs { Kode = "KJ101", Navn = "Introduksjon til programmering", MaksPlasser = 30 } }
    });
    studenter.Add(new Student
    {
        StudentId = "S12",
        Navn = "Jesper Berg",
        Epost = "JesperB@uia.no",
        KursListe = new List<Kurs> { new Kurs { Kode = "KJ101", Navn = "Introduksjon til programmering", MaksPlasser = 30 } }
    });


    studenter.Add(new Utvekslingsstudenter //funksjon for å legge til utvekslingsstudenter i systemt.
    {
        StudentId = "S2",
        Navn = "Hjalmar Svensson",
        Epost = "Hjalmar@uia.no",
        HjemUniversitet = "Malmo University",
        Periode = "2026-2027"
    });
    studenter.Add(new Utvekslingsstudenter
    {
        StudentId = "S9",
        Navn = "Lina Müller",
        Epost = "LinaM@uia.no",
        HjemUniversitet = "University of Vienna",
        Periode = "2026-2027",
    });
    studenter.Add(new Utvekslingsstudenter
    {
        StudentId = "S10",
        Navn = "Kenneth Larsen",
        Epost = "KennethL@uia.no",
        HjemUniversitet = "University of Copenhagen",
    });
    studenter.Add(new Utvekslingsstudenter
    {
        StudentId = "S11",
        Navn = "Harry Smith",
        Epost = "HarryS@uia.no",
        HjemUniversitet = "University of Edinburgh",
    });
    ansatte.Add(new Ansatt //funksjon for å legge til ansatte i systemet.
    {
        AnsattId = "A1",
        Navn = "Eva Jensen",
        Epost = "EvaJ@uia.no",
        Stilling = "Foreleser",
        Avdeling = "Økonomi",
    });
    ansatte.Add(new Ansatt
    {
        AnsattId = "A2",
        Navn = "Lars Pettersen",
        Epost = "LarsP@uia.no",
        Stilling = "Rådgiver",
        Avdeling = "Finans",
    });
    ansatte.Add(new Ansatt
    {
        AnsattId = "A3",
        Navn = "Sofia Berg",
        Epost = "SofiaB@uia.no",
        Stilling = "Forskning",
        Avdeling = "Teknologi",
    });
    ansatte.Add(new Ansatt
    {
        AnsattId = "A4",
        Navn = "Marius Nilsen",
        Epost = "MariusN@uia.no",
        Stilling = "Administrasjon",
        Avdeling = "HR",
    });
    ansatte.Add(new Ansatt
    {
        AnsattId = "A5",
        Navn = "Ingrid Larsen",
        Epost = "IngridL@uia.no",
        Stilling = "Foreleser",
        Avdeling = "Data Science",
    });

    //funksjon for å legge til bøker i biblioteket, inkludert ID, tittel, forfatter og antall eksemplarer.
    bøker.Add(new Bok { Id = 1, Tittel = "C# For object oriented programming", Forfatter = "Rafael Sanders and Miguel Farmer", Antall = 3 });
    bøker.Add(new Bok { Id = 2, Tittel = "Hodejegerne", Forfatter = "Jo Nesbø", Antall = 2 });
    bøker.Add(new Bok { Id = 3, Tittel = "Sult", Forfatter = "Knut Hamsun", Antall = 1 });
    bøker.Add(new Bok { Id = 4, Tittel = "Ringenes Herre", Forfatter = "J.R.R. Tolkien", Antall = 4 });
    bøker.Add(new Bok { Id = 5, Tittel = "Panserhjerte", Forfatter = "Jo Nesbø", Antall = 5 });
    bøker.Add(new Bok { Id = 6, Tittel = "Politi", Forfatter = "Jo Nesbø", Antall = 2 });
    bøker.Add(new Bok { Id = 7, Tittel = "Den store Gatsby", Forfatter = "F. Scott Fitzgerald", Antall = 3 });
    bøker.Add(new Bok { Id = 8, Tittel = "Den fremmede", Forfatter = "Albert Camus", Antall = 2 });
    bøker.Add(new Bok { Id = 9, Tittel = "Anne Franks Dagbok", Forfatter = "Anne Frank", Antall = 4 });
    bøker.Add(new Bok { Id = 10, Tittel = "Min skyld", Forfatter = "Abid Raja", Antall = 1 });
}

//slutt på kode