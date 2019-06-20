#!/usr/bin/env python3
import urllib.request
urllib.request.urlopen(
    urllib.request.Request("http://localhost/pleasanter/users/syncbyldap")
    , context=None)   #If you must use TLS, change 'None' to 'ssl.SSLContext(ssl.PROTOCOL_TLS)'.
