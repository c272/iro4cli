<div align="center">
<img src="https://github.com/c272/iro4cli/raw/master/logo.png" width=200/>
<br>
<img src="https://img.shields.io/github/issues/c272/iro4cli"> <img src="https://img.shields.io/github/workflow/status/c272/iro4cli/.NET"> <img src="https://img.shields.io/badge/.NET-6-blue">
<br>
<br>
</div>

An open-source, CLI based rewrite of Iro by Chris Ainsley, supporting automatic VSCode extension and Atom package generation. It's an easy to use command line tool, and will bundle up a provided Iro grammar and create various different grammar targets from it, **automagically** generating extensions to upload to the marketplace. Some of the available targets include:

 - Textmate
 - Atom
 - Ace
 - Pygments
 - Rouge
 - Sublime 3
 - CSS

To create an Atom package and VSCode extension at the same time, you can use the following:
```bash
iro grammarFile.iro --vscode --atomext
```
To use the online **official** version of Iro, you can go to the [official website](http://eeyo.io/iro). For documentation on how to create Iro grammars, check the official documentation [here](http://eeyo.io/iro/documentation).

## Getting Started
To get started using iro4cli, you can use one of the provided prebuilt binaries from the [Releases tab](https://github.com/c272/iro4cli/releases) on the main repository page. If one of these is not available for your chosen operating system or distribution, then you can follow the build steps below.

To build, ensure you have the .NET 6.0 SDK installed (`dotnet-sdk-6.0` on `apt` for Ubuntu/Debian users). **You will also need to install the Antlr4 v4.7.2 command line tool, and ensure it is available on PATH.** Installing a newer version of the ANTLR4 command line tool or having it unavailable on PATH will result in a build error. Once these prerequisites are installed, simply run the following after cloning the repository:
```
dotnet build
```
You may need to run `dotnet build` twice for it to compile properly.

## Usage
For all command line options available, simply run `iro --help`, or see the following Wiki page:
https://github.com/c272/iro4cli/wiki/Command-Line-Options

For Iro grammars, an example grammar is something like the following:
```
name = exampleGram
file_extensions [] = exgr, exg;

styles [] {
    .example : style {
        //todo
    }
}
```
I highly suggest you check the [official documentation](http://eeyo.io/iro/documentation) for more details, however.

## Progress
So far the following targets have been implemented:
- [x] Textmate
- [x] CSS
- [x] Ace
- [x] Atom
- [x] Pygments
- [x] Rouge
- [ ] Sublime 3

*Feature parity with Iro online here. Additional goals below.*

- [x] VSCode Extension Generation
- [x] Atom Package Generation
- [ ] Sublime Extension Generation
- [ ] HighlightJS
- [ ] ANTLR Input
