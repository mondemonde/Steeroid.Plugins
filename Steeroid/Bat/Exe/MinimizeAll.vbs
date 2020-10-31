'set oShellApp = CreateObject("Shell.Application")
'oShellApp.MinimizeAll
'oShellApp.ShellExecute "cmd.exe"
dim objShell
        
set objShell = CreateObject("shell.application")
objShell.MinimizeAll

set objShell = nothing
