# html-compiler-tool

![html-compiler-tool](https://raw.githubusercontent.com/lk-code/html-compiler-tool/main/icon_128.png)

[![.NET Version](https://img.shields.io/badge/dotnet%20version-net6.0-blue?style=flat-square)](https://www.nuget.org/packages/htmlc/)
[![License](https://img.shields.io/github/license/lk-code/html-compiler-tool.svg?style=flat-square)](https://github.com/lk-code/html-compiler-tool/blob/master/LICENSE)
[![Downloads](https://img.shields.io/nuget/dt/htmlc.svg?style=flat-square)](https://www.nuget.org/packages/htmlc/)
[![NuGet](https://img.shields.io/nuget/v/htmlc.svg?style=flat-square)](https://www.nuget.org/packages/htmlc/)

[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=lk-code_html-compiler-tool&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=lk-code_html-compiler-tool)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=lk-code_html-compiler-tool&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=lk-code_html-compiler-tool)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=lk-code_html-compiler-tool&metric=coverage)](https://sonarcloud.io/summary/new_code?id=lk-code_html-compiler-tool)

[![buy me a coffe](https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png)](https://www.buymeacoffee.com/lk.code)

This is the HTML Compiler Tool for your cli. htmlc is a small tool with which very easily static HTML files from various HTML components, including layout files and reusable HTML blocks (such as header, footer, etc.) from separate HTML files. The result is written to a complete and finished HTML file. Sass/SCSS compilation is also supported (the path to generated CSS file is then written into the HTML file). In the end you don't have to touch the generated html files.

## content

 - [content](#content)
 - [installation and update](#installation-and-update)
   - [update htmlc](#update-htmlc)
 - [usage](#usage)
 - [commands](#commands)
   - [new-command](#new-command)
     - [options](#options)
   - [config-command](#config-command)
     - [build-blacklist](#build-blacklist)
     - [template-repositories](#template-repositories)
   - [compile-command](#compile-command)
   - [watch-command](#watch-command)
 - [html files](#html-files)
   - [different html types](#different-html-types)
     - [entry html](#entry-html)
     - [layout](#layout)
     - [reusable components](#reusable-components)
   - [supported tags and its functionality](#supported-tags-and-its-functionality)
     - [The @PageTitle-Tag](#the-pagetitle-tag)
     - [The @Layout-Tag](#the-layout-tag)
     - [The @Body-Tag](#the-body-tag)
     - [The @File-Tag](#the-file-tag)
     - [The @StylePath-Tag](#the-stylepath-tag)
     - [The @Comment-Tag](#the-comment-tag)
     - [The @StartHtmlSpecialChars and @EndHtmlSpecialChars-Tag](#the-starthtmlspecialchars-and-endhtmlspecialchars-tag)

## installation and update

1.  install the .NET Runtime
you need to install the .NET Runtime (its free and available for macos, linux and windows)
* [macOS](https://learn.microsoft.com/en-us/dotnet/core/install/macos)
* [Windows](https://learn.microsoft.com/en-us/dotnet/core/install/windows)
* [Linux](https://learn.microsoft.com/en-us/dotnet/core/install/linux)

2. install the tool
then you can install the html-tool very simple via this command:

`dotnet tool install --global htmlc`

### update htmlc

`dotnet tool update --global htmlc`

## usage

The compile process searches in the folder for all HTML files. All files that do NOT start with an underscore are compiled. Files with an underscore (for example _layout.html or _footer.html) are used as reusable components.

## commands

### new-command

The new command creates a new project at the current folder location. The project contains the following elements:

- **.gitignore** - the git ignore file
- **/src** - the source directory for your files
  - **/src/index.html** - the html index file
  - **/src/shared** - the directory for alle shared components (like layout, etc.)
    - **/src/shared/_layout.html** - the layout file for all html files  
- **/dist** - the output directory

#### options
you can use the following options with the new command:

`-d --docker` - creates a simple Dockerfile with nginx configuration.

**example:** `htmlc new -d`

`-t --template` - creates a project based on the given template name. If several templates matching the search filter are found, a url can be specified (which must be available in one of the template repositories!)

**example:** `htmlc new -t Demo`

`-v --vscode` - add configuration directory for Visual Studio Code (.vscode) and settings-file.

**example:** `htmlc new -v`

`-l --vsliveserver` - add configuration for Visual Studio Code Extension [Live Server](https://marketplace.visualstudio.com/items?itemName=ritwickdey.LiveServer) (recommended) - **important:** vscode settings file needed. create via `html flag -v --vscode` needed!

**example:** `htmlc new -l`

the `vsliveserver` option creates the property `liveServer.settings.root` and sets it to the output directory in the vscode settings file

### config-command

With this command, the user configuration, which is located in the user directory as a .htmlc file (e.g. /Users/lk-code/.htmlc), can be edited. The file is hidden per default. all (!) html-files a always blocked, the compiler copies the result to the output-directory.

#### build-blacklist

With this command, a file type (e.g. *.png) can be blocked for the compilation process so that no files of this type are copied into the output directory during the asset copying process.

`htmlc config build-blacklist <add|remove> {fileextensions}`

#### template-repositories

With this command you can add more html template repositories or remove existing ones.

`htmlc config template-repositories <add|remove> {repository-url}`

**examples:**

`htmlc config build-blacklist add BaseRepository:https://url-to-repository.com` this command adds a block for all .png-files

`htmlc config build-blacklist remove .png` this command removes the entry for .png-files so all png files are copied.

### compile-command

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

### watch-command

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

## html files

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

#### The @PageTitle-Tag
The value of **PageTitle** is set as a global variable. Each time @PageTitle is used, this location is replaced with the value.

#### The @Layout-Tag
The **@Layout** tag is used in an HTML entry file to specify which layout file is to use.

#### The @Body-Tag
The **@Body** tag determines in a layout file where the content from the actual HTML entry file is written.

#### The @File-Tag
You can include another file with the **@File** tag in any HTML file (whether layout file. reusable file or entry file).

#### The @StylePath-Tag
htmlc can also compile style files (scss or sass). the path of the compiled CSS file can be inserted using this **@StylePath** tag. The following usage makes sense:<br />
`<link rel="stylesheet" href="@StylePath">`<br />

#### The @Comment-Tag
You can generate a HTML comment tag:

#### The @StartHtmlSpecialChars and @EndHtmlSpecialChars-Tag
You can escape special characters in a section HTML.
To do this, place the following tags @StartHtmlSpecialChars and @EndHtmlSpecialChars before and after the block to be escaped:

`@StartHtmlSpecialChars`<br />
`<h1>a h1 heading</h1>`<br />
`@EndHtmlSpecialChars`<br />

turns into

`&#60;h1&#62;a h1 heading&#60;/h1&#62;`<br />

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

## notices

this tool uses:
* [Cocona (MIT)](https://github.com/mayuki/Cocona) for console app environment
* [DartSassHost (MIT)](https://github.com/Taritsyn/DartSassHost) for scss compiling
