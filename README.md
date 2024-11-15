# Demo Snippets

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
![Works with Visual Studio 2019](https://img.shields.io/static/v1.svg?label=VS&message=2019&color=A853C7)
![Works with Visual Studio 2022](https://img.shields.io/static/v1.svg?label=VS&message=2022&color=A853C7)
![Visual Studio Marketplace 5 Stars](https://img.shields.io/badge/VS%20Marketplace-★★★★★-green)

[![Build](https://github.com/mrlacey/DemoSnippets/actions/workflows/build.yaml/badge.svg)](https://github.com/mrlacey/DemoSnippets/actions/workflows/build.yaml)
![Tests](https://gist.githubusercontent.com/mrlacey/c586ff0f495b4a8dd76ab0dbdf9c89e0/raw/DemoSnippets.badge.svg)

Download this extension from the [VS Marketplace(https://marketplace.visualstudio.com/items?itemName=MattLaceyLtd.DemoSnippets)].
---------------------------------------

Visual Studio extension that provides functionality relating to *.demosnippets files and interacting with the Toolbox.
Intended to make it easy to use code snippets in demos.

![screenshot](./art/screenshot.png)

## Features

- Store your code snippets in a file with a **.demosnippets** extension.
- Automatically add the contents of all .demosnippets files to the Toolbox when a solution loads. (Can be disabled in Options.)
- Automatically remove all demo snippets from the Toolbox when a solution is closed. (Can be disabled in Options.)
- Add individual files to the Toolbox by right-clicking on the file in Solution Explorer and selecting **'Add to Toolbox'**.
- Add all files to the Toolbox by right-clicking on the solution in Solution Explorer and selecting **'Add All DemoSnippets to Toolbox'**.
- Remove all DemoSnippets from the Toolbox by right-clicking on the Toolbox and selecting **'Remove All DemoSnippets'**.
- Remove any empty tabs from the Toolbox by right-clicking on the Toolbox and selecting **'Remove Empty Tabs'**.
- Automatically reload Toolbox entries when .demosnippets files are saved. (Can be disabled in Options.)
- Colorization of .demosnippets files.
- Create .demosnippets files by selecting File > New Item.

[Formatting DemoSnippets files](./Formatting.md)


See the [change log](CHANGELOG.md) for changes and road map.


## Contribute
Check out the [contribution guidelines](CONTRIBUTING.md)
if you want to contribute to this project.

For cloning and building this project yourself, make sure
to install the
[Extensibility Tools](https://visualstudiogallery.msdn.microsoft.com/ab39a092-1343-46e2-b0f1-6a3f91155aa6)
extension for Visual Studio which enables some features
used by this project.
