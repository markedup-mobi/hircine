require "rubygems"
require "bundler"
Bundler.setup

require 'albacore'
require 'version_bumper'

def env_buildversion
	bumper_version.to_s
end

def core_assembly_path(assemblyFile)
	"src/Hircine.Core/bin/release/%{fileName}" % {:fileName => assemblyFile}
end

def app_assembly_path(assemblyFile)
	"src/Hircine.Console/bin/release/%{fileName}" % {:fileName => assemblyFile}
end

#safe function for creating output directories
def create_dir(dirName)
	if !File.directory?(dirName)
		FileUtils.mkdir(dirName) #creates the /build directory
	end
end

def ilmerge_assemblies
	a = [
		#Load Hircine assemblies
		app_assembly_path('Hircine.Console.exe'), 
		app_assembly_path('Hircine.Core.dll'),

		#Load RavenDB client assemblies
		app_assembly_path('Raven.Abstractions.dll'),  
		app_assembly_path('Raven.Client.Lightweight.dll'),
		app_assembly_path('NLog.dll'),
		app_assembly_path('Newtonsoft.Json.dll'),

		#Load RavenDB embedded server assemblies
		app_assembly_path('Raven.Client.Embedded.dll'),
		app_assembly_path('Raven.Database.dll'), 
		app_assembly_path('Raven.Munin.dll'), 
		app_assembly_path('Raven.Storage.Esent.dll'), 
		app_assembly_path('Raven.Storage.Managed.dll'), 
		app_assembly_path('Esent.Interop.dll'),
		app_assembly_path('BouncyCastle.Crypto.dll'),
		app_assembly_path('Lucene.Net.dll'),
		app_assembly_path('Lucene.Net.Contrib.Spatial.dll'),
		app_assembly_path('Lucene.Net.Contrib.SpellChecker.dll'),
		app_assembly_path('ICSharpCode.NRefactory.dll'),
		app_assembly_path('Spatial4n.Core.dll')
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

desc "Creates all of the output folders we need for ILMerge / NuGet"
task :createOutputFolders do
	create_dir('build')
	create_dir('build/core')
	create_dir('build/core/net40')
	create_dir('build/app')
	create_dir('build/app/net40')
	create_dir('build/packages')
end

desc "merges all of the assemblies needed to run hircine"
exec :merge => :createOutputFolders do |cmd|
	cmd.command = 'tools/ilmerge/ilmerge.exe'
	cmd.parameters = "/out:build/hircine.exe %{assemblies}" % { assemblies: ilmerge_assemblies.map{|s| "#{s}"}.join(' ')}
	puts "Merging"
end

output :core_output => [:createOutputFolders] do |out|
	out.from '.'
	out.to 'build/core'
	out.file core_assembly_path('Hircine.Core.dll'), :as => 'Hircine.Core.dll'
	out.file 'README.md'
	out.file 'license.txt'
	out.file 'VERSION'
end

output :app_output => [:createOutputFolders] do |out|
	out.from '.'
	out.to 'build/app'
	out.file app_assembly_path('Hircine.Console.exe'), :as => 'net40/Hircine.exe'
	out.file 'README.md'
	out.file 'license.txt'
	out.file 'VERSION'
end

nuspec :core_nuspec do |nuspec|
	nuspec.id = "Hircine.Core"
	nuspec.version = env_buildversion
	nuspec.authors = "Aaron Stannard"
	nuspec.description = "Core engine for stand-alone RavenDB index builder, used in CI systems and automated deployments. See Hircine package for the runnable tool."
	nuspec.title = "Hircine.Core"
	nuspec.language = "en-US"
	nuspec.licenseUrl = "https://github.com/markedup-mobi/hircine/blob/master/license.txt"
	nuspec.projectUrl = "https://github.com/markedup-mobi/hircine"
	nuspec.dependency "RavenDB.Embedded", "1.0.960"
	nuspec.output_file = "build/hircine.core.nuspec"
	nuspec.tags = "ravendb, indexes, raven, index"
end

nuspec :app_nuspec do |nuspec|
	nuspec.id = "Hircine"
	nuspec.version = env_buildversion
	nuspec.authors = "Aaron Stannard"
	nuspec.description = "Runtime for stand-alone RavenDB index builder, used in CI systems and automated deployments."
	nuspec.title = "Hircine"
	nuspec.language = "en-US"
	nuspec.licenseUrl = "https://github.com/markedup-mobi/hircine/blob/master/license.txt"
	nuspec.projectUrl = "https://github.com/markedup-mobi/hircine"
	nuspec.dependency "Hircine.Core", env_buildversion
	nuspec.output_file = "build/hircine.nuspec"
	nuspec.tags = "ravendb, indexes, raven, index"
end

nugetpack :core_pack => [:core_nuspec] do |nuget|
	nuget.command = "tools/nuget/NuGet.exe"
	nuget.nuspec = "build/hircine.core.nuspec"
	nuget.base_folder = "./build/core/"
	nuget.output = "build/packages/"
end

nugetpack :app_pack => [:app_nuspec] do |nuget|
	nuget.command = "tools/nuget/NuGet.exe"
	nuget.nuspec = "build/hircine.nuspec"
	nuget.base_folder = "./build/app/"
	nuget.output = "build/packages/"
end

task :pack => [:test, :core_output, :app_output, :core_pack, :app_pack] do
	puts "Packing NuGet packages..."
end