grammar iro;

//Avoid annoying CLS compliance warnings from ANTLR.
@parser::header {#pragma warning disable 3021}
@lexer::header {#pragma warning disable 3021}

/*
 * Parser Rules
 */

compileUnit: block* EOF;

//Top level statement.
block : (statement | sys_set);

//Statement, a single definition or operation.
statement: (attribute | set | include);

//Attribute, top level statement defining a named value.
attribute: IDENTIFIER definition;

//Defining a system set.
sys_set: IDENTIFIER ARRAY_SYM? SET_OPEN statement* SET_CLOSE;

//Defining a normal named/unnamed set.
set: IDENTIFIER? ARRAY_SYM? COLON_SYM typed_set;
typed_set: IDENTIFIER (SET_OPEN statement* SET_CLOSE | statement+ SEMICOLON_SYM);

//Including an external set.
include: COLON_SYM IDENTIFIER QUOTE_SYM IDENTIFIER QUOTE_SYM SEMICOLON_SYM;

//Definition, possible right hand sides of attribute.
definition: ARRAY_SYM? (EQUALS_SYM | REG_EQUALS_SYM) (definition_ident //Literal (eg. myname)
			| regex //Regular expression.
			| constant_ref //Reference to a defined const (eg. $${__MYCONST}).
			| array
			) SEMICOLON_SYM?;

//Definition identifier. Can have surrounding quotes.
definition_ident: QUOTE_SYM? IDENTIFIER QUOTE_SYM?;

//An array of values.
array: (IDENTIFIER COMMA_SYM)+ IDENTIFIER;

//A (possibly invalid) regular expression.
regex: REGEX;

//Reference to a previously defined constant.
constant_ref: REF_SYM REF_SYM SET_OPEN IDENTIFIER SET_CLOSE;

/*
 * Lexer Rules
 */

//Regular expression.
REGEX: L_BRACKET (~[()\n\r] | '\\(' | '\\)' | REGEX)* R_BRACKET ('|' | '?' | '*' | '+')* REGEX?;

//Operators & symbols.
ESCAPED_BRACKET: '\\(' | '\\)';
REG_EQUALS_SYM: '\\=';
EQUALS_SYM: '=';
ARRAY_SYM: '[]';
COMMA_SYM: ',';
L_SQUARE_BRACKET: '[';
R_SQUARE_BRACKET: ']';
SET_OPEN: '{';
SET_CLOSE: '}';
COLON_SYM: ':';
SEMICOLON_SYM: ';';
QUOTE_SYM: '"';
REF_SYM: '$';
L_BRACKET: '(';
R_BRACKET: ')';

//An identifier.
IDENTIFIER: [A-Za-z0-9_.-]+;

//Comments start with "#" in Iro.
COMMENT: '#' (.)*? '\n' -> channel(HIDDEN);

//Ignore all newlines and whitespace.
WS:	[ \n\r\t]+ -> channel(HIDDEN);

//Capture token for unknowns.
UNKNOWN_SYMBOL: .;