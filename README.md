# htmlc - HTML Compiler

![html-compiler-tool](https://raw.githubusercontent.com/lk-code/html-compiler-tool/main/icon_128.png)

[![.NET Version](https://img.shields.io/badge/dotnet%20version-net6.0-blue?style=flat-square)](https://www.nuget.org/packages/htmlc/)
[![License](https://img.shields.io/github/license/lk-code/html-compiler-tool.svg?style=flat-square)](https://github.com/lk-code/html-compiler-tool/blob/master/LICENSE)
[![Downloads](https://img.shields.io/nuget/dt/htmlc.svg?style=flat-square)](https://www.nuget.org/packages/htmlc/)
[![NuGet](https://img.shields.io/nuget/v/htmlc.svg?style=flat-square)](https://www.nuget.org/packages/htmlc/)

[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=lk-code_html-compiler-tool&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=lk-code_html-compiler-tool)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=lk-code_html-compiler-tool&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=lk-code_html-compiler-tool)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=lk-code_html-compiler-tool&metric=coverage)](https://sonarcloud.io/summary/new_code?id=lk-code_html-compiler-tool)

[![buy me a coffe](https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png)](https://www.buymeacoffee.com/lk.code)

This is the HTML Compiler Tool for your cli. htmlc is a small tool with which very easily static HTML files from various
HTML components, including layout files and reusable HTML blocks (such as header, footer, etc.) from separate HTML
files. The result is written to a complete and finished HTML file. Sass/SCSS compilation is also supported (the path to
generated CSS file is then written into the HTML file). In the end you don't have to touch the generated html files.

## content

- [content](#content)
- [installation and update](#installation-and-update)
    - [update htmlc](#update-htmlc)
- [usage](#usage)
- [commands](#commands)
    - [new-command](#new-command)
        - [options](#options)
    - [compile-command](#compile-command)
    - [watch-command](#watch-command)
    - [environment-commands](#environment-commands)
        - [check-command](#check-command)
        - [setup-command](#setup-command)
    - [template-commands](#template-commands)
        - [create-command](#create-command)
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
        - [The @MarkdownFile-Tag](#the-markdown-file-tag)
        - [The @StylePath-Tag](#the-stylepath-tag)
        - [The @Comment-Tag](#the-comment-tag)
        - [The @Global-Tag](#the-global-tag)
        - [The @StartHtmlSpecialChars and @EndHtmlSpecialChars-Tag](#the-starthtmlspecialchars-and-endhtmlspecialchars-tag)
        - [The @Var-Tag](#the-var-tag)
        - [The @VarFile-Tag](#the-varfile-tag)
        - [The @BuildDate-Tag](#the-builddate-tag)

## installation and update

1. install the .NET Runtime
   you need to install the .NET Runtime (its free and available for macos, linux and windows)

* [macOS](https://learn.microsoft.com/en-us/dotnet/core/install/macos)
* [Windows](https://learn.microsoft.com/en-us/dotnet/core/install/windows)
* [Linux](https://learn.microsoft.com/en-us/dotnet/core/install/linux)

2. install the tool
   then you can install the html-tool very simple via this command:

```
dotnet tool install --global htmlc
```

### update htmlc

```
dotnet tool update --global htmlc
```

## usage

The compile process searches in the folder for all HTML files. All files that do NOT start with an underscore are
compiled. Files with an underscore (for example _layout.html or _footer.html) are used as reusable components.

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

`-t --template` - creates a project based on the given template name. If several templates matching the search filter
are found, a url can be specified (which must be available in one of the template repositories!)

**example:** `htmlc new -t Demo`

`-v --vscode` - add configuration directory for Visual Studio Code (.vscode) and settings-file.

**example:** `htmlc new -v`

`-l --vsliveserver` - add configuration for Visual Studio Code
Extension [Live Server](https://marketplace.visualstudio.com/items?itemName=ritwickdey.LiveServer) (recommended) - *
*important:** vscode settings file needed. create via `html flag -v --vscode` needed!

**example:** `htmlc new -l`

the `vsliveserver` option creates the property `liveServer.settings.root` and sets it to the output directory in the
vscode settings file

### compile-command

This command compiles all HTML files from the /src (rekusriv) folder and writes the results to /dist.

```
htmlc compile <project-directory>
```

If only one path is specified, htmlc searches for /src and /dist in this directory. If these do not exist, then they are
created. Then htmlc searches for files in /src and writes the results to the /dist directory. If no path is specified,
then htmlc searches for /src and /dist in the current folder.

**project-directory (optional):** the path to the project directory. for example: `/path/to/project`

```
htmlc compile <source-directory> <output-directory>
```

If two folders are specified, then htmlc uses the first value as the source path and the second value as the output
paths.

**source-directory (optional):** The path to the source directory (equivalent to /src). for
example: `/path/to/project/src`

**output-directory (optional):** The path to the output directory (equivalent to /dist). for
example: `/path/to/another/directory/output`

```
htmlc compile [...] [-s --style {path/to/main.scss}]
```

Optionally, a relative path to the style entry file can be specified with -s or -style. the path to the style file must
be specified relative to the /src directory. the relative path to the final css-file is written to the @StylePath-tags.

### watch-command

This command compiles all HTML files from the /src (rekusriv) folder and writes the results to /dist. then /src is
observed for changes and recompiled whenever a change is made.

```
htmlc watch <project-directory>
```

The watch command is identical to the compile command. The only difference is that the watch command observes the
directory after the first compile and restarts the compile process every time a change is made.

**project-directory (optional):** the path to the project directory. for example: `/path/to/project`

```
htmlc watch <source-directory> <output-directory>
```

If two folders are specified, then htmlc uses the first value as the source path and the second value as the output
paths.

**source-directory (optional):** The path to the source directory (equivalent to /src). for
example: `/path/to/project/src`

**output-directory (optional):** The path to the output directory (equivalent to /dist). for
example: `/path/to/another/directory/output`

```
htmlc watch [...] [-s --style {/path/to/main.scss}]
```

Optionally, a relative path to the style entry file can be specified with -s or -style. the path to the style file must
be specified relative to the /src directory. the relative path to the final css-file is written to the @StylePath-tags.

### environment-commands

The environment commands are used to secure the htmlc environment (e.g. all required dependencies, etc.)

#### check-command

The command checks if certain dependencies are installed:

* NodeJS
* sass
* less

The dependencies are registered in the code via dependency injection.

#### setup-command

This command executes necessary steps to install the dependencies.

### template-commands

Via the template commands htmlc provides all necessary commands to manage templates.

#### create-command

Creates a template from an htmlc project which is ready to be distributed.

* **source-directory** - the path to the source directory (equivalent to /repos/my-website).
* **output-directory (optional)** - the path to the template archive. /htmlc-templates/my-website.zip. if no path is
  given
  then the template is created in the source directory.

```
htmlc template create {source-directory} [output-directory]
```

**example:** (creates a template at /repos/my-website/template.zip)

```
htmlc template create /repos/my-website
```

## html files

### different html types

#### entry html

The compiler searches for all HTML files which do NOT start with an underscore (index.html, a-page.html, etc.). files
like _layout.html, footer.html, etc. are ignored.

for example:

`/src/index.html`<br />
`/src/pages.html`<br />
`/src/components/buttons.html`<br />

#### layout

The layout file must start with an underscore. The rest of the naming is up to you. the content consists of reusable
layout in HTML (styles and scripts, header, navigation, etc.).

for example:

`/src/_layout.html`<br />

#### reusable components

In addition, you can use other recyclable components. The file name must start with an underscore. The rest of the
naming is up to you.

for example:

`/src/_navigation.html`<br />
`/src/_header.html`<br />
`/src/_footer.html`<br />

### supported tags and its functionality

#### The @PageTitle-Tag

The value of **PageTitle** is set as a global variable. Each time @PageTitle is used, this location is replaced with the
value.

#### The @Layout-Tag

The **@Layout** tag is used in an HTML entry file to specify which layout file is to use.

#### The @Body-Tag

The **@Body** tag determines in a layout file where the content from the actual HTML entry file is written.

#### The @File-Tag

You can include another file with the **@File** tag in any HTML file (whether layout file. reusable file or entry file).

#### The @MarkdownFile-Tag

You can include markdown code from files with the **@MarkdownFile** tag in any HTML file (whether layout file. reusable
file or entry file). The Markdown code will be rendered in HTML.

#### The @StylePath-Tag

htmlc can also compile style files (scss or sass). the path of the compiled CSS file can be inserted using this *
*@StylePath** tag. The following usage makes sense:<br />

```
<link rel="stylesheet" href="@StylePath">
```

#### The @Comment-Tag

Creates an HTML comment:

```
@Comment=Example-Text
```

```
<!-- Example-Text -->`
```

#### The @Global-Tag

htmlc supports Global Variables. These are loaded from a JSON file. By default, the global.json file in the root
directory of the project is configured for this (Which file to load can be configured in the .htmlc file).

You can load all JSON entries via the @Global tag and thus write them to the HTML.

**global.json** (Global Variables File)

```
{
    "Application": {
        "Name": "title of website"
    }
}
```

**index.html** (Sample HTML File)

```
<div>
    <h1>@Global:Application:Name</h1>
</div>
```

**result**

```
<div>
    <h1>title of website</h1>
</div>
```

#### The @StartHtmlSpecialChars and @EndHtmlSpecialChars-Tag

You can escape special characters in a section HTML.
To do this, place the following tags @StartHtmlSpecialChars and @EndHtmlSpecialChars before and after the block to be
escaped:

```
@StartHtmlSpecialChars
<h1>a h1 heading</h1>
@EndHtmlSpecialChars
```

turns into

```
&#60;h1&#62;a h1 heading&#60;/h1&#62;
```

#### The @Var-Tag

htmlc supports the use of variables. JSON is always used as content. All @Var entries are merged by htmlc into a JSON
object. This means that two equal @Var calls are always overwritten by the last entry.

##### set a variable

**NOCTICE:** a htmlc variable must **always** contain JSON!

The following code is used to set a variable. htmlc merges all variables into a single JSON object.
An htmlc variable must stand alone in a line, so there must be nothing before or after it.

`@Var={"Title":"Hello World!"}`

`@Var={"Data":{"Persons":[,{"Name":"Lisa Mustermann","Username":"lmustermann"},{"Name":"Fred Conrad","Username":"fconrad"}]}}`

##### access a variable

A call to read a variable is simple. It starts with @Var and always ends with a semicolon ";". In between the path
inside the JSON object is specified. Individual levels are separated by a colon ":". Access to entries in an array are
done with an index access (The numbering always starts at 0!). For example, one accesses the 4 record with [3].

To access a variable, use this call:

`<p>@Var["Data:Persons:[1]:Name"];</p>`

**result:**

`<p>Fred Conrad</p>`

#### The @VarFile-Tag

The @VarFile tag works the same way as the @Var tag. A file name is specified behind it. This file is called in the
project and the content is loaded. This file must contain JSON. The content is then processed as with the @Var content
and added to the global variable JSON object. The access to the variable is done via @Var

### getting started with your own project

1. create a directory with two subdirectories **/src** and **/dist**. All project files must be stored under **/src**.
   The compiler writes the results under **/dist**.
2. create an initial entry file **index.html**.
3. create a layout file **_layout.html**.
4. write the following basic HTML structure in the **_layout.html** file.

```
<html>
    <head>
    </head>
    <body>
        @Body
    </body>
</html>
```

5. write the following example in **index.html**.

```
@Layout=_layout.html
<section>
    <div>Hello again</div>
</section>
```

6. open the console of your choice and change to the project directory. (**/src** and **/dist** must be in it).
7. type the following command:

```
htmlc compile
```

8. under **/dist** should now appear a file **index.html** with the following content:

```
<html>
    <head>
    </head>
    <body>
        <section>
            <div>Hello again</div>
        </section>
     </body>
</html>
```

#### The @BuildDate-Tag

The BuildDate is used to provide the date of execution. Optionally, a string can be specified for formatting.

The DateTime logic of .NET (https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings) is used.

**example:**
```
htmlc build date: @BuildDate (Depending on the language setting of the executing system)
CopyRight @BuildDate("yyyy") - My Website
Format Date @BuildDate("yyyy-MM-dd")
```

**result:**
```
htmlc build date: 10/09/2023 23:10:25 (Depending on the language setting of the executing system)
CopyRight 2023 - My Website
Format Date 2023-10-09
```

## licenses

### [Cocona (MIT)](https://github.com/mayuki/Cocona)

console app environment

```
MIT License

Copyright (c) Mayuki Sawatari <mayuki@misuzilla.org>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

### [FluentDataBuilder (MIT)](https://github.com/lk-code/fluent-data-builder)

for fluent data generation and json editing

```
MIT License

Copyright (c) 2023 Lars Krämer

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

### [HtmlAgilityPack (MIT)](https://github.com/zzzprojects/html-agility-pack/)

for html editing

```
The MIT License (MIT)
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

### [Markdig (BSD-2-Clause)](https://github.com/xoofx/markdig)

for markdown rendering

```
Copyright (c) 2018-2019, Alexandre Mutel
All rights reserved.

Redistribution and use in source and binary forms, with or without modification
, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
```

### [NLog (BSD-3-Clause)](https://github.com/NLog/NLog)

for logging

```
Copyright (c) 2004-2021 Jaroslaw Kowalski <jaak@jkowalski.net>, Kim Christensen, Julian Verdurmen

All rights reserved.

Redistribution and use in source and binary forms, with or without 
modification, are permitted provided that the following conditions 
are met:

* Redistributions of source code must retain the above copyright notice, 
  this list of conditions and the following disclaimer. 

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution. 

* Neither the name of Jaroslaw Kowalski nor the names of its 
  contributors may be used to endorse or promote products derived from this
  software without specific prior written permission. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
THE POSSIBILITY OF SUCH DAMAGE.
```


### [SkiaSharp (MIT)](https://github.com/mono/SkiaSharp)

for image rendering

```
Copyright (c) 2015-2016 Xamarin, Inc.
Copyright (c) 2017-2018 Microsoft Corporation.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
```
