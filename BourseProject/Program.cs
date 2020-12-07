using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using System.IO;

namespace StructTest1
{
    public struct Test
    {
        public string Nom { get; set; }
        public int Nombre { get; set; }

        public int Calculer()
        {
            int nbr;
            nbr = this.Nombre - 2;

            return nbr;
        }

        public string Afficher()
        {
            return "Nom : " + this.Nom + ", Nombre : " + this.Nombre + ", Calculer : " + this.Calculer();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Test Struct
            {   
                //Test[] tabStruct = new Test[10];

                //Test test1 = new Test() { Nom = "test1", Nombre = 13 };
                //Test test2 = new Test() { Nom = "test2", Nombre = 42 };

                //tabStruct[0] = test1;
                //tabStruct[1] = test2;

                //Console.WriteLine("Test Struct :");
                //Console.WriteLine("Nom : " + test1.Nom + " | Nombre : " + test1.Nombre);
                //Console.WriteLine("Nom : " + test2.Nom + " | Nombre : " + test2.Nombre);

                //Console.WriteLine("Parcours du tableau : ");
                //for (int i = 0; i < tabStruct.Length; i++)
                //{
                //    if (tabStruct[i].Nombre != 0)
                //    {
                //        Console.WriteLine(tabStruct[i].Afficher());
                //    }
                //}          
            }

            string filePath = "";

            string[] lines = System.IO.File.ReadAllLines("@" + filePath);
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
                    // Use a tab to indent each line of the file.
                    Console.WriteLine("Ligne " + countLines + " : " + line);

                    Console.WriteLine(" ");

                    // string[] stringSep = new string[] { " ", "\t" };
                    char[] charsSeparateur = new char[] { ' ', (char)9 }; // (char)9 = "\t" qui est le tab
                    string[] tokensNonVerifies = line.Trim(' ').Split(charsSeparateur);
                    int cptTokensVerif = 0;

                    for (int i = 0; i < tokensNonVerifies.Length; i++) // Analyse du tableau de tokens non vérifiés et non analysés
                    {
                        string[] tokensVerifies = new string[9];

                        //Console.WriteLine(tokensNonVerifies[i] + " - " + i);
                        
                        if ( tokensNonVerifies[i] != "")
                        {
                            tokensVerifies[cptTokensVerif] = tokensNonVerifies[i];  // On vient stocker le token qui vient d'être analysé dans le tableau des tokens vérifié
                            Console.WriteLine(tokensVerifies[cptTokensVerif] + " - " + cptTokensVerif); // Affichage tableau tokens triés (analyés) de chaque lignes
                            cptTokensVerif++;
                        }
                    }
                    Console.WriteLine("--------------------------------");
                }
            }

            float time = ((float)DateTime.Now.Day);
            Console.WriteLine(time);
        }

    }
}
