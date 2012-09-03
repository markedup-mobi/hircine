# Albacore build task for running Hircine

require 'albacore/albacoretask'
class HircineBuild
	include Albacore::Task 
	include Albacore::RunCommand

	attr_accessor	:command,
					:run_embedded,
					:sequential_jobs,
					:continue_jobs_on_failure,
					:supress_ssl_error

	attr_array 		:assemblies, :connection_strings

	def initialize(command=nil)
		@assemblies = []
		@connection_strings = []
		super()
		@command = command unless command.nil?
	end

	def execute
		command_params = []
		command_params << @command
    	command_params << get_command_parameters
    	commandline = command_params.join(" ")
    	@logger.debug "Build Hircine Command Line: " + commandline
    	commandline
	end

	def build_assemblies
		assembly_text = []
    	@assemblies.each do |value|
      		assembly_text << "-a \"#{value}\""
  		end
  		assembly_text.join(" ")
	end

	def build_connection_strings
		conn_text = []
    	@connection_strings.each do |value|
      		conn_text << "-c \"#{value}\""
  		end
  		conn_text.join(" ")
    end
end