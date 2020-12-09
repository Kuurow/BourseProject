﻿using System;
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
        static public int tailleOctetsStruct = 7;
        static public char[] charsSeparateur = new char[] { ' ', (char)9 }; // (char)9 = "\t" qui est le tab
        static int countLines = 0;           
        static public BinaryWriter bw;

        static public string filePath = @"..\..\..\..\CAC_40_1990_test.txt";
        const string fileNameData = @"..\..\..\..\data.dat";

        static int IsLeapYear(int Year) 
        // Recherche si l'année est bisextile ou non
        {
            if (((Year % 4) == 0) && (((Year % 100) != 0)) || ((Year % 400) == 0))
                return 1;
            else
                return 0;
        }

        static bool DoEncodeDate(int Year, int Month, int Days, ref int Date)
        // Encodage de la date
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

        static bool DoDecodeDate(int convertedDate, ref int Date)
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

        static void LectureTxt(string[] lines)
        // Lecture du fichier txt
        {
            foreach (string line in lines)
            {       
                if (line.Length == 0)
                {
                    //Console.WriteLine("Ligne " + countLines + " vide !");
                    //Console.WriteLine("--------------------------------");
                }
                else
                {
                    countLines++;
                    //Console.WriteLine("Ligne " + countLines + " : " + line);
                    //Console.WriteLine(" ");

                    ConvertirLigne(line);

                    //Console.WriteLine("--------------------------------");
                } // Fin traitement          

            } // Fin traitement lignes  
        }

        static void ConvertirLigne(string dbLine)
        // Convertir la ligne courante en objet DonneesBourses
        {
            string[] tokensNonVerifies = dbLine.Trim(' ').Split(charsSeparateur);
            int cptTokensVerif = 0;

            if ((tokensNonVerifies[3] == "-") && (tokensNonVerifies[4] == "-") && (tokensNonVerifies[5] == "-") && (tokensNonVerifies[6] == "-")
                                              && (tokensNonVerifies[7] == "-") && (tokensNonVerifies[8] == "-"))
            {
                //Console.WriteLine("Jour férié");
            }
            else
            {
                string[] tokensVerifies = new string[9];

                for (int i = 0; i < tokensNonVerifies.Length; i++) // Analyse du tableau de tokens non vérifiés et non analysés
                {

                    if (tokensNonVerifies[i] != "")
                    {
                        tokensVerifies[cptTokensVerif] = tokensNonVerifies[i];  // On vient stocker le token qui vient d'être analysé dans le tableau des tokens vérifié

                        switch (tokensVerifies[1])  // On compare le nom du mois pour lui assigner un nombre correspondant au mois
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

                        if (tokensVerifies[8] == "-") { tokensVerifies[8] = "-1"; } // Si on ne connait pas le volume on met -1

                        //Console.WriteLine(tokensVerifies[cptTokensVerif] + " - " + cptTokensVerif); // Affichage tableau tokens triés (analyés) de chaque lignes
                        cptTokensVerif++;
                    }

                } // Fin traitement des tokens non vérifiés

                int Date = 0;
                float[] result = new float[9];

                for (int y = 0; y < tokensVerifies.Length; y++) // Pour chaque case du tableau de token 
                {
                    result[y] = Convert.ToSingle(tokensVerifies[y]);
                }
                //Console.WriteLine("");

                //Console.WriteLine("Encodage 2 : " + DoEncodeDate((int)result[2], (int)result[1], (int)result[0], ref Date));

                //Console.WriteLine("Date Encodée : " + Date);
                DoEncodeDate((int)result[2], (int)result[1], (int)result[0], ref Date);

                DonneesBourse curDate = new DonneesBourse
                {
                    DateConvertie = Date,
                    Ouverture = result[3],
                    Eleve = result[4],
                    Faible = result[5],
                    Cloture = result[6],
                    ClotureAjuste = result[7],
                    Volume = result[8]
                };

                FileStream fsBinary = File.Open(fileNameData, FileMode.Append);
                BinaryWriter writer = new BinaryWriter(fsBinary);

                TraduireBinaire(curDate, writer); // On envoie l'objet writer soit le fichier binaire sur lequel on va écrire

                fsBinary.Close();
                writer.Close();
            }
        }

        static void TraduireBinaire(DonneesBourse curDate, BinaryWriter writer)
        {
            // On va convertir la ligne courante en octets dans un fichier de données
            writer.Write(curDate.DateConvertie);
            writer.Write(curDate.Ouverture);
            writer.Write(curDate.Eleve);
            writer.Write(curDate.Faible);
            writer.Write(curDate.Cloture);
            writer.Write(curDate.ClotureAjuste);
            writer.Write(curDate.Volume);          
        }

        static void AfficherBinaire()
        {          
            FileStream fs = File.Open(fileNameData, FileMode.Open);
            fs.Seek(0, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(fs); //tradui de binaire en données lisibles 

            Console.WriteLine((countLines - 1) * tailleOctetsStruct); // Affiche le nombre d'octets dans le fichier
            Console.WriteLine(countLines);
            int i = 0;
            while (fs.Position < (countLines) * tailleOctetsStruct*4) // On va parcourir tous les octets du fichier
            {
                Console.Write(br.ReadSingle() + " ");
                Console.WriteLine("nbr octet : " + i); i++;
            }
            Console.WriteLine("\n-----------------");

            fs.Close();
            br.Close();            
        }

        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(filePath);
            bw = new BinaryWriter(File.Create(fileNameData)); // vas traduire en binaire et stocker dans la variable visée 
            bw.Close();

            Console.WriteLine("Contenu de CAC_40_1990_test = ");

            LectureTxt(lines);
            AfficherBinaire();
        }        
    }
}
