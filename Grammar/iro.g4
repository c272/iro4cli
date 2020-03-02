grammar iro;

/*
 * Parser Rules
 */

compileUnit: statement* EOF;

//Statement, a single definition or operation.
statement: (attribute | sys_set | set | include);

//Attribute, top level statement defining a named value.
attribute: IDENTIFIER ARRAY_SYM? (EQUALS_SYM | REG_EQUALS_SYM) definition;

//Defining a system set.
sys_set: IDENTIFIER ARRAY_SYM? SET_OPEN statement* SET_CLOSE;

//Defining a normal named/unnamed set.
set: IDENTIFIER? ARRAY_SYM? COLON_SYM IDENTIFIER SET_OPEN statement* SET_CLOSE;

//Including an external set.
include: COLON_SYM INCLUDE QUOTE_SYM IDENTIFIER QUOTE_SYM;

//Definition, possible right hand sides of attribute.
definition: (IDENTIFIER //Literal (eg. myname)
			| regex //Regular expression.
			| constant_ref //Reference to a defined const (eg. $${__MYCONST}).
			);

//A (possibly invalid) regular expression.
regex: L_BRACKET .* R_BRACKET;

//Reference to a previously defined constant.
constant_ref: REF_SYM REF_SYM SET_OPEN IDENTIFIER SET_CLOSE;
			

/*
 * Lexer Rules
 */

//Reserved words.
INCLUDE: 'include';

//Operators & symbols.
REG_EQUALS_SYM: '\\=';
EQUALS_SYM: '=';
ARRAY_SYM: '[]';
L_SQUARE_BRACKET: '[';
R_SQUARE_BRACKET: ']';
SET_OPEN: '{';
SET_CLOSE: '}';
L_BRACKET: '(';
R_BRACKET: ')';
COLON_SYM: ':';
SEMICOLON_SYM: ';';
QUOTE_SYM: '"';
REF_SYM: '$';

//An identifier.
IDENTIFIER: [A-Za-z0-9_\\-\\.]+;

//Comments start with "#" in Iro.
COMMENT: '#' (.)*? '\n' -> channel(HIDDEN);

//Ignore all newlines and whitespace.
WS:	[ \n\r]+ -> channel(HIDDEN);

//Capture token for unknowns.
UNKNOWN_SYMBOL: .;