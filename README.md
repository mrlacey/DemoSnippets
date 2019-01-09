# Demo Snippets

[![Build status](https://ci.appveyor.com/api/projects/status/n2awlsnbjapax7uf?svg=true)](https://ci.appveyor.com/project/mrlacey/demosnippets)

<!-- Update the VS Gallery link after you upload the VSIX-->
Download this extension from the [VS Gallery](https://visualstudiogallery.msdn.microsoft.com/[GuidFromGallery])
or get the [CI build](http://vsixgallery.com/extension/DemoSnippets.e2d68c23-8599-40e8-b402-a57060bf3d29/).

---------------------------------------

Visual Studio extension that provides a command for adding the contents of *.demosnippets to the Toolbox.
Intended to make it easy to use code snippets in demos.

See the [change log](CHANGELOG.md) for changes and road map.

![screenshot](./art/screenshot.png)

## Features

- Store your code snippets in a file with a **.demosnippets** extension.
- Right-click on the file in Solution Explorer and select 'Add to Toolbox'

### .demosnippets file

- Comment lines start with a hash
- Labels (displayed in te toolbox) start with a hyphen
- Text between labels is included as the contents of the .



## Contribute
Check out the [contribution guidelines](CONTRIBUTING.md)
if you want to contribute to this project.

For cloning and building this project yourself, make sure
to install the
[Extensibility Tools 2015](https://visualstudiogallery.msdn.microsoft.com/ab39a092-1343-46e2-b0f1-6a3f91155aa6)
extension for Visual Studio which enables some features
used by this project.

## License
[MIT](LICENSE)