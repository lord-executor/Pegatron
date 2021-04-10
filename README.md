# Introduction
This is **not** a parser **generator**. It doesn't generate code. It is a non-recursive PEG rule engine / state machine that runs PEG rules against an input token stream. It also does not require the developer to write the grammar in a separate file with its own syntax but instead defines a grammar with an easy to use API which includes mechanisms to add your AST reducer code directly to those rules which means you have all of the features of your C# IDE available to you when writing reducers.

While a general purpose state machine is probably not as efficient (citation needed) as generated code, it is also much more flexible and easier to integrate into any project.


# Example
TODO


# Features
TODO
