Option Explicit
Dim conn, server, db, uid, pwd, path
server =     "(local)"
db =         "Implem.Pleasanter"
uid =        "sa"
pwd =        "SetSaPWD"
path =       "c:\Backup\" & _
             Replace(Replace(Replace(Now(), "/", ""), " ", "_"), ":", "") & _
             ".bak"
Set conn =   WScript.CreateObject("ADODB.Connection")
conn.Open (  "Driver={SQL Server};" & _
             "Server=" & server & ";" & _
             "Database=" & db & ";" & _
             "UID=" & uid & ";" & _
             "PWD=" & pwd & ";")
conn.Execute "BACKUP DATABASE " & _
             "[" & db & "] " & _
             "TO DISK=N'" & path & "' " & _
             "WITH NOFORMAT, " & _
             "NOINIT, " & _
             "NAME=N'" & db & "- FULL BACKUP', " & _
             "SKIP, " & _
             "NOREWIND, " & _
             "NOUNLOAD, " & _
             "STATS=10;"
conn.Execute "DBCC SHRINKDATABASE(N'Implem.Pleasanter');"
conn.Close
Set conn = Nothing

'Delete backup data more than 8 days ago
'Set wsh = CreateObject("WScript.Shell")
'wsh.Run "forfiles /p C:\Backup /d -8 /c ""cmd /c del @path"""
'Set wsh = Nothing
