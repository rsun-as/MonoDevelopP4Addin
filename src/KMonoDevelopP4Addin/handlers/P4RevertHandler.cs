using System;
using System.Collections;
using System.Collections.Generic;

using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;


namespace KMonoDevelopP4Addin
{

    public class P4RevertHandler : P4HandlerBase
    {
        protected override void Run ()
        {
            var fn = IdeApp.Workbench.ActiveDocument.FileName;
			var dir = fn.ParentDirectory.FullPath.ToString();

            if (MonoDevelop.Ide.MessageService.Confirm("Confirm revert file " + fn + "?",
                   new AlertButton("yes")))
            {        
                CheckForError( PerforceInterface.Revert(dir, fn), "P4 Reverted file "+ fn.FileName );
            }
        }

        protected override void Update (CommandInfo info)
        {

        }   
    }
}

