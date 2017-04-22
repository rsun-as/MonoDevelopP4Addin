using System;
using System.Collections;
using System.Collections.Generic;

using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Core;


namespace KMonoDevelopP4Addin
{

	public class P4CheckoutHandler : P4HandlerBase
	{
		protected override void Run ()
		{
			var fn = IdeApp.Workbench.ActiveDocument.FileName;
			var dir = fn.ParentDirectory.FullPath.ToString();

			CheckForError(PerforceInterface.Checkout(dir, fn), "P4 Checked out file " + fn.FileName);

		} 

		protected override void Update (CommandInfo info)
		{

		}   
	}
}

