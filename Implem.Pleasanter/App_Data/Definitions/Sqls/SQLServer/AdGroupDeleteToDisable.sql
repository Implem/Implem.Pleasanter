update "Groups" 
set "Disabled" = 1, "Updator" = @_U, "UpdatedTime" = getdate() 
where (1=1)
 and ("Disabled" = 0)
 and ("SynchronizedTime" is not null)
 and ("SynchronizedTime" <> @SynchronizedTime#CommandCount#)
;