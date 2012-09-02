$: << './'
require "rubygems"
require "bundler"
Bundler.setup

require 'albacore'
require 'version_bumper'

#-----------------------
# Local dependencies
#-----------------------
require 'buildscripts/project_data'
require 'buildscripts/paths'

#-----------------------
# Environment variables
#-----------------------
@env_buildconfigname = "Release"

def env_buildversion
	bumper_version.to_s
end

desc "Build"
msbuild :build => :assemblyinfo do |msb|
	msb.properties :configuration => :Release
	msb.targets :Clean, :Build #Does the equivalent of a "Rebuild Solution"
	msb.solution = Files[:solution]
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