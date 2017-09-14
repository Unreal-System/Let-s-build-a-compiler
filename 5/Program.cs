using System;
using static LBAC.Cradle;
using static LBAC.C;

namespace LBAC
{

    partial class Program
    {
        static void Other()
        {
            tmp = new char[] {GetName()};
            EmitLn(tmp);
        }

        static void Block(char[] L)
        {
            while (!strchr("elu", Look)) {
                //dprint("Block: get Look = %c\n", Look);
                switch (Look) {
                    case 'i':
                        DoIf(L);
                        break;
                    case 'w':
                        DoWhile();
                        break;
                    case 'p':
                        DoLoop();
                        break;
                    case 'r':
                        DoRepeat();
                        break;
                    case 'f':
                        DoFor();
                        break;
                    case 'd':
                        DoDo();
                        break;
                    case 'b':
                        DoBreak(L);
                        break;
                    default:
                        Other();
                        break;
                }
                /* this is for convinent, otherwise newline character will
                cause an error */
                Newline();
            }
        }

        static void Condition()
        {
            EmitLn(S2C("<codition>"));
        }

        static void DoProgram()
        {
            Block(null);
            if (Look != 'e') 
            {
                Expected(S2C("End"));
            }
            EmitLn(S2C("END"));
        }

        static void DoIf(char[] L)
        {
            char[] L1 = NewLabel(); //strcpy(L1, NewLabel());
            char[] L2 = L1;

            Match('i');
            Condition();

            tmp = S2C($"jz {C2S(L1)}");
            EmitLn(tmp);

            Block(L);
            //dprint("DoIf: Got Look = %c\n", Look);

            if (Look == 'l') 
            {
                /* match *else* statement */
                Match('l');
                L2 = NewLabel();

                tmp = S2C($"jmp {C2S(L2)}");
                EmitLn(tmp);

                PostLabel(L1);

                Block(L);
            }

            Match('e');
            PostLabel(L2);
        }

        static void DoWhile()
        {
            Match('w');
            char[] L1 = NewLabel(); //strcpy(L1, NewLabel());
            char[] L2 = NewLabel(); //strcpy(L2, NewLabel());
            PostLabel(L1);
            Condition();
            tmp = S2C($"jz {C2S(L2)}");
            EmitLn(tmp);
            Block(L2);
            Match('e');
            tmp = S2C($"jmp {C2S(L1)}");
            EmitLn(tmp);
            PostLabel(L2);
        }

        static void DoLoop()
        {
            Match('p');
            char[] L1 = NewLabel(); //strcpy(L1, NewLabel());
            char[] L2 = NewLabel(); //strcpy(L2, NewLabel());
            PostLabel(L1);
            Block(L2);
            Match('e');
            tmp = S2C($"jmp {C2S(L1)}");
            EmitLn(tmp);
            PostLabel(L2);
        }

        static void DoRepeat()
        {
            Match('r');
            char[] L1 = NewLabel(); //strcpy(L1, NewLabel());
            char[] L2 = NewLabel(); //strcpy(L2, NewLabel());
            PostLabel(L1);
            Block(L2);
            Match('u');
            Condition();

            tmp = S2C($"jz {C2S(L1)}");
            EmitLn(tmp);
            PostLabel(L2);
        }

        /* I haven't test the actual generated x86 code here, so you're free to
        * inform me if there are bugs. :) */
        static void DoFor()
        {
            Match('f');
            char[] L1 = NewLabel(); //strcpy(L1, NewLabel());
            char[] L2 = NewLabel(); //strcpy(L2, NewLabel());
            char name = GetName();
            Match('=');
            Expression();
            EmitLn(S2C("subl %eax, $1"));  /* SUBQ #1, D0*/
            tmp = S2C($"lea {name}, %%edx");
            EmitLn(tmp);
            EmitLn(S2C("movl %eax, (%edx)"));
            Expression();
            EmitLn(S2C("push %eax")); /* save the execution of expression */
            PostLabel(L1);
            tmp = S2C($"lea {name}, %%edx");
            EmitLn(tmp);
            EmitLn(S2C("movl (%edx), %eax"));
            EmitLn(S2C("addl %eax, 1"));
            EmitLn(S2C("movl %eax, (%edx)"));
            EmitLn(S2C("cmp (%esp), %eax"));
            tmp = S2C($"jg {C2S(L2)}");
            EmitLn(tmp);
            Block(L2);
            Match('e');
            tmp = S2C($"jmp {C2S(L1)}");
            EmitLn(tmp);
            PostLabel(L2);
            EmitLn(S2C("pop %eax"));
        }
    
        static void Expression()
        {
            EmitLn(S2C("<expression>"));
        }

        static void DoDo()
        {
            Match('d');
            
            char[] L1 = NewLabel(); //strcpy(L1, NewLabel());
            char[] L2 = NewLabel(); //strcpy(L2, NewLabel());
            Expression();
            EmitLn(S2C("subl %eax, $1"));
            EmitLn(S2C("movl %eax, %ecx"));
            PostLabel(L1);
            EmitLn(S2C("pushl %ecx"));
            Block(L2);
            EmitLn(S2C("popl %ecx"));
            tmp = S2C($"loop {C2S(L1)}");
            EmitLn(tmp);
            EmitLn(S2C("pushl %ecx"));
            PostLabel(L2);
            EmitLn(S2C("popl %ecx"));
        }

        static void DoBreak(char[] L)
        {
            Match('b');
            if (L != null && L.Length > 0) 
            {
                tmp = S2C($"jmp {C2S(L)}");
                EmitLn(tmp);
            } 
            else 
            {
                Abort(S2C("No loop to break from"));
            }
        }

        static int Main(string[] args)
        {
            Init();
            DoProgram();
            return 0;
        }
    }
}
