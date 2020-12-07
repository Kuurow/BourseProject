using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using System.IO;

namespace StructTest1
{
    public struct DonneesBourse
    {
        public float DateConvertie { get; set; }
        public float Ouverture { get; set; }
        public float Eleve { get; set; }
        public float Faible { get; set; }
        public float Cloture { get; set; }
        public float ClotureAjuste { get; set; }
        public float Volume { get; set; }
    }

    class Program
    {
        static public int[] MonthDaysbix = new int[] { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        static public int[] MonthDays = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        static public string filePath = @"..\..\..\..\CAC_40_1990_test.txt";

        static int IsLeapYear(int Year)
        {
            if (((Year % 4) == 0) && (((Year % 100) != 0)) || ((Year % 400) == 0))
                return 1;
            else
                return 0;
        }

        //static bool DoEncodeDate(int Year, int Month, int Day, ref int Date)
        //{
        //    int cptAnneesBis = 0;
        //    Date = 365 * Year + Day;    // On ajoute le nombre de jours (moyenne 365) fois le nombre d'années et le nombre de jours entré en paramètre           

        //    for (int i = 0; i < Year-1; i++)
        //    {
        //        cptAnneesBis = cptAnneesBis + IsLeapYear(i);    // On compte le nombre d'années bisextiles entre l'année 0000 et l'année entrée
        //    }

        //    Date = Date + ((Month + 1) / 2 + Month / 8) + cptAnneesBis; // On ajoute au nombre de jours déjà compté le 

        //    Console.WriteLine("M/8 : " + (Date + ((Month + 1) / 2 + Month/8) + cptAnneesBis));

        //    return true;
        //}    

        static bool DoEncodeDate2(int Year, int Month, int Days, ref int Date)
        {
            int[] tabAnnee;

            if (IsLeapYear(Year) == 1) tabAnnee = MonthDaysbix;
            else tabAnnee = MonthDays;

            if ((Year >= 1) && (Year <= 9999) && (Month >= 1) && (Month <= 12) && (Days >= 1) && (Days <= tabAnnee[Month - 1]))
            {                       
                int result = Year * 10000 + Month * 100 + Days;

                //Console.WriteLine("date convertie : " + result);                

                Date = result;

                return true;
            }

            return false;
        }

        //static bool DoDecodeDate(int convertedDate, ref int decodedDate)
        //{
        //    int cptAnneesBis = 0;
        //    int cvtDate = convertedDate;

        //    int J001 = 365; // nombre de jours dans une année
        //    int J004 = J001 * 4 + 1; // Nombre de jours dans 4 années
        //    int J100 = J004 * 25 - 1; // Nombre de jours dans 100 années
        //    int J400 = J100 * 4 + 1; // Nombre de jours dans 400 années

        //    int A400 = (convertedDate / J400) * 400;    // on recherche le nombre de paquets de 400 ans dans l'année encodée
        //    convertedDate = convertedDate % J400;

        //    int A100 = (convertedDate / J100) * 100;    // on recherche le nombre de paquets de 100 ans dans l'année encodée
        //    convertedDate = convertedDate % J100;

        //    int A004 = (convertedDate / J004) * 4;      // on recherche le nombre de paquets de 4 ans dans l'année encodée
        //    convertedDate = convertedDate % J004;

        //    int A001 = (convertedDate / J001);          // on recherche le nombre de paquets d'un an dans l'année encodée
        //    convertedDate = convertedDate % J001;

        //    decodedDate = A400 + A100 + A004 + A001;    // On calcule l'année

        //    Console.WriteLine(A400 + " " + A100 + " " + A004 + " " + A001);

            

        //    return true;
        //}

        static bool DoDecodeDate2(int convertedDate, ref int Date)
        {
            if (convertedDate != -1)
            {
                Console.WriteLine("Decodage 2 : ");

                int Year = convertedDate / 10000;
                convertedDate -= Year * 10000;
                int Month = convertedDate / 100;
                convertedDate -= Month * 100;
                int Days = convertedDate;

                Console.WriteLine("année décodée : " + Year + " " + Month + " " + Days);

                return true;                         
            }
            Date = -1;

            return false;
        }

        static void Main(string[] args)
        {
            
            string[] lines = System.IO.File.ReadAllLines(filePath);
            int countLines = 0;

            System.Console.WriteLine("Contenu de CAC_40_1990_test = ");

            foreach (string line in lines)
            {
                countLines++;
                if (line.Length == 0)
                {
                    Console.WriteLine("Ligne " + countLines + " vide !");
                    Console.WriteLine("--------------------------------");
                }
                else
                {                 
                    Console.WriteLine("Ligne " + countLines + " : " + line);
                    Console.WriteLine(" ");

                    // string[] stringSep = new string[] { " ", "\t" };
                    char[] charsSeparateur = new char[] { ' ', (char)9 }; // (char)9 = "\t" qui est le tab
                    string[] tokensNonVerifies = line.Trim(' ').Split(charsSeparateur);
                    int cptTokensVerif = 0;

                    if ((tokensNonVerifies[3] == "-") && (tokensNonVerifies[4] == "-") && (tokensNonVerifies[5] == "-") && (tokensNonVerifies[6] == "-")
                                                      && (tokensNonVerifies[7] == "-") && (tokensNonVerifies[8] == "-"))
                    {
                        Console.WriteLine("Jour férié");
                    }

                    else
                    {
                        string[] tokensVerifies = new string[9];

                        for (int i = 0; i < tokensNonVerifies.Length; i++) // Analyse du tableau de tokens non vérifiés et non analysés
                        {

                            if (tokensNonVerifies[i] != "")
                            {
                                tokensVerifies[cptTokensVerif] = tokensNonVerifies[i];  // On vient stocker le token qui vient d'être analysé dans le tableau des tokens vérifié

                                switch (tokensVerifies[1])
                                {
                                    case "janv.":
                                        tokensVerifies[1] = "01";
                                        break;
                                    case "févr.":
                                        tokensVerifies[1] = "02";
                                        break;
                                    case "mars":
                                        tokensVerifies[1] = "03";
                                        break;
                                    case "avr.":
                                        tokensVerifies[1] = "04";
                                        break;
                                    case "mai":
                                        tokensVerifies[1] = "05";
                                        break;
                                    case "juin":
                                        tokensVerifies[1] = "06";
                                        break;
                                    case "juil.":
                                        tokensVerifies[1] = "07";
                                        break;
                                    case "août":
                                        tokensVerifies[1] = "08";
                                        break;
                                    case "sept.":
                                        tokensVerifies[1] = "09";
                                        break;
                                    case "oct.":
                                        tokensVerifies[1] = "10";
                                        break;
                                    case "nov.":
                                        tokensVerifies[1] = "11";
                                        break;
                                    case "déc.":
                                        tokensVerifies[1] = "12";
                                        break;
                                }

                                if (tokensVerifies[8] == "-")
                                {
                                    tokensVerifies[8] = "-1";
                                }

                                Console.WriteLine(tokensVerifies[cptTokensVerif] + " - " + cptTokensVerif); // Affichage tableau tokens triés (analyés) de chaque lignes
                                cptTokensVerif++;
                            }

                        } // Traitement des tokens non vérifiés

                        int Date = 0;

                        float[] result = new float[9];

                        for (int y = 0; y < tokensVerifies.Length; y++) // Pour chaque case du tableau de token 
                        {
                            result[y] = Convert.ToSingle(tokensVerifies[y]);
                        }
                        Console.WriteLine("");

                        Console.WriteLine("Encodage 2 : " + DoEncodeDate2((int)result[2], (int)result[1], (int)result[0], ref Date));
                        Console.WriteLine("Date Encodée : " + Date);

                        DonneesBourse curDate = new DonneesBourse { DateConvertie = Date, Ouverture = result[3], Eleve = result[4],
                            Faible = result[5], Cloture = result[6], ClotureAjuste = result[7], Volume = result[8] };

                        Console.WriteLine(curDate);
                        Console.WriteLine(curDate.DateConvertie);

                        Console.WriteLine(DoDecodeDate2((int)Date, ref Date));
                        // Console.WriteLine("Date encodée : " + Date);
                    }
                    
                    Console.WriteLine("--------------------------------");
                    Console.WriteLine(sizeof(float)*7);
                } // Fin traitement

            } // Fin traitement lignes       
            //Console.WriteLine(TabStructToBinaire.Length);
        }        
    }
}
