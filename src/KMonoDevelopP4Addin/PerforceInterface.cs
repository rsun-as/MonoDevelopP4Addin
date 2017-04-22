using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;


// todo: someday use the perforce dll libraries to do this instead of relying on commandline.

public class PerforceInterface
{
	public class Result
    {
        public int resultCode = -1;
        public string message = "";
        public string error = "";  
    }
    
	static string pathToP4 = null;
    static string Executable 
    {
        get
        {
			if (pathToP4 == null)
			{
				var enviromentPath = GetSysPath();

				string[] paths;
				string exeName;
				if (Environment.OSVersion.Platform == PlatformID.Unix)
				{
					paths = enviromentPath.Split(':');
					exeName = "p4";
				}
				else
				{
					paths = enviromentPath.Split(';');
					exeName = "p4.exe";
				}

				pathToP4 = Array.Find(paths, (string s) =>
					{
						return File.Exists( Path.Combine( s, exeName ) );
					});

				pathToP4 = Path.Combine(pathToP4, exeName);
				
			}

			return pathToP4;
        }
    }
    
	private static string GetSysPath()
	{
		if (Environment.OSVersion.Platform == PlatformID.Unix)
		{
			// necessary apparently because the default path doesn't include the login path stuff.
			// discovered here: http://howtodevelop.eu/question/n-a,101902
			var info = new ProcessStartInfo();
			info.FileName = "/bin/bash";
			info.Arguments = "-l -c \"echo $PATH\""; // -l = 'login shell' so we execute /etc/profile
			info.UseShellExecute = false;
			info.RedirectStandardOutput = true;
			info.RedirectStandardError = true;
			var p = Process.Start(info);
			p.WaitForExit();

			string path = p.StandardOutput.ReadToEnd().Trim();  // Drop the trailing \n from our echo output
			return path;
		}
		else
		{
			// TODO: do i need to do the above on windows?
			return System.Environment.GetEnvironmentVariable("PATH");
		}
	}
    
    
    public static Result RunCommand(string workingDirectory, string command)
    {
        Result result = new Result();
        string prevDirectory = System.IO.Directory.GetCurrentDirectory();
        
        try {

            System.IO.Directory.SetCurrentDirectory(workingDirectory);  

            var processStartInfo = new System.Diagnostics.ProcessStartInfo();
			processStartInfo.FileName = Executable;
            processStartInfo.Arguments = command;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.WorkingDirectory = workingDirectory;


            // This is specifically for MonoDevelop environment!
            // for SOME reason, it has PWD env variable set to "/" regardless of WorkingDirectory set above.
            // which completely breaks p4 commandline's use of P4CONFIG settings.  Have to manually set to cwd.
            processStartInfo.EnvironmentVariables["PWD"]=workingDirectory;

            if (!processStartInfo.EnvironmentVariables.ContainsKey("P4CONFIG"))
            {
                processStartInfo.EnvironmentVariables.Add("P4CONFIG", "P4CONFIG");
            }

            Process proc = System.Diagnostics.Process.Start(processStartInfo);
            bool reading = true;

            do
            {
                string outLine = proc.StandardOutput.ReadLine();
                string errLine = proc.StandardError.ReadLine();
                if (outLine != null && outLine != "") 
                {
                    result.message += outLine + "\r";
                }
                if (errLine != null && errLine != "")
                {
                    result.error += errLine + "\r";
                }
                reading = (outLine != null) || (errLine != null);
            } while (reading);

            proc.WaitForExit();
            result.resultCode = proc.ExitCode;
        } 
        catch (System.Exception ex)
        {
			System.Console.WriteLine("Failed to execute P4: " + ex.ToString());
			result.message = ex.ToString();
        }
        finally
        {
            System.IO.Directory.SetCurrentDirectory(prevDirectory);         
        }
        return result;
    }

    
    public static string GetSettings( string path )
    {
        Result result = RunCommand(path, "info");
        return result.message + "\n" + result.error;
    }
        

    
    public static Result Checkout(string workingDirectory, string filename, int changelist = -1)
    {
		
        if (changelist == -1)
        {
			return PerforceInterface.RunCommand(workingDirectory, "edit " + filename);
        }
        else
        {
			PerforceInterface.RunCommand(workingDirectory, "edit -c " + changelist + " " + filename);
			return PerforceInterface.RunCommand(workingDirectory, "reopen -c " + changelist + " " + filename);
        }
    }
    
    
    
    
    
    public static Result Add(string workingDirectory, string filePath, int changelist = -1)
    {
        if (changelist == -1)
        {
			return PerforceInterface.RunCommand(workingDirectory, "add -f " + filePath);
        }
        else
        {
			return PerforceInterface.RunCommand(workingDirectory, "add -c " + changelist + " -f " + filePath);
        }
    }
    
  

    //---------------------------------------------------------------------------------
    public static Result Revert(string currentDir, string filePath, int changelist = -1)
    {
        if (changelist == -1)
        {
			return PerforceInterface.RunCommand(currentDir, "revert " + filePath);
        }
        else
        {
			return PerforceInterface.RunCommand(currentDir, "revert -c " + changelist + " " + filePath);
        }
    }


}

