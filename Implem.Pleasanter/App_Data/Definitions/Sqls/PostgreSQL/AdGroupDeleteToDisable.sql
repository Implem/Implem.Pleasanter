update "Groups" 
set "Disabled" = true, "Updator" = @ipU, "UpdatedTime" = CURRENT_TIMESTAMP
where (1=1)
 and ("Disabled" = false)
 and ("SynchronizedTime" is not null)
 and ("SynchronizedTime" <> @SynchronizedTime#CommandCount#)
;