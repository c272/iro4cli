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
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="iroParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.6.6")]
[System.CLSCompliant(false)]
public interface IiroVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="iroParser.compileUnit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCompileUnit([NotNull] iroParser.CompileUnitContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="iroParser.block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBlock([NotNull] iroParser.BlockContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="iroParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement([NotNull] iroParser.StatementContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="iroParser.attribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAttribute([NotNull] iroParser.AttributeContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="iroParser.sys_set"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSys_set([NotNull] iroParser.Sys_setContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="iroParser.set"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSet([NotNull] iroParser.SetContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="iroParser.typed_set"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTyped_set([NotNull] iroParser.Typed_setContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="iroParser.include"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitInclude([NotNull] iroParser.IncludeContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="iroParser.definition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDefinition([NotNull] iroParser.DefinitionContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="iroParser.definition_ident"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDefinition_ident([NotNull] iroParser.Definition_identContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="iroParser.array"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArray([NotNull] iroParser.ArrayContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="iroParser.regex"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRegex([NotNull] iroParser.RegexContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="iroParser.constant_ref"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitConstant_ref([NotNull] iroParser.Constant_refContext context);
}
} // namespace iro4cli.Grammar
