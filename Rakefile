require "rubygems"
require "bundler"
Bundler.setup

require 'albacore'
require 'version_bumper'

def env_buildversion
	bumper_version.to_s
end

def assembly_path(assemblyFile)
	"src/Hircine.Console/bin/release/%{fileName}" % {:fileName => assemblyFile}
end

def ilmerge_assemblies
	a = [
		#Load Hircine assemblies
		assembly_path('Hircine.Console.exe'), 
		assembly_path('Hircine.Core.dll'),

		#Load RavenDB client assemblies
		assembly_path('Raven.Abstractions.dll'),  
		assembly_path('Raven.Client.Lightweight.dll'),
		assembly_path('NLog.dll'),
		assembly_path('Newtonsoft.Json.dll'),

		#Load RavenDB embedded server assemblies
		assembly_path('Raven.Client.Embedded.dll'),
		assembly_path('Raven.Database.dll'), 
		assembly_path('Raven.Munin.dll'), 
		assembly_path('Raven.Storage.Esent.dll'), 
		assembly_path('Raven.Storage.Managed.dll'), 
		assembly_path('Esent.Interop.dll'),
		assembly_path('BouncyCastle.Crypto.dll'),
		assembly_path('Lucene.Net.dll'),
		assembly_path('Lucene.Net.Contrib.Spatial.dll'),
		assembly_path('Lucene.Net.Contrib.SpellChecker.dll'),
		assembly_path('ICSharpCode.NRefactory.dll'),
		assembly_path('Spatial4n.Core.dll')
	]
end

desc "Build"
msbuild :build => :assemblyinfo do |msb|
	msb.properties :configuration => :Release
	msb.targets :Clean, :Build #Does the equivalent of a "Rebuild Solution"
	msb.solution = "Hircine.sln"
end

desc "Test"
nunit :test => :build do |nunit|
	nunit.command = "tools/nunit/nunit-console.exe"
	nunit.options '/framework v4.0.30319'

	nunit.assemblies "tests/Hircine.Core.Tests/bin/release/Hircine.Core.Tests.dll", "tests/Hircine.Console.Tests/bin/release/Hircine.Console.Tests.dll"
end

#Task for bumping the version number
desc "Bumps a new version of Hircine"
task :bumpVersion do
	bumper_version.bump_build
	bumper_version.write('VERSION')
end

desc "Updates the assembly information for Hircine"
assemblyinfo :assemblyinfo => :bumpVersion do |asm|
	assemblyInfoPath = "src/Hircine.Console/Properties/AssemblyInfo.cs"

	asm.input_file = assemblyInfoPath
	asm.output_file = assemblyInfoPath

	asm.version = env_buildversion
	asm.file_version = env_buildversion

	asm.title = "hircine.exe"
	asm.description = "Stand-alone RavenDB index builder, used in CI systems and automated deployments"
	asm.company_name = "MarkedUp LLC"
	asm.product_name = "Hircine"
	asm.copyright = "MarkedUp LLC (c) 2012"
end

desc "Merges all of the assemblies needed to run Hircine"
ILMerge :merge do |cfg|
	cfg.command = 'tools/ilmerge/ILMerge.exe'
	cfg.assemblies = ilmerge_assemblies 
	cfg.output = 'build/hircine.exe'
end