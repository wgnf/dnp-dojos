# 04/21: Command line calculator

This month's excercise was to create a Command line calculator that can parse something like `(1 + 2) * 3` and calculate it correctly.  
  
More on the excercise can be found [here](https://www.dotnetpro.de/workout/dotnetpro-dojos/einmaleins-zweiten-2640320.html).

## Features

### Can's

What this "Tool" currently can do:

- basic arithmetic (`+`, `-`, `*`, `/`)
- while respecting multiplication/division before addition/subtraction
- parentheses (in any level of nesting), i.e.: `1 + 2 * 3 = 7`, `(1 + 2) * 3 = 9`
- `^` as a symbol for exponents, i.e.: `2^2 = 4`, `2^(1+1) = 4`

### Can't's

What this "Tool" is currently missing:

- scientific exponential notation, i.e. `3.2e3 = 3200`
- providing static methods from `System.Math`, i.e. `sqrt(4) = 2`
