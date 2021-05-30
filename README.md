![Build Status](https://github.com/lord-executor/Pegatron/actions/workflows/build.yml/badge.svg)

# Introduction
This is **not** a parser **generator**. It doesn't generate code. It is a non-recursive PEG rule engine / state machine that runs PEG rules against an input token stream. It also does not require the developer to write the grammar in a separate file with its own syntax but instead defines a grammar with an easy to use API which includes mechanisms to add your AST reducer code directly to those rules which means you have all of the features of your C# IDE available to you when writing reducers.

While a general purpose state machine is definitely not as good as a specifically crafted parser or even generated code, it is also much more flexible and easier to integrate into any project. It makes it easy to prototype a language design and see its limitations.

# Grammar Definition API
TODO

# Rule Notation

## Token Types
* `Identifier` - Identifiers start with a character and continue with any number of characters, digits and underscores.
* `Literal` - Single quoted strings.
* `Number` - Integer numbers.
* `Special` - Special characters and operators. In the grammar they are referenced as plain literals.

## Expressions
* Literals
  * `T<Identifier>` - Matches a terminal by **type**. The expression `T<Identifier>` is already an example of a terminal matcher that matches the terminal type "Identifier".
  * `T<Literal>` | `Literal` - Matches a terminal by its **literal value**. The "T<...>" is optional and can be omitted to make the expression more readable. Examples of terminals matching literals are `T<'foo'>`, `'+'`, `T<'{'>`.
  * `'.'` - The "any" literal matches _any_ literal regardless of type and value.
* `ruleRef` - Any naked identifier is treated as the name of another rule that should be matched in that position.
* `{min[[,]max]}` - General form of a quantifier. It can be used to match the preceeding rule anywhere from "min" to "max" where both values of course have to be positive integers. Most common quantifiers have their own short forms.
  * `*` | `{0,}` - Matches the preceeding rule zero or more times.
  * `+` | `{1,}` - Matches the preceeding rule one or more times.
  * `?` | `{0,1}` - Matches the preceeding rule zero or one time.
* Combinators
  * `|` - Choice combinator matches the _first_ matching option separated by "|"
  * ` ` - Sequence combinator matches _all_ options in the sequence separated by " " (a space character)
* Zero Width Assertions
  * `&` - Positive lookahead checks if the rule that follows immediately after the operator matches the input, but it does not consume any input.
  * `!` - Negative lookahead checks if the rule that follows immediately after the operator **does not** match the input, but it does not consume any input.
* `#!` - The "lift" operator is similar to a named reference, but it ensures that the rule that it is attached to is the only matched value that is propagated to the parent rule. See more details about the "Lift Operator" below.
* `#name` - Attaches a _name_ to a rule so that the value that it matches can be used _by name_ in a reducer. When used in a nested expression, reference names follow the "Named Reference Propagation" rules defined below.
* `:=` - The definition operator defines a rule with the name on the left and the rule expression on the right.

## PEG Rule Grammar
These grammar rules are using the expression syntax defined above and thus, these rules can be and are being used to parse "themselves".

```
atom             :=  ruleRef | terminal | '(' choice #! ')'
atomExpression   :=  ('&' | '!')? #prefix atom range? #suffix
choice           :=  sequence ('|' sequence #!)*
definition       :=  T<Identifier> #name ':=' choice #rule
minmax           :=  '{' T<Number> #min ','? #sep T<Number>? #max '}'
namedAtom        :=  atomExpression #atom ('#!' | '#' T<Identifier>)? #name
range            :=  ('*' | '+' | '?' | minmax)
ruleRef          :=  T<Identifier>
sequence         :=  namedAtom+
terminal         :=  terminalType | terminalLiteral | terminalAny
terminalAny      :=  '.'
terminalLiteral  :=  T<Literal> | 'T' '<' T<Literal> #! '>'
terminalType     :=  'T' '<' T<Identifier> #type '>'
```

## Concepts

### Lift Operator
TODO

### Named Reference Propagation
TODO

# Example
TODO


# Features
TODO
