using System;
using static LBAC.Cradle;
using static LBAC.C;

namespace LBAC
{
    class Program
    {    
        static void Multiply()
        {
            Match('*');
            Factor();
            EmitLn(S2C("imull (%esp), %eax"));
            /* push of the stack */
            EmitLn(S2C("addl $4, %esp"));
        } 

        static void Divide()
        {
            Match('/');
            Factor();

            /* for a expersion like a/b we have eax=b and %(esp)=a
            * but we need eax=a, and b on the stack 
            */
            EmitLn(S2C("movl (%esp), %edx"));
            EmitLn(S2C("addl $4, %esp"));

            EmitLn(S2C("pushl %eax"));

            EmitLn(S2C("movl %edx, %eax"));

            /* sign extesnion */
            EmitLn(S2C("sarl $31, %edx"));
            EmitLn(S2C("idivl (%esp)"));
            EmitLn(S2C("addl $4, %esp"));
        }

        public static void Ident()
        {
            var name = GetName();
            if (Look == '(') 
            {
                Match('(');
                Match(')');
                tmp = S2C($"call {name}");
                EmitLn(tmp);
            } 
            else 
            {
                tmp = S2C($"movl {name}, %%eax");
                EmitLn(tmp);
            }
        }

        static int Factor()
        {
            int factor;
            if (Look == '(') {
                Match('(');
                factor = Expression();
                Match(')');
            } else if (IsAlpha(Look)) {
                factor = Table[GetName() - 'A'];
            } else {
                factor = GetNum();
            }

            return factor;
        }

        static int Term()
        {
            int value = Factor();
            while (strchr("*/", Look)) {
                switch(Look)
                {
                    case '*':
                        Match('*');
                        value *= Factor();
                        break;
                    case '/':
                        Match('/');
                        value /= Factor();
                        break;
                    default:
                        Expected(S2C("Mulop"));
                        break;
                }
            }

            return value;
        }

        static int Expression()
        {
            int value;
            if(IsAddop(Look))
                value = 0;
            else
                value = Term();

            while (IsAddop(Look)) {
                switch(Look)
                {
                    case '+':
                        Match('+');
                        value += Term();
                        break;
                    case '-':
                        Match('-');
                        value -= Term();
                        break;
                    default:
                        Expected(S2C("Addop"));
                        break;
                }
            }

            return value;
        }


        static void Add()
        {
            Match('+');
            Term();
            EmitLn(S2C("addl (%esp), %eax"));
            EmitLn(S2C("addl $4, %esp"));
            
        }

        static void Substract()
        {
            Match('-');
            Term();
            EmitLn(S2C("subl (%esp), %eax"));
            EmitLn(S2C("negl %eax"));
            EmitLn(S2C("addl $4, %esp"));
        }

        public static void Assignment()
        {
            var name = GetName();
            Match('=');
            var index = name - 'A';
            Table[name - 'A'] = Expression();
        }

        /* Input Routine
        * We do a little different to the original article.  The syntax of
        * input is "?<variable name><expression>" */
        public static void Input()
        {
            Match('?');
            var name = GetName();
            Table[name - 'A'] = Expression();
        }

        /* Output Routine */
       public static  void Output()
        {
            Match('!');
            var t = Table[GetName() - 'A'];
            tmp = S2C($"{t}");
            EmitLn(tmp);
        }

        static int Main(string[] args)
        {
            Init();
            do
            {
                switch(Look) 
                {
                    case '?':
                        Input();
                        break;
                    case '!':
                        Output();
                        break;
                    default:
                        Assignment();
                        break;
                }

                Newline();
            } 
            while (Look != '.');
            return 0;
        }
    }
}
