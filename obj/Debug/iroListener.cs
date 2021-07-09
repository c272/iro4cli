//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.6.6
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:\Files\Programming\iro4cli\Grammar\iro.g4 by ANTLR 4.6.6

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
using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="iroParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.6.6")]
[System.CLSCompliant(false)]
public interface IiroListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="iroParser.compileUnit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCompileUnit([NotNull] iroParser.CompileUnitContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="iroParser.compileUnit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCompileUnit([NotNull] iroParser.CompileUnitContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="iroParser.block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBlock([NotNull] iroParser.BlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="iroParser.block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBlock([NotNull] iroParser.BlockContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="iroParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatement([NotNull] iroParser.StatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="iroParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatement([NotNull] iroParser.StatementContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="iroParser.attribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAttribute([NotNull] iroParser.AttributeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="iroParser.attribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAttribute([NotNull] iroParser.AttributeContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="iroParser.sys_set"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSys_set([NotNull] iroParser.Sys_setContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="iroParser.sys_set"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSys_set([NotNull] iroParser.Sys_setContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="iroParser.set"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSet([NotNull] iroParser.SetContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="iroParser.set"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSet([NotNull] iroParser.SetContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="iroParser.typed_set"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTyped_set([NotNull] iroParser.Typed_setContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="iroParser.typed_set"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTyped_set([NotNull] iroParser.Typed_setContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="iroParser.include"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInclude([NotNull] iroParser.IncludeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="iroParser.include"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInclude([NotNull] iroParser.IncludeContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="iroParser.definition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDefinition([NotNull] iroParser.DefinitionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="iroParser.definition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDefinition([NotNull] iroParser.DefinitionContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="iroParser.definition_ident"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDefinition_ident([NotNull] iroParser.Definition_identContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="iroParser.definition_ident"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDefinition_ident([NotNull] iroParser.Definition_identContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="iroParser.array"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArray([NotNull] iroParser.ArrayContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="iroParser.array"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArray([NotNull] iroParser.ArrayContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="iroParser.regex"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRegex([NotNull] iroParser.RegexContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="iroParser.regex"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRegex([NotNull] iroParser.RegexContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="iroParser.constant_ref"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConstant_ref([NotNull] iroParser.Constant_refContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="iroParser.constant_ref"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConstant_ref([NotNull] iroParser.Constant_refContext context);
}
} // namespace iro4cli.Grammar
