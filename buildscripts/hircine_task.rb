# Albacore build task for running Hircine

require 'albacore/albacoretask'
class Hircine
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
		@run_embedded = false
		@sequential_jobs = false
		@continue_jobs_on_failure = false
		@supress_ssl_error = false
		super()
		@command = command unless command.nil?
	end

	def execute

		fail_with_message 'Must include at least 1 assembly' if @assemblies.nil? || @assemblies.length == 0
		fail_with_message 'Must include at least 1 connection string or set the run_embedded flag to true' if !@run_embedded && (@connection_strings.nil? || @connection_strings.length == 0)

		params = []
		if run_embedded
			params << '-e'
		else
			params << build_connection_strings
		end
		params << '-s' if @sequential_jobs
		params << '-f' if @continue_jobs_on_failure
		params << '-n' if @supress_ssl_error
		params << build_assemblies

    	commandline = params.join(" ")
    	@logger.debug "Build Hircine Command Line: " + commandline
    	result = run_command "Hircine", commandline

    	failure_message = "Hircine failed. See Build log for details."
    	fail_with_message failure_message if !result
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