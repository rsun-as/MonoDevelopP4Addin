# MonoDevelopP4Addin

This is a simple MonoDevelop Addin, allowing simple MonoDevelop p4 integration, that you can apply keyboard shortcuts to.
Note that this was built with Unity's distributed MonoDevelop in mind, and hasn't been tested outside of this environment.

To Install:
Go to MonoDevelop-Unity->Add In Manager...   (On Windows, I believe it's Tools->Addin Manager...)
Click "Install from file..."

Navigate to where you cloned this repo to (or saved the mpack file to)
Select the .mpack file
Make sure your p4 environment is set up to your liking.  
(p4 set, need at minimum p4CLIENT and p4PORT set, which is beyond the scope of this doc).  
several operations are now available for p4 in the Version Control pulldown, all of which can be shortcutted if desired.  By default, on Mac, checkout is ^E  (ctrl-e).  If you just start typing in a file, it will attempt to automatically check out the file for you.

 
Some quirky things to be aware of

P4 setup.
Some people have experienced issues with getting their commandline p4 setup functioning correctly.  This does require it to be in your environment path.

MonoDevelop's handling of read-only checking
As it turns out, MonoDevelop's internal code stupidly only checks for readonly flag on the FIRST time you edit the file, so the hook to check if it needs checking out only happens the first time you edit it.  Thus, if you check in or revert, it won't know to check it out again on edit.  If you close the file entirely and reopen it will check again.  HOWEVER, it does always check on save, so if you try to save in this situation, it will try to auto-checkout again.  Don't panic!  it will be ok.
It doesn't work in some scenarios due to limitations in MonoDevelop
In some scenarios, MonoDevelop will modify files under your nose and there's no chance to hook into the process to check things out.  
One such scenario is if you use the code Refactoring tools that MonoDevelop offers.  Under the hood, the refactor system ultimately makes calls directly to the C# OS file calls to write to a temp file and then move it over the original.  There's no opportunity for the plugin to catch this.  There may be other situations where MonoDevelop fails to respect its own internal FileSystem class that would cause this as well, so keep an eye out if using features that modify files beyond editing them directly. 

How to Build the AddIn from Source
Working on this addin currently requires the AddIn Maker plugin to be installed from the gallery

Without this, the project won't even load.
At that point, if your configuration in the project is Debug, hitting "run" will automatically open a new MonoDevelop instance w/ the debug addin installed. You can debug as normal.
If you build in Release, it automatically runs a shell script to package the addin for installing from file.  Note that this script is currently set up to assume Unity is installed in default location (on Mac) at /Applications/Unity.
NOTE: there's some wacky crap going on, where if the Addin Maker is installed and enabled, it will fail to install without an error.  Disabling the Addin Maker add in temporarily while installing will resolve the issue. Dumb. 
 
 
This Add in was originally written at Kabam, Inc / Aftershock by me and was approved for open sourcing in 2017
