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
                tmp = S2C($"call {C2S(name)}");
                EmitLn(tmp);
            } 
            else 
            {
                tmp = S2C($"movl {C2S(name)}, %%eax");
                EmitLn(tmp);
            }
        }

        static void Factor()
        {

            if(Look == '(') 
            {
                Match('(');
                Expression();
                Match(')');
            } 
            else if(IsAddop(Look)) 
            {
                Match('-');
                tmp = S2C($"movl ${C2S(GetNum())}, %%eax");
                EmitLn(tmp);
                EmitLn(S2C("negl %eax"));
            } 
            else 
            {
                tmp = S2C($"movl ${C2S(GetNum())}, %%eax");
                EmitLn(tmp);
            }
        }

        static void Term()
        {
            Factor();
            while (strchr("*/", Look)) 
            {
                EmitLn(S2C("pushl %eax"));

                switch(Look)
                {
                    case '*':
                        Multiply();
                        break;
                    case '/':
                        Divide();
                        break;

                    default:
                        Expected(S2C("Mulop"));
                        break;
                }
            }
        }

        static void Expression()
        {
            if(IsAddop(Look))
                EmitLn(S2C("xor %eax, %eax"));
            else
                Term();

            while (strchr("+-", Look))
            {

                EmitLn(S2C("pushl %eax"));

                switch(Look)
                {
                    case '+':
                        Add();
                        break;
                    case '-':
                        Substract();
                        break;
                    default:
                        Expected(S2C("Addop"));
                        break;
                }
            }
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
            Expression();
            tmp = S2C($"lea {C2S(name)}, %%ebx");
            EmitLn(tmp);
            EmitLn(S2C("movl %eax, (%ebx)"));
        }

        static int Main(string[] args)
        {
            Init();
            EmitLn(S2C(".text"));
            EmitLn(S2C(".global _start"));
            EmitLn(S2C("_start:"));
            
            Assignment();

            if (Look != '\r' || Look != '\n') {
                Expected(S2C("NewLine"));
            }

            /* return the result */
            EmitLn(S2C("movl %eax, %ebx"));
            EmitLn(S2C("movl $1, %eax"));
            EmitLn(S2C("int $0x80"));
            return 0;
        }
    }
}
