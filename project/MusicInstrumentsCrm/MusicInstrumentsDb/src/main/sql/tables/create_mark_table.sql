﻿CREATE TABLE "Mark"
(
    id      INTEGER GENERATED BY DEFAULT AS IDENTITY
        CONSTRAINT "PK_Mark"
            PRIMARY KEY,
    name    TEXT,
    country INTEGER
        CONSTRAINT "FK_Mark_Country_country"
            REFERENCES "Country"
            ON DELETE RESTRICT
);

CREATE INDEX "IX_Mark_country"
    ON "Mark" (country);
