# C# info
This is a port of the C version of this code to C#. I have left the C code in place so that the output can be studied and compared. I'm slowly working through the chapters.

## Phase 1
Re-implement the C as C#. following the original C as far as possible. The initial implementation is almost a completely straight copy of the C, but put in to C# syntax. 

## Phase 2
I will do a second pass one I make is to chapter 16 and update the entire repo to modern sane C#. When I do that I'll create a branch for the original version and be sure to let you know what it's called here. At the moment Master contains the only C# implementation. 

## Phase 3
After I've updated the code to modern C#, I will port the output to CIL instructions. Again, that will mean the original x86 version will be left in a named branch. I don't see leaving this as x86 as being particularly useful. The reader can look at the original 68000 (in the tutorX.txt files) and the x86 (in the C) to see how the compiler differs, so making it output CIL assembly language is not such a big deal.

I/m in pahase 1, and I've made it to Chapter 3 as of writing this. But, check later, I might forget to update this text.

----
----

# Original Information for C version
A C version of the "Let's Build a Compiler" by Jack Crenshaw 
http://compilers.iecc.com/crenshaw/

This repository is forked form: https://github.com/vtudose/Let-s-build-a-compiler

And since the original is far from complete(only the first two chapter), and
the author had been inactive for quite a long time, I decided to create a new
repository.

If there are are any licence issue, please inform.

# Comments
Below are some comments about the source code here and the original article.

## What to do with the generated code
The generated assembly code is devoted to x86 architecture. The syntax is AT&T
style and the experiment operating system is Linux. You may try to save the
generated code to for example "test.s" and execute the following commands to
check the output.
```
as --32 -o test.o test.s
ld -m elf_i386 test.o -o test
./test
echo $?
```

## Assembly code
It is a C port of the original article which is written in Pascal. And the
generated code are ported to x86 instead of 86000.

If you want to test the generated code, please keep in mind that the generated
code might be incomplete to be directly assembled. For example, when loading a
variable, we directly generate Assembly code "movl X, %eax", and variable 'X'
might not be declared since the article is far from mentioning "variable types".
Thus you'll have to type the missing parts all by yourself.

## IO routines
I am definitely NOT an assembly expert. When the author mentioned adding IO
routine by library, I cannot simply find the x86 alternative. The closest
thing is C's library function "printf". But then I decided not no bother
adding this kind of routine cause they are only used in two chapters.

Instead, I'll save the value as the return code of a process. It's done by
saving the value to register "%ebx" and call "int $0x80", as follows:
```
movl var, %ebx
movl $1, %eax  # exit function
int $0x80
```

## The Article
The article mainly talks about how to design compiler top-down. It covered a
lot of aspects in compiler design like lexical scanning, BNF, symbols,
procedures, types, code generation, etc.

It is a good article to follow for beginners.
