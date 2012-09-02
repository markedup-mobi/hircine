#----------------------------------
# Environment variables for Hircine
#----------------------------------
root_folder = File.expand_path("#{File.dirname(__FILE__)}")


Folders={
	:root => root_folder,
	:src => "src",
	:out => "build",	
	:tools => "tools",
	:nunit => File.join("tools", "nunit"),
	:tests => "tests",
	:hircine_tests => File.join("tests", Projects[:hircine][:test_dir]),
	:hircine_core_tests => File.join("tests", Projects[:hircine_core][:test_dir]),

	:nuget_bin => File.join("tools", "nuget"),
	:nuget_build => File.join("build", "nuget"),
	:hircine_nuspec => File.join("build", "nuspec", Projects[:hircine][:dir]),
	:hircine_core_nuspec => File.join("build", "nuspec", Projects[:hircine_core][:dir]),

	:ilmerge => File.join("tools", "ilmerge"),

	:hircine_out => 'placeholder - specify build environment',
	:hircine_core_out => 'placeholder - specify build environment'
}

Files = {
	:solution = "Hircine.sln",
	:version = "VERSION",
	:assembly_info = File.join(Folders[:src], "SharedAssemblyInfo.cs")

	:hircine = {
		:nuspec => File.join(Folders[:hircine_nuspec], "#{Projects[:hircine][:id]}.nuspec"),
		:test => File.join(Folders[:hircine_tests], "#{Projects[:hircine][:test_dir]}.dll")
	},

	:hircine_core = {
		:nuspec => File.join(Folders[:hircine_core_nuspec], "#{Projects[:hircine_core][:id]}.nuspec"),
		:test => File.join(Folders[:hircine_core_tests], "#{Projects[:hircine_core][:test_dir]}.dll")
	}

	:ilmerge_assemblies = [
		#Hircine binaries
		'Hircine.Console.exe', 
		'Hircine.Core.dll',

		#RavenDB client assemblies
		'Raven.Abstractions.dll',  
		'Raven.Client.Lightweight.dll',
		'NLog.dll',
		'Newtonsoft.Json.dll',

		#Load RavenDB embedded server assemblies
		'Raven.Client.Embedded.dll'),
		'Raven.Database.dll', 
		'Raven.Munin.dll', 
		'Raven.Storage.Esent.dll', 
		'Raven.Storage.Managed.dll', 
		'Esent.Interop.dll',
		'BouncyCastle.Crypto.dll',
		'Lucene.Net.dll',
		'Lucene.Net.Contrib.Spatial.dll',
		'Lucene.Net.Contrib.SpellChecker.dll',
		'ICSharpCode.NRefactory.dll',
		'Spatial4n.Core.dll'
	]
}

Commands = {
	:nunit => File.join(Folders[:nunit], "nunit-console.exe"),
	:nuget => File.join(Folders[:nuget_bin], "NuGet.exe"),
	:ilmerge => File.join(Folders[:ilmerge], "ilmerge.exe")
}

#safe function for creating output directories
def create_dir(dirName)
	if !File.directory?(dirName)
		FileUtils.mkdir(dirName) #creates the /build directory
	end
end