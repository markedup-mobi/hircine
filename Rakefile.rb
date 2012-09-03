$: << './'
require 'albacore'
require 'version_bumper'

#-----------------------
# Local dependencies
#-----------------------
require File.expand_path(File.dirname(__FILE__)) + '/buildscripts/project_data'
require File.expand_path(File.dirname(__FILE__)) + '/buildscripts/paths'
require File.expand_path(File.dirname(__FILE__)) + '/buildscripts/albacore_mods.rb'
require File.expand_path(File.dirname(__FILE__)) + '/buildscripts/hircine_task.rb'

#-----------------------
# Environment variables
#-----------------------
@env_buildconfigname = "Release"

def env_buildversion
	bumper_version.to_s
end

#-----------------------
# Control Flow (meant to be called directly)
#-----------------------

desc "Creates a new build of Hircine"
task :default => [:build]

desc "Builds and tests Hircine without touching version \#"
task :test => [:integration_test]

desc "Builds Hircine with new build \#"
task :build => [:bump_build, :test]

desc "Builds Hircine with new revision \#"
task :build_revision => [:bump_revision, :test]

desc "Builds Hircine with new minor version \#"
task :build_minor => [:bump_minor, :test]

desc "Builds Hircine with new major version \#"
task :build_major => [:bump_major, :test]

desc "Packs the most recent build output into NuGet packages (does not bump versions)"
task :pack => [:test, :clean_output_folders, :create_output_folders, :core_pack, :app_pack]

#Note - relies on you having an accepted API key on your system
desc "Pushes updates of all packages to NuGet.org"
task :push => [:push_core, :push_app]

#-----------------------
# Building
#-----------------------

desc "Build"
msbuild :msbuild => [:assemblyinfo] do |msb|
	msb.properties :configuration => :Release
	msb.targets :Clean, :Build #Does the equivalent of a "Rebuild Solution"
	msb.solution = File.join(Folders[:root], Files[:solution])
end

#----------------------------------
# Testing
#----------------------------------

desc "Test"
nunit :nunit_test => :msbuild do |nunit|
	nunit.command = Commands[:nunit]
	nunit.options '/framework v4.0.30319'

	nunit.assemblies "#{Folders[:hircine_tests]}/bin/#{@env_buildconfigname}/#{Files[:hircine][:test]}", "#{Folders[:hircine_core_tests]}/bin/#{@env_buildconfigname}/#{Files[:hircine_core][:test]}"
end

desc "Runs an integration test against the currenly built version of hircine"
hircine :integration_test => [:nunit_test, :set_output_folders] do |hircine|
	puts "Testing Hircine against embedded database..."
	hircine.command = File.join(Folders[:hircine_bin], Files[:hircine][:bin])
	puts "Command path %s" % hircine.command
	hircine.run_embedded = true #use an emebedded RavenDB instance
	hircine.assemblies File.join(Folders[:hircine_test_indexes_bin], Files[:hircine_test_indexes][:bin])
end

#----------------------------------
# Version Management
#----------------------------------
desc "Bumps a new build number of Hircine"
task :bump_build do
	bumper_version.bump_build
	bumper_version.write(File.join(Folders[:root], Files[:version]))
end

desc "Bumps a new revision number of Hircine"
task :bump_revision do
	bumper_version.bump_revision
	bumper_version.write(File.join(Folders[:root], Files[:version]))
end

desc "Bumps a minor release number of Hircine"
task :bump_minor do
	bumper_version.bump_minor
	bumper_version.write(File.join(Folders[:root], Files[:version]))
end

desc "Bumps a major release number of Hircine"
task :bump_major do
	bumper_version.bump_major
	bumper_version.write(File.join(Folders[:root], Files[:version]))
end

desc "Updates the assembly information for Hircine"
assemblyinfo :assemblyinfo do |asm|
	assemblyInfoPath = File.join(Folders[:src], Files[:assembly_info])

	asm.input_file = assemblyInfoPath
	asm.output_file = assemblyInfoPath

	asm.version = env_buildversion
	asm.file_version = env_buildversion
end

#----------------------------------
# Output
#----------------------------------
desc "Sets the output / bin folders based on the current build configuration"
task :set_output_folders do
	Folders[:hircine_bin] = File.join(Folders[:src], Projects[:hircine][:dir],"bin", @env_buildconfigname)
	Folders[:hircine_core_bin] = File.join(Folders[:src], Projects[:hircine_core][:dir],"bin", @env_buildconfigname)
	Folders[:hircine_test_indexes_bin] = File.join(Folders[:tests], Projects[:hircine_test_indexes][:dir],"bin", @env_buildconfigname)
end

desc "Wipes out the build folder so we have a clean slate to work with"
task :clean_output_folders => :set_output_folders do
	puts "Flushing build folder..."
	flush_dir(Folders[:out])
end

desc "Creates all of the output folders we need for ILMerge / NuGet"
task :create_output_folders => :set_output_folders do
	create_dir(Folders[:out])
	create_dir(Folders[:nuget_build])
	create_dir(Folders[:hircine_nuspec][:root])
	create_dir(Folders[:hircine_nuspec][:lib])
	create_dir(Folders[:hircine_nuspec][:net40])
	create_dir(Folders[:hircine_core_nuspec][:root])
	create_dir(Folders[:hircine_core_nuspec][:lib])
	create_dir(Folders[:hircine_core_nuspec][:net40])
end

output :core_static_output do |out|
	out.from '.'
	out.to Folders[:hircine_core_nuspec][:root]
	out.file Files[:readme]
	out.file Files[:license]
end

output :app_static_output do |out|
	out.from Folders[:root]
	out.to Folders[:hircine_nuspec][:root]
	out.file Files[:readme]
	out.file Files[:license]
end

output :core_net40_output => [:core_static_output] do |out|
	out.from Folders[:hircine_core_bin]
	create_dir(Folders[:hircine_core_nuspec][:lib])
	out.to Folders[:hircine_core_nuspec][:net40]
	out.file Files[:hircine_core][:bin]
end


output :app_net40_output => [:app_static_output] do |out|
	out.from Folders[:hircine_bin]
	create_dir(Folders[:hircine_nuspec][:lib])
	out.to Folders[:hircine_nuspec][:net40]
	out.file Files[:hircine][:bin], :as => 'hircine.exe'
end

#----------------------------------
# NuSpec
#----------------------------------

nuspec :core_nuspec do |nuspec|
	nuspec.id = Projects[:hircine_core][:id]
	nuspec.version = env_buildversion
	nuspec.authors = Projects[:hircine_core][:authors]
	nuspec.description = Projects[:hircine_core][:description]
	nuspec.title = Projects[:hircine_core][:title]
	nuspec.language = Projects[:language]
	nuspec.licenseUrl = Projects[:licenseUrl]
	nuspec.projectUrl = Projects[:projectUrl]
	nuspec.dependency "RavenDB.Embedded", "1.0.960"
	nuspec.output_file = File.join(Folders[:nuget_build], "#{Projects[:hircine_core][:id]}-v#{env_buildversion}(#{@env_buildconfigname}).nuspec")
	nuspec.tags = "ravendb, indexes, raven, index"
end

nuspec :app_nuspec do |nuspec|
	nuspec.id = Projects[:hircine][:id]
	nuspec.version = env_buildversion
	nuspec.authors = Projects[:hircine][:authors]
	nuspec.description = Projects[:hircine][:description]
	nuspec.title = Projects[:hircine][:title]
	nuspec.language = Projects[:language]
	nuspec.licenseUrl = Projects[:licenseUrl]
	nuspec.projectUrl = Projects[:projectUrl]
	nuspec.dependency "Hircine.Core", env_buildversion
	nuspec.output_file = File.join(Folders[:nuget_build], "#{Projects[:hircine][:id]}-v#{env_buildversion}(#{@env_buildconfigname}).nuspec")
	nuspec.tags = "ravendb, indexes, raven, index"
end

#----------------------------------
# NuGet (Pack)
#----------------------------------

nugetpack :core_pack => [:test, :core_net40_output, :core_nuspec] do |nuget|
	nuget.command = Commands[:nuget]
	nuget.nuspec = File.join(Folders[:nuget_build], "#{Projects[:hircine_core][:id]}-v#{env_buildversion}(#{@env_buildconfigname}).nuspec")
	nuget.base_folder = Folders[:hircine_core_nuspec][:root]
	nuget.output = Folders[:nuget_build]
end

nugetpack :app_pack => [:test, :app_net40_output, :app_nuspec] do |nuget|
	nuget.command = Commands[:nuget]
	nuget.nuspec = File.join(Folders[:nuget_build], "#{Projects[:hircine][:id]}-v#{env_buildversion}(#{@env_buildconfigname}).nuspec")
	nuget.base_folder = Folders[:hircine_nuspec][:root]
	nuget.output = Folders[:nuget_build]
end

#----------------------------------
# NuGet (Push)
#----------------------------------

desc "Publishes a new verison of the Hircine package to NuGet"
nugetpush :push_app => [:pack] do |nuget|
    nuget.command = Commands[:nuget]
    nuget.package = File.join(Folders[:root], Folders[:nuget_build], "#{Projects[:hircine][:id]}.#{env_buildversion}.nupkg")
end

desc "Publishes a new verison of the Hircine.Core package to NuGet"
nugetpush :push_core => [:pack] do |nuget|
    nuget.command = Commands[:nuget]
    nuget.package = File.join(Folders[:root], Folders[:nuget_build], "#{Projects[:hircine_core][:id]}.#{env_buildversion}.nupkg")
end