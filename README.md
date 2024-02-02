# DVarScript

DVarScript is an interpreted dynamically typed programming language.
The interpreter is written in C#.


In order to write a Hello World program, you can do the following:

```
println("Hello World!");
```

Data types:
  - **Boolean:** has two values `true` and `false`
  - **Number:** is integer or double-precision floating point (`69`, `3.14`).
  - **String:** represents a sequence of Unicode characters.  They are enclosed in double quotes (`"Naruto"`, `""` (empty string), `"69"`).
  - **Nil:** is a special value that indicates the absence of a value.

## Expressions

Expression is a combination of values, variables, operators, and functions that produces a single value. 

### Arithmetic

Add two numbers:

```
let a be 5;
let b be 10;
let c be a + b;
```

Subtract one number from another:

```
let a be 5;
let b be 10;
let c be a - b;
```

Multiply two numbers:

```
let a be 5;
let b be 10;
let c be a * b;
```

Divide one number by another:

```
let a be 5;
let b be 10;
let c be a / b;
```

### Comparison and Equality

Comparison and equality operators are used to compare two values and determine if they are equal or not.

Equal to (==): Returns true if both the values on either side of the operator are equal, false otherwise.

```
let a be 5;
let b be 5;
let c be a == b; // c will be true
```

Not equal to (!=): Returns true if both the values on either side of the operator are not equal, false otherwise.

```
let a be 5;
let b be 10;
let c be a != b; // c will be true
```

Greater than (>): Returns true if the value on the left is greater than the value on the right, false otherwise.

```
let a be 5;
let b be 10;
let c be a > b; // c will be false
```

Less than (<): Returns true if the value on the left is less than the value on the right, false otherwise.

```
let a be 5;
let b be 10;
let c be a < b; // c will be true
```

Greater than or equal to (>=): Returns true if the value on the left is greater than or equal to the value on the right, false otherwise.

```
let a be 5;
let b be 10;
let c be a >= b; // c will be false
```

Less than or equal to (<=): Returns true if the value on the left is less than or equal to the value on the right, false otherwise.

```
let a be 5;
let b be 10;
let c be a <= b; // c will be true
```

### Logical operators

The not operator, a prefix `!`, returns false if its operand is true, and vice versa.

```
!true;  // false.
!false; // true.
```

An `and` expression determines if two values are both true.

```
true and false; // false.
true and true;  // true.
```

An `or` expression determines if either of two values or both are true.

```
false or false; // false.
true or false;  // true.
```

### Precedence and grouping

Precedence refers to the order in which operators are evaluated in an expression.
For example, in arithmetic expressions, multiplication and division have higher precedence than addition and subtraction.
This means that expressions such as `2 + 3 * 4` are evaluated as `(2 + 3) * 4`, rather than `2 + (3 * 4)`.

### Statements

A statement is a unit of code that performs an action. Statements can be simple, such as declaring a variable, or complex, such as an if-else statement.

Declaration statement: Declares a variable and assigns it a value. If you omit the initializer, the variable’s value defaults to `nil`.

```
let x be 69;
let isNil; // is nil
```

If-else statement: Executes a statement if a condition is true, or another statement if the condition is false.

```
if (x > 10) {
  println("x is greater than 10");
} else {
  println("x is less than or equal to 10");
}
```

A while loop executes the body repeatedly as long as the condition expression evaluates to true.

```
let a be 1;
while (a < 10) {
  println(a);
  a = a + 1;
}
```

For loop: Loops through a set of statements a specified number of times.

```
for (let i be 0; i < 10; i++) {
  println(i);
}
```

### Functions

A function is a block of code that performs a specific task and can be called multiple times with different inputs. 

```
func sum(a, b) {
  return a + b;
}

let result be sum(5, 10);
println(result); // 15
```







