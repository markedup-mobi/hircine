Hircine
=======

Hircine is a stand-alone [RavenDB](http://ravendb.net/) index builder used in CI systems and automated deployments. Think of it like FluentMigrations, but for RavenDB indexes instead of SQL schemas.

Hircine currently works with [Albacore](https://github.com/Albacore/albacore) if you're using Rake for your build system.

## How to Use Hircine ##
Hircine comes in two flavors:

* hircine.exe - a stand-alone command-line interface (CLI) that you can use to execute index build jobs against RavenDB or
* Hircine.Core - the index-building engine that powers hircine.exe, available as a C# assembly for your use.

For detailed instructions, check out the [Hircine Wiki](https://github.com/markedup-mobi/hircine/wiki).

If you're brand new to Hircine, check out the [getting started](https://github.com/markedup-mobi/hircine/wiki/Getting-Started) wiki page.

## Supported Versions of RavenDB ##
Hircine has been tested and successfully used against the following STABLE versions of RavenDB (server:)

* RavenDB 1.0.960
* RavenDB 1.0.888

Hircine uses the RavenDB 1.0.960 client internally.

## License ##
Licensed under Apache 2.0, see license.txt for details.