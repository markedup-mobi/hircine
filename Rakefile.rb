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

desc "Build"
msbuild :build => [:assemblyinfo] do |msb|
	msb.properties :configuration => :Release
	msb.targets :Clean, :Build #Does the equivalent of a "Rebuild Solution"
	msb.solution = File.join(Folders[:root], Files[:solution])
end

desc "Test"
nunit :test => :build do |nunit|
	nunit.command = Commands[:nunit]
	nunit.options '/framework v4.0.30319'

	nunit.assemblies "#{Folders[:hircine_tests]}/bin/#{@env_buildconfigname}/#{Files[:hircine][:test]}", "#{Folders[:hircine_core_tests]}/bin/#{@env_buildconfigname}/#{Files[:hircine_core][:test]}"
end

#Task for bumping the version number
desc "Bumps a new version of Hircine"
task :bumpVersion do
	bumper_version.bump_build
	bumper_version.write(File.join(Folders[:root], Files[:version]))
end

desc "Updates the assembly information for Hircine"
assemblyinfo :assemblyinfo => :bumpVersion do |asm|
	assemblyInfoPath = File.join(Folders[:src], Files[:assembly_info])

	asm.input_file = assemblyInfoPath
	asm.output_file = assemblyInfoPath

	asm.version = env_buildversion
	asm.file_version = env_buildversion
end

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

desc "Runs an integration test against the currenly built version of hircine"
hircine :integration_test => [:test, :set_output_folders] do |hircine|
	puts "Testing Hircine against embedded database..."
	hircine.command = File.join(Folders[:hircine_bin], Files[:hircine][:bin])
	puts "Command path %s" % hircine.command
	hircine.run_embedded = true #use an emebedded RavenDB instance
	hircine.assemblies File.join(Folders[:hircine_test_indexes_bin], Files[:hircine_test_indexes][:bin])
end

task :pack => [:integration_test, :clean_output_folders, :create_output_folders, :core_pack, :app_pack] do
	puts "Packing NuGet packages..."
end

