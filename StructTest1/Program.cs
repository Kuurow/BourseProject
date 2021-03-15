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

    public static class StreamEOF
    {
        public static bool EOF(this BinaryReader binaryReader)
        {
            var bs = binaryReader.BaseStream;
            return (bs.Position == bs.Length);
        }
    }

    class Program
    {
        static public int[] MonthDaysbix = new int[] { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        static public int[] MonthDays = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        static public int nbChampsStructFichData = 0;
        static public char[] charsSeparateur = new char[] { ' ', (char)9 }; // (char)9 = "\t" qui est le tab
        static int countLines = 0;           
        static public BinaryWriter bw;
        static public float[] tableauLigneA = new float[7];
        static public float[] tableauLigneB = new float[7];
        static public long nbCotation = 0;

        static public string dataTextFilePath = @"..\..\..\..\CAC_40_1990_test.txt";
        const string dataBinaryFilePath = @"..\..\..\..\data.dat";

        static int IsLeapYear(int Year)
        // Recherche si l'année est bisextile ou non
        {
            if (((Year % 4) == 0) && (((Year % 100) != 0)) || ((Year % 400) == 0))
                return 1;
            else
                return 0;
        }

        static bool DoEncodeDate(int Year, int Month, int Day, ref float Date)
        {
            int[] tabAnnee;
            float result = 0;
            int cptAnneesBis = 0;

            //Console.WriteLine("date avant encodage : " + Day + " " + Month + " " + Year);

            if (IsLeapYear(Year) == 1) tabAnnee = MonthDaysbix;
            else tabAnnee = MonthDays;

            if ((Year >= 1) && (Year <= 9999) && (Month >= 1) && (Month <= 12) && (Day >= 1) && (Day <= tabAnnee[Month - 1]))
            {
                for (int i = 0; i < Year; i++)
                {
                    if (IsLeapYear(i) == 1)
                    {
                        cptAnneesBis++;
                    }
                }
                result = Year * 365 + cptAnneesBis;

                for (int i = 0; i <= Month-1; i++)
                {
                    result += tabAnnee[i];
                    //nbJoursDsMois += tabAnnee[i];
                }

                result += Day;
                Date = result;
                return true;
            }
            Date = -1;
            return false;
        }

        static bool DoDecodeDate(int convertedDate, ref float decodedDate)
        {
            //int bckpDateCvt = convertedDate;
            int encodedDate = convertedDate;
            int cptNbAnneesBis = 0;
            int decodedMonth = 0;
            int[] tabAnnee;

            int J001 = 365; // nombre de jours dans une année
            int J004 = J001 * 4 + 1; // Nombre de jours dans 4 années
            int J100 = J004 * 25 - 1; // Nombre de jours dans 100 années
            int J400 = J100 * 4 + 1; // Nombre de jours dans 400 années

            int A400 = (convertedDate / J400) * 400;    // on recherche le nombre de paquets de 400 ans dans l'année encodée
            convertedDate = convertedDate % J400;
            int A100 = (convertedDate / J100) * 100;    // on recherche le nombre de paquets de 100 ans dans l'année encodée
            convertedDate = convertedDate % J100;
            int A004 = (convertedDate / J004) * 4;      // on recherche le nombre de paquets de 4 ans dans l'année encodée
            convertedDate = convertedDate % J004;
            int A001 = (convertedDate / J001);          // on recherche le nombre de paquets d'un an dans l'année encodée
            convertedDate = convertedDate % J001;

            //Console.WriteLine("400ans : " + A400 + " 100ans : " + A100 + " 4ans : " + A004 + " 1an : " + A001);

            int decodedYear = A400 + A100 + A004 + A001;    // On calcule l'année

            for (int i = 0; i <= decodedYear; i++) // On va calculer le nombre d'années bissextiles entre l'année 0 et l'année décodée
            {
                if (IsLeapYear(i)==1)
                {
                    cptNbAnneesBis++;
                    //Console.Write(i + " ");
                }
            }
            float resteDate = encodedDate - (decodedYear * 365 + cptNbAnneesBis); // on retire le nombre de jours par année * l'année décodée et 
                                                                                    // aussi le nombre de jours qu'il manque avec les années bissextiles

            if (IsLeapYear(decodedYear) == 1) tabAnnee = MonthDaysbix; // On récupère le tableau de jours d'une année 
            else tabAnnee = MonthDays;                                 // bissextile ou non en fonction de l'année décodée

            for (int i = 0; i <= 12; i++)     // On va trouver le mois à décoder
            {
                if (resteDate > tabAnnee[i])  // Si le reste de l'encodage est suppérieur au nombre de jours contenu dans le mois i
                {
                    resteDate -= tabAnnee[i]; // On retire le nombre de jours dans le mois
                    decodedMonth++;           // On incrémente le nombre de mois
                }
                else break;
            }

            //Console.WriteLine(A400 + " " + A100 + " " + A004 + " " + A001 + " année décodée : " + decodedYear);
            //Console.WriteLine(IsLeapYear(decodedYear));
            //Console.WriteLine("Année décodée : " + decodedYear);
            //Console.WriteLine("Mois décodé : " + decodedMonth);
            //Console.WriteLine("Jour décodé : " + resteDate);

            return true;
        }

        static void LectureTxt(string[] lines)
        // Lecture du fichier txt
        {
            foreach (string line in lines)
            {       
                if (line.Length != 0)
                {
                    countLines++;

                    ConvertirLigne(line);
                }
                // Ce bloc était dans un else
                //Console.WriteLine("Ligne " + countLines + " vide !");
                //Console.WriteLine("--------------------------------");

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

                float Date = 0;
                float[] result = new float[9];

                for (int y = 0; y < tokensVerifies.Length; y++) // Pour chaque case du tableau de token 
                {
                    result[y] = Convert.ToSingle(tokensVerifies[y]);
                }
                
                DoEncodeDate((int)result[2], (int)result[1], (int)result[0], ref Date); // Encodage

                DoDecodeDate((int)Date, ref Date); // Decodage

                //Console.WriteLine("");

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

                FileStream fsBinary = File.Open(dataBinaryFilePath, FileMode.Append);
                BinaryWriter writer = new BinaryWriter(fsBinary);

                TraduireBinaire(curDate, writer); // On envoie l'objet writer soit le fichier binaire sur lequel on va écrire

                fsBinary.Close();
                writer.Close();
            }
        }

        static void TraduireBinaire(DonneesBourse curDate, BinaryWriter writer)
        {
            // On va convertir la ligne courante ici l'objet curDate courant en octets dans un fichier de données
            writer.Write(Convert.ToSingle(curDate.DateConvertie));
            writer.Write(curDate.Ouverture);
            writer.Write(curDate.Eleve);
            writer.Write(curDate.Faible);
            writer.Write(curDate.Cloture);
            writer.Write(curDate.ClotureAjuste);
            writer.Write(curDate.Volume);          
        }

        static void AfficherBinaire(string file)
        {          
            FileStream fs = File.Open(file, FileMode.Open);
            fs.Seek(0, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(fs); //traduit de binaire en données lisibles 

            var brTaille = br.BaseStream;

            Console.WriteLine("Nombre d'octets dans fichier binaire : " + brTaille.Length);
            //Console.WriteLine("Taille fichier binaire / 2 : " + (brTaille.Length) / 2);
            Console.WriteLine("");

            while (!br.EOF())
            {
                Console.WriteLine(br.ReadSingle() + " ");
                //Console.Write(": Numéro octet : " + nbChampsStructFichData + " | "); nbChampsStructFichData++;
                //Console.WriteLine("Position du reader : " + fs.Position);
                if (fs.Position % 28 == 0) 
                {
                    Console.WriteLine(fs.Position / 28);
                    Console.WriteLine();
                }
            }
            Console.WriteLine("\n-----------------");

            br.Close();
            fs.Close();        
        }

        static void InverserFichierBinaire()
        {
            FileStream fs = File.Open(dataBinaryFilePath, FileMode.Open);
            nbCotation = fs.Length / 28;
            fs.Close();

            //Console.WriteLine("Nbr lignes : " + nbCotation + "\n");

            for (int i1 = 0; i1 <= (nbCotation/2) - 1; i1++) 
            {
                int i2 = (int)nbCotation - i1 - 1;

                //Console.WriteLine("i1 : " + (i1+1) + " <> i2 : " + (i2+1));

                LireLigne(i1, ref tableauLigneA); LireLigne(i2, ref tableauLigneB);
                EcrireLigne(i1, tableauLigneB); EcrireLigne(i2, tableauLigneA);
            }

        }

        static void LireLigne(int numLigne, ref float[] tab)
        {
            FileStream fs = File.Open(dataBinaryFilePath, FileMode.Open);
            fs.Seek(numLigne * 28, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(fs); //traduit de binaire en données lisibles 

            for (int i = 0; i < 7; i++)
            {
                tab[i] = br.ReadSingle();
            }

            br.Close();
            fs.Close();                
        }

        static void EcrireLigne(int numLigne, float[] tabChangement) // On écrit le tableau récupéré par la méthode LireLigne <tabChangement>(passage par référence) 
        {                                                            // dans le fichier binaire à la ligne souhaitée <numLigne>
            FileStream fs = File.Open(dataBinaryFilePath, FileMode.Open);
            fs.Seek(numLigne * 28, SeekOrigin.Begin);  // Positionnement du pointeur afin de pouvoir ecrire là où on le veut                 
            BinaryWriter bw = new BinaryWriter(fs);

            for (int i = 0; i < 7; i++)
            {
                bw.Write(tabChangement[i]); // on ecrit la valeur de tabChangement[i] dans le fichier binaire
            }

            bw.Close(); // Fermeture du writer et du filestream afin de pouvoir s'en servir ailleurs
            fs.Close();
        }

        static void RechercherMinEtMax(String dataBinaryFilePath, float[] tabDeRecherche)
        {
            float min = tabDeRecherche[0];
            float max = tabDeRecherche[0];


            for(int i = 0; i < tabDeRecherche.Length; i++)
            {
                if(tabDeRecherche)
            }
        }

        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(dataTextFilePath);
            bw = new BinaryWriter(File.Create(dataBinaryFilePath)); // vas traduire en binaire et stocker dans la variable visée 
            bw.Close();

            Console.WriteLine("Contenu de CAC_40_1990_test = ");

            LectureTxt(lines);

            AfficherBinaire(dataBinaryFilePath);

            InverserFichierBinaire();
            //Console.WriteLine("inversion du fichier binaire effectuée!\n");

            AfficherBinaire(dataBinaryFilePath);

            Console.WriteLine(nbCotation);
        }
    }
}