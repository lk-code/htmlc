# html-compiler-tool

![html-compiler-tool](https://raw.githubusercontent.com/lk-code/html-compiler-tool/main/icon_128.png)

[![.NET Version](https://img.shields.io/badge/dotnet%20version-net6.0-blue?style=flat-square)](https://www.nuget.org/packages/htmlc/)
[![License](https://img.shields.io/github/license/lk-code/html-compiler-tool.svg?style=flat-square)](https://github.com/lk-code/html-compiler-tool/blob/master/LICENSE)
[![Build](https://github.com/lk-code/html-compiler-tool/actions/workflows/dotnet.yml/badge.svg)](https://github.com/lk-code/html-compiler-tool/actions/workflows/dotnet.yml)
[![Downloads](https://img.shields.io/nuget/dt/htmlc.svg?style=flat-square)](https://www.nuget.org/packages/htmlc/)
[![NuGet](https://img.shields.io/nuget/v/htmlc.svg?style=flat-square)](https://www.nuget.org/packages/htmlc/)

[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=lk-code_html-compiler-tool&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=lk-code_html-compiler-tool)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=lk-code_html-compiler-tool&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=lk-code_html-comvvpiler-tool)

[![buy me a coffe](https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png)](https://www.buymeacoffee.com/lk.code)

This is the HTML Compiler Tool for your cli

## installation and update

### first: install the .NET Runtime
you need to install the .NET Runtime (its free and available for macos, linux and windows)
* [macOS](https://learn.microsoft.com/en-us/dotnet/core/install/macos)
* [Windows](https://learn.microsoft.com/en-us/dotnet/core/install/windows)
* [Linux](https://learn.microsoft.com/en-us/dotnet/core/install/linux)

### then: install the tool
then you can install the html-tool very simple via this command:

`dotnet tool install --global htmlc`

### update the tool

`dotnet tool update --global htmlc`

## usage

The compile process searches in the folder for all HTML files. All files that do NOT start with an underscore are compiled. Files with an underscore (for example _layout.html or _footer.html) are used as reusable components.

### html files

### different html types

#### entry html
The compiler searches for all HTML files which do NOT start with an underscore (index.html, a-page.html, etc.). files like _layout.html, footer.html, etc. are ignored.

for example:

`/src/index.html`<br />
`/src/pages.html`<br />
`/src/components/buttons.html`<br />

#### layout
The layout file must start with an underscore. The rest of the naming is up to you. the content consists of reusable layout in HTML (styles and scripts, header, navigation, etc.).

for example:

`/src/_layout.html`<br />

#### reusable components
In addition, you can use other recyclable components. The file name must start with an underscore. The rest of the naming is up to you.

for example:

`/src/_navigation.html`<br />
`/src/_header.html`<br />
`/src/_footer.html`<br />

### supported tags and its functionality

#### @Layout
The **@Layout** tag is used in an HTML entry file to specify which layout file is to use.

#### @Body
The **@Body** tag determines in a layout file where the content from the actual HTML entry file is written.

#### @File
You can include another file with the **@File** tag in any HTML file (whether layout file. reusable file or entry file).

#### @StylePath
htmlc can also compile style files (scss or sass). the path of the compiled CSS file can be inserted using this **@StylePath** tag. The following usage makes sense:<br />
`<link rel="stylesheet" href="@StylePath">`<br />

### getting started with your own project

1. create a directory with two subdirectories **/src** and **/dist**. All project files must be stored under **/src**. The compiler writes the results under **/dist**.
2. create an initial entry file **index.html**.
3. create a layout file **_layout.html**.
4. write the following basic HTML structure in the **_layout.html** file.<br />
`<html>`<br />
`    <head>`<br />
`    </head>`<br />
`    <body>`<br />
`        @Body`<br />
`    </body>`<br />
`</html>`<br />
5. write the following example in **index.html**.<br />
`@Layout=_layout.html`<br />
`<section>`<br />
`    <div>Hello again</div>`<br />
`</section>`<br />
6. open the console of your choice and change to the project directory. (**/src** and **/dist** must be in it).
7. type the following command:<br />
`htmlc compile`<br />
8. under **/dist** should now appear a file **index.html** with the following content:<br />
`<html>`<br />
`    <head>`<br />
`    </head>`<br />
`    <body>`<br />
`        <section>`<br />
`            <div>Hello again</div>`<br />
`        </section>`<br />
`    </body>`<br />
`</html>`<br />

## commands

### config

With this command, the user configuration, which is located in the user directory as a .htmlc file (e.g. /Users/lk-code/.htmlc), can be edited. The file is hidden per default. all (!) html-files a always blocked, the compiler copies the result to the output-directory.

#### build-blacklist

With this command, a file type (e.g. *.png) can be blocked for the compilation process so that no files of this type are copied into the output directory during the asset copying process.

`htmlc config build-blacklist <add|remove> {fileextensions}`

**examples:**

`htmlc config build-blacklist add .png` this command adds a block for all .png-files

`htmlc config build-blacklist remove .png` this command removes the entry for .png-files so all png files are copied.

### compile

This command compiles all HTML files from the /src (rekusriv) folder and writes the results to /dist.

`htmlc compile <project-directory>`

If only one path is specified, htmlc searches for /src and /dist in this directory. If these do not exist, then they are created. Then htmlc searches for files in /src and writes the results to the /dist directory. If no path is specified, then htmlc searches for /src and /dist in the current folder.

**project-directory (optional):** the path to the project directory. for example: `/path/to/project`

`htmlc compile <source-directory> <output-directory>`

If two folders are specified, then htmlc uses the first value as the source path and the second value as the output paths.

**source-directory (optional):** The path to the source directory (equivalent to /src). for example: `/path/to/project/src`

**output-directory (optional):** The path to the output directory (equivalent to /dist). for example: `/path/to/another/directory/output`

`htmlc compile [...] [-s --style {path/to/main.scss}]`

Optionally, a relative path to the style entry file can be specified with -s or -style. the path to the style file must be specified relative to the /src directory. the relative path to the final css-file is written to the @StylePath-tags.

### watch

This command compiles all HTML files from the /src (rekusriv) folder and writes the results to /dist. then /src is observed for changes and recompiled whenever a change is made.

`htmlc watch <project-directory>`

The watch command is identical to the compile command. The only difference is that the watch command observes the directory after the first compile and restarts the compile process every time a change is made.

**project-directory (optional):** the path to the project directory. for example: `/path/to/project`

`htmlc watch <source-directory> <output-directory>`

If two folders are specified, then htmlc uses the first value as the source path and the second value as the output paths.

**source-directory (optional):** The path to the source directory (equivalent to /src). for example: `/path/to/project/src`

**output-directory (optional):** The path to the output directory (equivalent to /dist). for example: `/path/to/another/directory/output`

`htmlc watch [...] [-s --style {/path/to/main.scss}]`

Optionally, a relative path to the style entry file can be specified with -s or -style. the path to the style file must be specified relative to the /src directory. the relative path to the final css-file is written to the @StylePath-tags.

## notices

this tool uses:
* [Cocona (MIT)](https://github.com/mayuki/Cocona) for console app environment
* [DartSassHost (MIT)](https://github.com/Taritsyn/DartSassHost) for scss compiling
