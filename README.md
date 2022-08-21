# html-compiler-tool

This is the HTML Compiler Tool for your cli

## install the tool

`dotnet tool install --global htmlc`

## update the tool

`dotnet tool update --global htmlc`

## usage

generates HTML from the template file demo.html and writes it into the folder ".\output" with the same file name ".\output\demo.html":<br />
`htmlc compile .\source\demo.html .\output\`

generates HTML from the template file test.html and writes it into the file ".\output\sample.html":<br />
`htmlc compile .\source\test.html .\output\sample.html`

### specify a template file
this file contains only the individual content:

`@Layout=_layout.html`<br />
`<section>`<br />
`    <div>Hello again</div>`<br />
`</section>`<br />

**important:** add the ***@Layout***-element to specify which file the layout is in.

### specify a layout file
The layout file contains the HTML framework, which is identical for all generated files:<br />
`<html>`<br />
`    <head>`<br />
`        ...`<br />
`    </head>`<br />
`    <body>`<br />
`        @Body`<br />
`    </body>`<br />
`</html>`<br />

**important:** add the ***@Body***-element to specify where the content of the page should be written.

### generated output

The above example generates the following code:
`<html>`<br />
`    <head>`<br />
`        ...`<br />
`    </head>`<br />
`    <body>`<br />
`<section>`<br />
`    <div>Hello again</div>`<br />
`</section>`<br />
`    </body>`<br />
`</html>`<br />