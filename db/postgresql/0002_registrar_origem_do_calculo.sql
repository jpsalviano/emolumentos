-- Passa a registrar qual aplicação fez o cálculo. As linhas anteriores vieram todas do desktop.

ALTER TABLE emolumentos.calculos
    ADD COLUMN IF NOT EXISTS origem VARCHAR(20) NOT NULL DEFAULT 'desktop';

-- O histórico é sempre lido do mais recente para o mais antigo.
CREATE INDEX IF NOT EXISTS ix_calculos_criado_em ON emolumentos.calculos (criado_em DESC);
