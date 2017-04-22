using System;
using System.Collections;
using System.Collections.Generic;

using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Core;

namespace KMonoDevelopP4Addin
{

    public class P4AddHandler : P4HandlerBase
    {
        protected override void Run ()
        {
            var fn = IdeApp.Workbench.ActiveDocument.FileName;
			var dir = fn.ParentDirectory.FullPath.ToString();

            CheckForError(PerforceInterface.Add(dir, fn), "P4 Added file " + fn.FileName);
        }

        protected override void Update (CommandInfo info)
        {

        }   
    }
}

