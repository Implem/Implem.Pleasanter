IF not EXISTS
(
	SELECT 
		*
	FROM 
		sys.tables AS tbls
	INNER JOIN 
		sys.columns AS cols
	ON 
		tbls.object_id = cols.object_id
		AND tbls.name = 'Items'
		AND cols.name = 'FullText'
	INNER JOIN
		sys.fulltext_index_columns AS full_cols
	ON
		cols.object_id = full_cols.object_id
		AND cols.column_id = full_cols.column_id
)
BEGIN
	if not Exists(SELECT * FROM sys.fulltext_catalogs AS full_ctlgs WHERE full_ctlgs.name = 'ftx')
	BEGIN
		CREATE FULLTEXT CATALOG ftx 
		WITH ACCENT_SENSITIVITY = OFF;  
	END;
	CREATE FULLTEXT INDEX ON [Items]
		([FullText] Language 'Japanese') 
		KEY INDEX #PKItems#
		ON ftx;
END;

IF not EXISTS
(
	SELECT 
		*
	FROM 
		sys.tables AS tbls
	INNER JOIN 
		sys.columns AS cols
	ON 
		tbls.object_id = cols.object_id
		AND tbls.name = 'Binaries'
		AND cols.name = 'Bin'
	INNER JOIN
		sys.fulltext_index_columns AS full_cols
	ON
		cols.object_id = full_cols.object_id
		AND cols.column_id = full_cols.column_id
)
BEGIN
	if not Exists(SELECT * FROM sys.fulltext_catalogs AS full_ctlgs WHERE full_ctlgs.name = 'ftx')
	BEGIN
		CREATE FULLTEXT CATALOG ftx 
		WITH ACCENT_SENSITIVITY = OFF;  
	END;
	CREATE FULLTEXT INDEX ON [Binaries]
		([Bin] TYPE COLUMN Extension Language 'Japanese' ) 
		KEY INDEX #PKBinaries#
		ON ftx;
END;