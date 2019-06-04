# Dormez
Dormez is an interpreted programming language. It supports the following:
* Primitive types (number, string, bool, etc.)
* Non-primitive types (console, file, etc.)
* If/else if/else statements
* All kinds of loops
* Functions
* Lua-style tables
* Partial object-oriented programming

## Using multiple files
Let's say you have a program and you wanted to split it into several files. You can use the `include` statement to execute a file, allowing you to use any variables inside it. Refer to the following example:

`add.dmz`

    declare add = function of x, y {
        return x + y;
    }
    
`program.dmz`

    include "add.dmz"
    console.print(add(5, 6));
    console.readKey();

How it actually works is it inserts the text from `add.dmz` directly after the `include` statement. If you try to include the same file twice it will throw an error.

## Where should I put semicolons?
In Dormez, semicolons are *technically* not required anywhere, however, it doesn't hurt to add them to separate your statements. Take this, for example, some code without semicolons:

    declare set = [ 1, 2, 3 ]
    
    ++set[0]
    
That would throw an error because you cannot increment an array. What? Increment an array? That's supposed to increment the 0th element of `set`, it should be fine, right? Well, these are the tokens that the interpreter sees:

    declare set = [ 1 , 2 , 3 ] ++ set [ 0 ]

As you can see, the `++` operator is ambiguous between incrementing `[ 1, 2, 3 ]` or incrementing `set[0]`. In this case, it would be beneficial to put a semicolon after `[ 1, 2, 3 ]` to specify that `++set[0]` is its own statement. The following code works as expected:

    declare set = [ 1, 2, 3 ];
    
    ++set[0];

## Oddities of Dormez

### Anything is a statement
Anything syntactically correct is a statement. Take the following perfectly legal code for example:

    console.write("What is your name? ");
    declare input = console.readLine();
    7 + 8;
    console.print("Nice to meet you, " + input);
    
What does `7 + 8` do? Internally, it adds the numbers 7 and 8, but for all intents and purposes, it does nothing. Again, anything is a statement, as long as it is syntactically correct. There is a reason for this behaviour, but it's too long to explain here.

### Different scoping rules

Scoping in Dormez is quite different than in most languages. The important part is that a function **is not** outside the scope that called it. Refer to the following example:

    
    declare foo = function {
        return x + y;
    }
    
    declare bar = function {
        declare x = 5;
        declare y = 10;
        console.print(foo());
    }
    
    bar();
    
This will print 15. This is because `x` and `y` are still in scope. But why are they still in scope? That's because in Dormez, all functions are basically treated as though they were just code appended where they are called. The code above can pretty much be considered to be the code below:

    declare bar = function {
        declare x = 5;
        declare y = 10;
        console.print(function { return x + y; }());
    }
    
    bar();
    
Of course, that's not actually how the Dormez interpreter handles functions internally, so rest assured that calling a function is efficient and comes with minimal overhead.
