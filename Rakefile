require "rubygems"
require "bundler"
Bundler.setup

require 'albacore'
require 'version_bumper'

desc "Build"
msbuild :build do |msb|
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