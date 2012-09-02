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