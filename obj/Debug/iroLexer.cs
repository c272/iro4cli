//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.6.6
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:\Users\Larry\Files\Programming\iro4cli\Grammar\iro.g4 by ANTLR 4.6.6

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace iro4cli.Grammar {
#pragma warning disable 3021
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.6.6")]
[System.CLSCompliant(false)]
public partial class iroLexer : Lexer {
	public const int
		REGEX=1, INCLUDE=2, ESCAPED_BRACKET=3, REG_EQUALS_SYM=4, EQUALS_SYM=5, 
		ARRAY_SYM=6, COMMA_SYM=7, L_SQUARE_BRACKET=8, R_SQUARE_BRACKET=9, SET_OPEN=10, 
		SET_CLOSE=11, COLON_SYM=12, SEMICOLON_SYM=13, QUOTE_SYM=14, REF_SYM=15, 
		L_BRACKET=16, R_BRACKET=17, IDENTIFIER=18, COMMENT=19, WS=20, UNKNOWN_SYMBOL=21;
	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"REGEX", "INCLUDE", "ESCAPED_BRACKET", "REG_EQUALS_SYM", "EQUALS_SYM", 
		"ARRAY_SYM", "COMMA_SYM", "L_SQUARE_BRACKET", "R_SQUARE_BRACKET", "SET_OPEN", 
		"SET_CLOSE", "COLON_SYM", "SEMICOLON_SYM", "QUOTE_SYM", "REF_SYM", "L_BRACKET", 
		"R_BRACKET", "IDENTIFIER", "COMMENT", "WS", "UNKNOWN_SYMBOL"
	};


	public iroLexer(ICharStream input)
		: base(input)
	{
		_interp = new LexerATNSimulator(this,_ATN);
	}

	private static readonly string[] _LiteralNames = {
		null, null, "'include'", null, "'\\='", "'='", "'[]'", "','", "'['", "']'", 
		"'{'", "'}'", "':'", "';'", "'\"'", "'$'", "'('", "')'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "REGEX", "INCLUDE", "ESCAPED_BRACKET", "REG_EQUALS_SYM", "EQUALS_SYM", 
		"ARRAY_SYM", "COMMA_SYM", "L_SQUARE_BRACKET", "R_SQUARE_BRACKET", "SET_OPEN", 
		"SET_CLOSE", "COLON_SYM", "SEMICOLON_SYM", "QUOTE_SYM", "REF_SYM", "L_BRACKET", 
		"R_BRACKET", "IDENTIFIER", "COMMENT", "WS", "UNKNOWN_SYMBOL"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[System.Obsolete("Use Vocabulary instead.")]
	public static readonly string[] tokenNames = GenerateTokenNames(DefaultVocabulary, _SymbolicNames.Length);

	private static string[] GenerateTokenNames(IVocabulary vocabulary, int length) {
		string[] tokenNames = new string[length];
		for (int i = 0; i < tokenNames.Length; i++) {
			tokenNames[i] = vocabulary.GetLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = vocabulary.GetSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}

		return tokenNames;
	}

	[System.Obsolete("Use IRecognizer.Vocabulary instead.")]
	public override string[] TokenNames
	{
		get
		{
			return tokenNames;
		}
	}

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "iro.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return _serializedATN; } }

	public static readonly string _serializedATN =
		"\x3\xAF6F\x8320\x479D\xB75C\x4880\x1605\x191C\xAB37\x2\x17~\b\x1\x4\x2"+
		"\t\x2\x4\x3\t\x3\x4\x4\t\x4\x4\x5\t\x5\x4\x6\t\x6\x4\a\t\a\x4\b\t\b\x4"+
		"\t\t\t\x4\n\t\n\x4\v\t\v\x4\f\t\f\x4\r\t\r\x4\xE\t\xE\x4\xF\t\xF\x4\x10"+
		"\t\x10\x4\x11\t\x11\x4\x12\t\x12\x4\x13\t\x13\x4\x14\t\x14\x4\x15\t\x15"+
		"\x4\x16\t\x16\x3\x2\x3\x2\x6\x2\x30\n\x2\r\x2\xE\x2\x31\x3\x2\x3\x2\x5"+
		"\x2\x36\n\x2\x3\x2\x3\x2\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3"+
		"\x3\x3\x4\x3\x4\x3\x4\x3\x4\x5\x4\x46\n\x4\x3\x5\x3\x5\x3\x5\x3\x6\x3"+
		"\x6\x3\a\x3\a\x3\a\x3\b\x3\b\x3\t\x3\t\x3\n\x3\n\x3\v\x3\v\x3\f\x3\f\x3"+
		"\r\x3\r\x3\xE\x3\xE\x3\xF\x3\xF\x3\x10\x3\x10\x3\x11\x3\x11\x3\x12\x3"+
		"\x12\x3\x13\x6\x13g\n\x13\r\x13\xE\x13h\x3\x14\x3\x14\a\x14m\n\x14\f\x14"+
		"\xE\x14p\v\x14\x3\x14\x3\x14\x3\x14\x3\x14\x3\x15\x6\x15w\n\x15\r\x15"+
		"\xE\x15x\x3\x15\x3\x15\x3\x16\x3\x16\x3n\x2\x2\x17\x3\x2\x3\x5\x2\x4\a"+
		"\x2\x5\t\x2\x6\v\x2\a\r\x2\b\xF\x2\t\x11\x2\n\x13\x2\v\x15\x2\f\x17\x2"+
		"\r\x19\x2\xE\x1B\x2\xF\x1D\x2\x10\x1F\x2\x11!\x2\x12#\x2\x13%\x2\x14\'"+
		"\x2\x15)\x2\x16+\x2\x17\x3\x2\x5\x3\x2*+\a\x2/\x30\x32;\x43\\\x61\x61"+
		"\x63|\x5\x2\v\f\xF\xF\"\"\x84\x2\x3\x3\x2\x2\x2\x2\x5\x3\x2\x2\x2\x2\a"+
		"\x3\x2\x2\x2\x2\t\x3\x2\x2\x2\x2\v\x3\x2\x2\x2\x2\r\x3\x2\x2\x2\x2\xF"+
		"\x3\x2\x2\x2\x2\x11\x3\x2\x2\x2\x2\x13\x3\x2\x2\x2\x2\x15\x3\x2\x2\x2"+
		"\x2\x17\x3\x2\x2\x2\x2\x19\x3\x2\x2\x2\x2\x1B\x3\x2\x2\x2\x2\x1D\x3\x2"+
		"\x2\x2\x2\x1F\x3\x2\x2\x2\x2!\x3\x2\x2\x2\x2#\x3\x2\x2\x2\x2%\x3\x2\x2"+
		"\x2\x2\'\x3\x2\x2\x2\x2)\x3\x2\x2\x2\x2+\x3\x2\x2\x2\x3-\x3\x2\x2\x2\x5"+
		"\x39\x3\x2\x2\x2\a\x45\x3\x2\x2\x2\tG\x3\x2\x2\x2\vJ\x3\x2\x2\x2\rL\x3"+
		"\x2\x2\x2\xFO\x3\x2\x2\x2\x11Q\x3\x2\x2\x2\x13S\x3\x2\x2\x2\x15U\x3\x2"+
		"\x2\x2\x17W\x3\x2\x2\x2\x19Y\x3\x2\x2\x2\x1B[\x3\x2\x2\x2\x1D]\x3\x2\x2"+
		"\x2\x1F_\x3\x2\x2\x2!\x61\x3\x2\x2\x2#\x63\x3\x2\x2\x2%\x66\x3\x2\x2\x2"+
		"\'j\x3\x2\x2\x2)v\x3\x2\x2\x2+|\x3\x2\x2\x2-\x35\x5!\x11\x2.\x30\n\x2"+
		"\x2\x2/.\x3\x2\x2\x2\x30\x31\x3\x2\x2\x2\x31/\x3\x2\x2\x2\x31\x32\x3\x2"+
		"\x2\x2\x32\x36\x3\x2\x2\x2\x33\x36\x5\a\x4\x2\x34\x36\x5\x3\x2\x2\x35"+
		"/\x3\x2\x2\x2\x35\x33\x3\x2\x2\x2\x35\x34\x3\x2\x2\x2\x36\x37\x3\x2\x2"+
		"\x2\x37\x38\x5#\x12\x2\x38\x4\x3\x2\x2\x2\x39:\ak\x2\x2:;\ap\x2\x2;<\a"+
		"\x65\x2\x2<=\an\x2\x2=>\aw\x2\x2>?\a\x66\x2\x2?@\ag\x2\x2@\x6\x3\x2\x2"+
		"\x2\x41\x42\a^\x2\x2\x42\x46\a*\x2\x2\x43\x44\a^\x2\x2\x44\x46\a+\x2\x2"+
		"\x45\x41\x3\x2\x2\x2\x45\x43\x3\x2\x2\x2\x46\b\x3\x2\x2\x2GH\a^\x2\x2"+
		"HI\a?\x2\x2I\n\x3\x2\x2\x2JK\a?\x2\x2K\f\x3\x2\x2\x2LM\a]\x2\x2MN\a_\x2"+
		"\x2N\xE\x3\x2\x2\x2OP\a.\x2\x2P\x10\x3\x2\x2\x2QR\a]\x2\x2R\x12\x3\x2"+
		"\x2\x2ST\a_\x2\x2T\x14\x3\x2\x2\x2UV\a}\x2\x2V\x16\x3\x2\x2\x2WX\a\x7F"+
		"\x2\x2X\x18\x3\x2\x2\x2YZ\a<\x2\x2Z\x1A\x3\x2\x2\x2[\\\a=\x2\x2\\\x1C"+
		"\x3\x2\x2\x2]^\a$\x2\x2^\x1E\x3\x2\x2\x2_`\a&\x2\x2` \x3\x2\x2\x2\x61"+
		"\x62\a*\x2\x2\x62\"\x3\x2\x2\x2\x63\x64\a+\x2\x2\x64$\x3\x2\x2\x2\x65"+
		"g\t\x3\x2\x2\x66\x65\x3\x2\x2\x2gh\x3\x2\x2\x2h\x66\x3\x2\x2\x2hi\x3\x2"+
		"\x2\x2i&\x3\x2\x2\x2jn\a%\x2\x2km\v\x2\x2\x2lk\x3\x2\x2\x2mp\x3\x2\x2"+
		"\x2no\x3\x2\x2\x2nl\x3\x2\x2\x2oq\x3\x2\x2\x2pn\x3\x2\x2\x2qr\a\f\x2\x2"+
		"rs\x3\x2\x2\x2st\b\x14\x2\x2t(\x3\x2\x2\x2uw\t\x4\x2\x2vu\x3\x2\x2\x2"+
		"wx\x3\x2\x2\x2xv\x3\x2\x2\x2xy\x3\x2\x2\x2yz\x3\x2\x2\x2z{\b\x15\x2\x2"+
		"{*\x3\x2\x2\x2|}\v\x2\x2\x2},\x3\x2\x2\x2\t\x2\x31\x35\x45hnx\x3\x2\x3"+
		"\x2";
	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN.ToCharArray());
}
} // namespace iro4cli.Grammar
