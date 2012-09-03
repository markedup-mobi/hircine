#----------------------------------
# Project data for Hircine
#----------------------------------

Projects = {
	:language => "en-US",
	:licenseUrl => "https://github.com/markedup-mobi/hircine/blob/master/license.txt",
	:projectUrl => "https://github.com/markedup-mobi/hircine",

	:hircine => {
		:id => "Hircine",
		:description => "Stand-alone RavenDB index builder, used in CI systems and automated deployments",
		:authors => "Aaron Stannard",
		:dir => "Hircine.Console",
		:test_dir => "Hircine.Console.Tests",
		:title => "Hircine"
	},
	:hircine_core => {
		:id => "Hircine.Core",
		:description => "Core engine for stand-alone RavenDB index builder, used in CI systems and automated deployments.",
		:authors => "Aaron Stannard",
		:dir => "Hircine.Core",
		:test_dir => "Hircine.Core.Tests",
		:title => "Hircine Core"
	},
	:hircine_test_indexes => {
		:id => "Hircine.TestIndexes",
		:description => "Sample test indexes used in the course of testing Hircine.",
		:authors => "Aaron Stannard",
		:dir => "Hircine.TestIndexes",
		:title => "Hircine RavenDB Test Indexes"
	}
}