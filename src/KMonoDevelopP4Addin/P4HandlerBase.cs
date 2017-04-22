using System;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Core;

namespace KMonoDevelopP4Addin
{
    public class P4HandlerBase : CommandHandler
    {
        public bool CheckForError( PerforceInterface.Result result, string successMsg )
        {
            if (result.resultCode != 0)
            {
                MonoDevelop.Ide.MessageService.ShowMessage("P4 error:\n" + result.message + "\n" + result.error);
                return true;
            }

            using (IProgressMonitor m = IdeApp.Workbench.ProgressMonitors.GetToolOutputProgressMonitor(false))
            {
                m.ReportSuccess(successMsg);
            }

            return false;
        }
    }
}

