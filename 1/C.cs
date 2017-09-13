using System;

namespace LBAC
{      
    
    // Silly little class to make a direct translation of the C code a little simpler
    //This will go away after I work through some more chapters.
    class C
    {
        public static void printf(string s)
        {
            Console.Write(s);
        }

        public static char getchar()
        {
            return (char)Console.Read();
        }

        public static void exit(int code)
        {
            Environment.Exit(code);
        }

        public static char[] S2C(string s)
        {
            return s.ToCharArray();
        }

        public static string C2S(char[] c)
        {
            return new string(c).Trim();
        }
    }
}