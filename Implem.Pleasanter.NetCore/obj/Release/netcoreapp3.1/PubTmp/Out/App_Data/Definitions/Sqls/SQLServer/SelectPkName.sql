SELECT TOP 1
    key_const.name
FROM
    sys.tables AS tbls
        INNER JOIN sys.key_constraints AS key_const ON
            tbls.object_id = key_const.parent_object_id AND key_const.type = 'PK'
            AND tbls.name = '#TableName#'
        INNER JOIN sys.index_columns AS idx_cols ON
            key_const.parent_object_id = idx_cols.object_id
            AND key_const.unique_index_id  = idx_cols.index_id;