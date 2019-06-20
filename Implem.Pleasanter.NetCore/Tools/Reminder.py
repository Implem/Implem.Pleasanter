#!/usr/bin/env python3
import urllib.request
import ssl
import time
from datetime import datetime
while True:
    try:
        urllib.request.urlopen(
            urllib.request.Request("http://localhost/pleasanter/reminderschedules/remind?NoLog=1")
            , context=None)   #If you must use TLS, change 'None' to 'ssl.SSLContext(ssl.PROTOCOL_TLS)'.
        print(str(datetime.now()) + ": success")
    except Exception as e:
        print(str(datetime.now()) + ": " + str(e))
    time.sleep(5)