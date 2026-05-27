import { describe, it, expect } from "vitest";
import { assertReadOnly, parseConnectionString } from "./db.js";

describe("assertReadOnly (read-only zaštita)", () => {
  it("dozvoljava SELECT upite", () => {
    expect(() => assertReadOnly("SELECT * FROM Tickets")).not.toThrow();
    expect(() => assertReadOnly("WITH x AS (SELECT 1) SELECT * FROM x")).not.toThrow();
  });

  it("odbija INSERT/UPDATE/DELETE", () => {
    expect(() => assertReadOnly("INSERT INTO Tickets VALUES (1)")).toThrow();
    expect(() => assertReadOnly("UPDATE Tickets SET Title = 'x'")).toThrow();
    expect(() => assertReadOnly("DELETE FROM Tickets")).toThrow();
  });

  it("odbija DROP/ALTER/TRUNCATE/MERGE/EXEC", () => {
    expect(() => assertReadOnly("DROP TABLE Tickets")).toThrow();
    expect(() => assertReadOnly("ALTER TABLE Tickets ADD x int")).toThrow();
    expect(() => assertReadOnly("TRUNCATE TABLE Tickets")).toThrow();
    expect(() => assertReadOnly("MERGE INTO Tickets")).toThrow();
    expect(() => assertReadOnly("EXEC sp_who")).toThrow();
  });
});

describe("parseConnectionString (ADO.NET -> mssql)", () => {
  it("parsira server, port, bazu i kredencijale", () => {
    const cfg = parseConnectionString(
      "Server=sqlserver,1433;Database=TelecomSupportDb;User Id=sa;Password=Secret123;TrustServerCertificate=True;"
    );
    expect(cfg.server).toBe("sqlserver");
    expect(cfg.port).toBe(1433);
    expect(cfg.database).toBe("TelecomSupportDb");
    expect(cfg.user).toBe("sa");
    expect(cfg.password).toBe("Secret123");
    expect(cfg.options?.trustServerCertificate).toBe(true);
  });

  it("koristi default port kada nije naveden", () => {
    const cfg = parseConnectionString("Server=localhost;Database=Db;User Id=sa;Password=p");
    expect(cfg.port).toBe(1433);
  });
});
