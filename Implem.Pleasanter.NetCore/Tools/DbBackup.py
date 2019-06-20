#!/usr/bin/env python3
from datetime import datetime
import os
import pyodbc
timeout = 3600
server = "(local)"
db = "Implem.Pleasanter"
uid = "sa"
pwd = "SetSaPWD"
dest = "/var/opt/mssql/data/backup/Implem.Pleasanter/"
file = datetime.now().strftime("%Y%m%d_%H%M%S") + ".bak"
con = cur = None
try:
    con = pyodbc.connect("Driver=ODBC Driver 17 for SQL Server;"
        + "Server=" + server + ";"
        + "Database=" + db + ";"
        + "UID=" + uid + ";"
        + "PWD=" + pwd + ";"
        , timeout=timeout
        , autocommit=True)
    cur = con.cursor()
    cur.execute("BACKUP DATABASE "
        + "[" + db + "] "
        + "TO DISK=N'" + dest + file + "' "
        + "WITH NOFORMAT, "
        + "NOINIT, "
        + "NAME='" + db + "- FULL BACKUP', "
        + "SKIP, "
        + "NOREWIND, "
        + "NOUNLOAD, "
        + "STATS=10;")
    while cur.nextset():
          pass
    print("Backedup: " + dest + file)
    cur.execute("DBCC SHRINKDATABASE('" + db + "');")
    while cur.nextset():
        pass
    print("Shrinked: " + db)
finally:
    if cur is not None:
        cur.close()
    if con is not None:
        con.close()
#Delete backup data more than 8 days ago
#for f in os.listdir(dest):
#    if (datetime.now() - datetime.fromtimestamp(os.stat(dest + f).st_mtime)).days >= 8:
#        os.remove(dest + f)
#        print("Deleted: " + dest + f)