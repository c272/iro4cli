<div align="center">
<img src="https://github.com/c272/iro4cli/raw/master/logo.png" width=200/>
<br>
<img src="https://img.shields.io/github/issues/c272/iro4cli"> <img src="https://img.shields.io/travis/c272/iro4cli"> <img src="https://img.shields.io/badge/%2ENET->=4.7.1-blue">
<br>
<br>
</div>

An open-source, CLI based rewrite of Iro by Chris Ainsley, supporting automatic VSCode extension generation. It's an easy to use command line grammar file generator, that will bundle up a provided Iro grammar and create various different grammar targets from it, and can **automagically** generate VSCode extensions to upload to the marketplace. Some of the available targets include:

 - Textmate
 - Atom
 - Ace
 - Pygments
 - Rouge
 - Sublime 3
 - CSS

The most basic command usage for Iro4CLI is the following:
```bash
iro grammarFile.iro -t
```
To use the online **official** version of Iro, you can go to the [official website](http://eeyo.io/iro). For documentation on how to create Iro grammars, check the official documentation [here](http://eeyo.io/iro/documentation).

## Getting Started
To get started using Iro4CLI on Windows, all you'll need is .NET Framework 4.7.1 or later, and you can download one of the prebuilt binaries from the [Releases tab](https://github.com/c272/iro4cli/releases) on the main repository page.

If you're using Linux, however, you'll have to build the project using Mono and `mkbundle`. Make sure you have the following dependencies installed as a nuget packages before attempting to build with `mkbundle`:

 - `Antlr4`
 - `Antlr4.CodeGeneration`
 - `Antlr4.Runtime`
 - `CommandLineParser`
 - `Newtonsoft.Json`
 - `shortid`

Once this is done, you can use a simple `mkbundle` command such as the example below to create a native executable for your distro:
```bash
mkbundle -o iro --simple bin/Debug/iro.exe --machine-config /etc/mono/4.5/machine.config --no-config --nodeps bin/Debug/*.dll
```

## Usage
An example grammar is something like the following:
```
name = exampleGram
file_extensions [] = exgr, exg;

styles [] {
    .example : style {
        //todo
    }
}
```

## Progress
So far the following targets have been implemented:
- [x] Textmate
- [x] CSS
- [x] Ace
- [ ] Atom
- [ ] Pygments
- [ ] Rouge

*Feature parity with Iro online here. Additional goals below.*

- [x] VSCode Extension Generation
- [ ] HighlightJS
- [ ] ANTLR Input
