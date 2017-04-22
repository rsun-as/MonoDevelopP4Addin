using System;
using System.Linq;
using System.IO;
using MonoDevelop.Core;
using System.Collections.Generic;
using MonoDevelop.Core.FileSystem;
using MonoDevelop.Ide;

namespace KMonoDevelopP4Addin
{
    public class P4FileSystemExtension : FileSystemExtension
    {
        public override bool CanHandlePath (FilePath path, bool isDirectory)
        {
            return !isDirectory;
        }

        public override void RequestFileEdit (IEnumerable<FilePath> files)
        {
            base.RequestFileEdit (files);

            if (!IdeApp.IsInitialized)
                return;

            // the rest of this code is gaffled from MonoDevelop source code IdeFileSystemExtensionExtension, and modified.
            List<FilePath> readOnlyFiles = new List<FilePath> ();
            foreach (var f in files) {
                if (File.Exists (f) && File.GetAttributes (f).HasFlag (FileAttributes.ReadOnly))
                    readOnlyFiles.Add (f);
            }
            string error;
            List<FilePath> failedToCheckOut = new List<FilePath>();

            if (readOnlyFiles.Count > 0)
            {
                foreach (var f in readOnlyFiles)
                {
					var result = PerforceInterface.Checkout(f.ParentDirectory.FullPath.ToString(), f);

                    Console.WriteLine("auto checked out: " + f);

                    if (result.resultCode != 0)
                    {
                        failedToCheckOut.Add(f);
                    }
                    else
                    {
                        using (IProgressMonitor m = IdeApp.Workbench.ProgressMonitors.GetToolOutputProgressMonitor(false))
                        {
                            m.ReportSuccess("P4 auto checked out " + f.FileName);
                        }                    
                    }
                }
            }

            if (failedToCheckOut.Count == 1)
                error = GettextCatalog.GetString ("Failed to check out: File {0} is read-only", failedToCheckOut [0].FileName);
            else if (failedToCheckOut.Count > 1) {
                var f1 = string.Join (", ", failedToCheckOut.Take (failedToCheckOut.Count - 1).ToArray ());
                var f2 = failedToCheckOut [failedToCheckOut.Count - 1];
                error = GettextCatalog.GetString ("Failed to check out: Files {0} and {1} are read-only", f1, f2);
            } else
                return;

            var btn = new AlertButton (GettextCatalog.GetString ("Make Writable"));
            var res = MessageService.AskQuestion (error, GettextCatalog.GetString ("Would you like {0} to attempt to make the file writable and try again?", BrandingService.ApplicationName), btn, AlertButton.Cancel);
            if (res == AlertButton.Cancel)
                throw new UserException (error) { AlreadyReportedToUser = true };

            foreach (var f in failedToCheckOut) {
                var atts = File.GetAttributes (f);
                File.SetAttributes (f, atts & ~FileAttributes.ReadOnly);
            }
        
        }   
    }
}

