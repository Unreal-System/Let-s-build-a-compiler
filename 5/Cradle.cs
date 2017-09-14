using System;
using static LBAC.C;

namespace LBAC
{
    //This class is close to te original C.
    //I'm trying to not deviate to far initally to ensure 
    //that I can reproduce the code in a way that makes
    //a transition form the C to the C#.
    //As I go on with this, I'll add in a more optimal C#
    //version. Certainly, the string manipulation is currently
    //too C like.
    public class Cradle
    {    
        const byte MAX_BUF = 100;
        const byte TABLE_SIZE  = 26;
        static int LCount = 0;

        public static char[] tmp = new char[MAX_BUF];
        
        //not used in this tutorial
        //public static char[]  token_buf  = new char[MAX_BUF];

        public static char Look;
        public static int[] Table = new int[TABLE_SIZE];

        public static void GetChar() 
        {
            Look = getchar();
        }

        public static void Error(char[] s)
        {
            printf($"\nError: {C2S(s)}.");
        }

        public static void Abort(char[] s)
        {
            Error(s);
            exit(1);
        }

        public static void Expected(char[] s)
        {
            tmp = S2C($"{C2S(s)} Expected");
            Abort(tmp);
        }

        public static void Match(char x)
        {
            if(Look == x) 
            {
                GetChar();
            } 
            else 
            {
                tmp =  S2C($"' {x} ' ");
                Expected(tmp);
            }
        }

        public static void Newline()
        {
            if (Look == '\r') 
            {
                GetChar();
                if (Look == '\n') 
                {
                    GetChar();
                }
            } 
            else if (Look == '\n') 
            {
                GetChar();
            }
        }

        public static bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        } 

        public static bool IsDigit(char c)
        {
            return (c >= '0') && (c <= '9');
        }

        //Not used in this tutorial, left here for reference.
        // public static bool IsAlNum(char c)
        // {
        //     return IsAlpha(c) || IsDigit(c);
        // }

        public static bool IsAddop(char c)
        {
            return (c == '+') || (c == '-');
        }

        //Not used in this tutorial, left here for reference.
        // public static bool IsWhite(char c)
        // {
        //     return (c == ' ') || (c == '\t');
        // }

        public static char GetName()
        {
            char c = Look;

            if(!IsAlpha(Look)) 
            {
                tmp = S2C("Name");
                Expected(tmp);
            }

            GetChar();

            return uppercase(c);
        }
        
        public static int GetNum()
        {
            int value = 0;
            if( !IsDigit(Look)) 
            {
                tmp = S2C("Integer");
                Expected(tmp);
            }

            while (IsDigit(Look)) {
                value = value * 10 + Look - '0';
                GetChar();
            }

            return value;
        }

        //Not used in this tutorial, left here for reference.
        // public static void SkipWhite()
        // {
        //     while (IsWhite(Look)) {
        //         GetChar();
        //     }
        // }

        public static void Emit(char[] s)
        {
            printf($"\t{C2S(s)}");
        }

        public static void EmitLn(char[] s)
        {
            Emit(s);
            printf("\n");
        }

        public static void Init()
        {
            LCount = 0;

            InitTable();
            GetChar();
        }

        public static void InitTable()
        {
            int i;
            for (i = 0; i < TABLE_SIZE; i++) 
            {
                Table[i] = 0;
            }
        }

        public static char[] NewLabel()
        {
            var labelName = S2C($"L{LCount.ToString("D4")}"); //sprintf(labelName, "L%02d", LCount);
            LCount ++;
            return labelName;
        }

        public static void PostLabel(char[] label)
        {
            printf($"{label}:\n");
        }
    }   
}