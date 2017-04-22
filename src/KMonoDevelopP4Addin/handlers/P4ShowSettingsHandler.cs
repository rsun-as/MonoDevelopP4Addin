using System;
using System.Collections;
using System.Collections.Generic;

using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;


namespace KMonoDevelopP4Addin
{

    public class P4ShowSettingsHandler : P4HandlerBase
    {
        protected override void Run ()
        {
            var fn = IdeApp.Workbench.ActiveDocument.FileName.ParentDirectory.FullPath.ToString();
            MonoDevelop.Ide.MessageService.ShowMessage(PerforceInterface.GetSettings(fn));
        }

        protected override void Update (CommandInfo info)
        {

        }   
    }
}

