# Dormez
Dormez is an interpreted programming language. It supports the following:
* Primitive types (number, string, bool, etc.)
* Non-primitive types (console, file, etc.)
* If/else if/else statements
* All kinds of loops
* Functions
* Lua-style tables
* Partial object-oriented programming

## Getting started
Dormez should be super easy for anyone who knows how to code already. Hello world is as simple as two lines:
    
    console.print("Hello world");
    console.readKey();
    
## Variables
Variables are also simple:

    declare y = 5;
    declare x; // If x is not given a value, the default value is undefined
    
`undefined` is the equivalent to C#'s `null`, and it is a primitive type.

## If statements
If statements are mostly the same as in other languages. Brackets between 'if' and '{' are optional.

    declare input = console.readLine();
    if input == "foo" {
        // do something here
    } else if input == "bar" {
        // do something else here
    } else {
        // do something even else here
    }

## Loops
Dormez has four kinds of loops:
### While loops

    declare i = 0;
    while i < 10 {
        console.print(i);
        i++;
    }
    
### Until loops
Until loops are just while loops but with the condition inverted. This until loop, for example, is equivalent to the while loop shown above.

    declare i = 0;
    until i == 10 {
        console.print(i);
        i++;
    }

### From loops
These are kind of like for loops in other languages, and are meant to be a shortcut for iterating over ranges of numbers. The following code is equivalent to the two loops shown above:

    from 0 to 10 with i {
        console.print(i);
    }
    
The upper limit is **exclusive**, meaning `i` will never actually reach 10. 
You can also specify an increment using the `by` keyword. To go up by three, for example, use this:

    from 0 to 10 by 3 with i {
        console.print(i);
    }

This would output 0, 3, 6, and 9 on new lines.

The lower bound, upper bound, and increment can be evaluated, so they do not have to be constant numbers. Refer to the following example, showing a legal `from` loop:

    declare beginning = 0;
    declare end = 10;
    declare increment = 1;
    
    // legal
    from beginning to end by increment with index {
        console.print(index);
    }
    
    // also legal
    // from 9 to -1 by -1 with i
    from math.sqrt(80 + 1) to (1-1)^3 - 1 by math.nthRoot(-1, 3) with i {
        console.print(i);
    }
    // Keep in mind that the 'upper bound' (-1) is exclusive, so it will iterate from 9-0

### For-each loops
These are like the foreach loops from C#. The following loop prints each number in 'set', which should be the numbers from 0 to 9.

    declare set = [ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 ]
    for each number in set {
        console.print(number);
    }
    
### Break and continue
You can use break and continue in loops just like in other programming languages. The following example will print every number from 0 to 9 inclusively, except for 5.

    from 0 to 10 with i {
        if i == 5 {
            continue;
        }
        console.print(i);
    }

The following function will print every number from 0 to 4 inclusively:

    from 0 to 10 with i {
        if i == 5 {
            break;
        }
        console.print(i);
    }
    
## Functions
Here is where Dormez strays from other languages somewhat. You cannot simply declare a function, but instead must assign a function to a variable. It's way more simple than it sounds. Here's an example:

    declare exampleFunction = function {
        console.print("Hello world");
    }
    
Then you can call the function like in other languages.

    exampleFunction();
    
This will print 'Hello world'.
Functions can also take parameters:

    declare addAndPrint = function of x, y {
        console.print(x + y);
    }
    
And finally, functions can also return values:

    declare add = function of x, y {
        return x + y;
    }
    
You can also use return for flow control reasons:

    declare func = function of x {
        if x > 0 {
            return;
        }
        
        // ...
    }
    
When a value is not returned, a function will return `void`, which is technically a primitive type, like C#'s `null`.

## Tables
Dormez tables are like Lua tables but immutable. Refer to the following example:

    declare position = table {
        x = 5,
        y = 2
    }
    
As functions are simply a data type, a table can also contain functions:

    declare position = table {
        x = 5,
        y = 2,
        
        myFunc = function {
            // whatever
        }
    }
    
Tables also act like classes, which means the `this` keyword is available inside table functions. Refer to the following example:

    declare position = table {
        x = 5,
        y = 2,
        
        magnitude = function {
            return math.sqrt(this.x ^ 2 + this.y ^ 2);
        }
    }
    
However, unlike Lua's tables, tables in Dormez are immutable, meaning you cannot add members to the table. The following would throw an exception:

    declare position = table {
        x = 5,
        y = 2
    }
    
    position.z = -7; // ERROR: Object does not contain member: z

Obviously, trying to read a member that doesn't exist will also throw the same error:

    declare position = table {
        x = 5,
        y = 2
    }
    
    console.print(position.z == undefined); // ERROR: Object does not contain member: z

Why doesn't it return `true`? Simple. `position.z` isn't undefined *because it doesn't exist*. So then how do you check if a table contains a member? You can't. Since tables in Dormez are static, they reliably have the same set of variables, so you would never need to check in the first place. Think about it, how often do you need to check if an object contains a member in C# or C++?

It is easy to see how you could utilize tables as instantiatable objects, especially if you're used to Lua. You *could* do this to create a 'class' of sorts:

    declare Vector = function of x, y {
        return table {
            x = x,
            y = y,

            magnitude = function {
                return (this.x ^ 2 + this.y ^ 2).sqrt()
            }
        }
    }
    
    declare position = Vector(5, 2);

But Dormez has an alternative (sort of).

## Templates
Templates are the closest thing to classes in Dormez. You can recreate the `Vector` class from above using the following code:

    declare Vector = template {
        x, y,
        
        constructor = function of x, y {
            this.x = x;
            this.y = y;
        },
        
        magnitude = function {
            return (this.x ^ 2 + this.y ^ 2).sqrt()
        }
    }

To create an instance of `Vector`, just do this:

    declare position = Vector(5, 2);
    
Upon instantiation, `constructor` is called immediately with the arguments `5` and `2`. The template keyword is really just a shortcut for the following:

    declare Vector = function of x, y {
        declare instance = table {
            x, y,

            constructor = function {
                this.x = x;
                this.y = y;
            },

            magnitude = function {
                return (this.x ^ 2 + this.y ^ 2).sqrt()
            }
        }
        
        instance.constructor();
        
        return instance;
    }

And in the same way, instantiating a template also returns a `table`.

### Operator overloading
Templates support overloading any operator except indexers. Refer to the following example:

    declare Vector = template {
        x, y,
        
        constructor = function of x, y {
            this.x = x;
            this.y = y;
        },
        
        add = function of other {
            return Vector(this.x + other.x, this.y + other.y);
        },
        
        sub = function of other {
            return Vector(this.x - other.x, this.y - other.y);
        }
        
        // ...
    }

The full list of operators that can be overloaded are the following:
* `add(other)`
* `sub(other)`
* `multiply(other)`
* `divide(other)`
* `negate()` -- unary operator `-`
* `mod(other)` -- modulus operator `%`
* `exponent(other)` -- exponent operator `^`
* `equals(other)`
* `notEquals(other)`
* `lessThan(other)`
* `greaterThan(other)`

Furthermore, `toString()` and `getType()` can also be overridden, though the latter is not recommended.

### Inheritance
Templates allow inheritance, which works more-or-less the same as in other languages. Let's say you have a template `Vector2` for example:

    declare Vector2 = template {
        x, y,

        constructor = function of x, y {
            this.x = x;
            this.y = y;
        },

        toString = function {
            return "(" + this.x + ", " + this.y + ")";
        }
    }

Now let's say you wanted to make a `Vector3` class identical to `Vector2` but with a Z component, and of course, you must change `toString` to represent the new `Vector3` class. You can use this through use of the `extending` and `base` keywords. Refer to the following example:

    declare Vector3 = template extending Vector2 {
        z,

        constructor = function of x, y, z {
            base.constructor(x, y);

            this.z = z;
        },

        toString = function {
            return "(" + base.x + ", " + base.y + ", " + this.z + ")";
        }
    }

Keep in mind, that `this` does **not** contain the members `x` and `y`, so to access them, use `base.x` and `base.y`. Consider `base` a separate object from `this`, with its own functions like `toString` and `constructor`.

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
