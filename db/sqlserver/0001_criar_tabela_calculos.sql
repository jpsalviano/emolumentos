-- Histórico de cálculos. Idempotente: pode rodar quantas vezes for, em banco novo ou já migrado.

-- CREATE SCHEMA precisa ser o único comando do lote, daí o EXEC.
IF SCHEMA_ID(N'emolumentos') IS NULL
    EXEC(N'CREATE SCHEMA emolumentos');

IF OBJECT_ID(N'emolumentos.calculos', N'U') IS NULL
    CREATE TABLE emolumentos.calculos (
        id              BIGINT IDENTITY(1, 1) NOT NULL CONSTRAINT pk_calculos PRIMARY KEY,
        criado_em       DATETIME2(3)   NOT NULL, -- UTC
        uf              CHAR(2)        NOT NULL,
        ato             VARCHAR(40)    NOT NULL,
        codigo_ato      VARCHAR(20)    NOT NULL,
        quantidade      INT            NOT NULL,
        valor_declarado DECIMAL(18, 2) NOT NULL,
        total           DECIMAL(18, 2) NOT NULL
    );
