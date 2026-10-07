-- Passa a registrar qual aplicação fez o cálculo. As linhas anteriores vieram todas do desktop.

IF COL_LENGTH(N'emolumentos.calculos', N'origem') IS NULL
    ALTER TABLE emolumentos.calculos
        ADD origem VARCHAR(20) NOT NULL CONSTRAINT df_calculos_origem DEFAULT 'desktop';

-- O histórico é sempre lido do mais recente para o mais antigo.
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'ix_calculos_criado_em' AND object_id = OBJECT_ID(N'emolumentos.calculos')
)
    CREATE INDEX ix_calculos_criado_em ON emolumentos.calculos (criado_em DESC);
