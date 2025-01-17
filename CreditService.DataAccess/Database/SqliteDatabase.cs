using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace CreditService.Infrastructure.Database;

class SqliteDatabase : IDatabase
{
    private readonly string _connectionString;
    
    public SqliteDatabase(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("CreditServiceDb")!;
    }
    
    public IDbConnection CreateConnection()
    {
        var con = new SqliteConnection(_connectionString);
        con.Open();
        return con;
    }

    public async Task Init()
    {
        using var con = CreateConnection();

        await con.ExecuteAsync("""
            CREATE TABLE IF NOT EXISTS Credit (
                CreditID INTEGER PRIMARY KEY AUTOINCREMENT,
                CreditNumber TEXT NOT NULL,
                ClientName TEXT NOT NULL,
                RequestedAmount INTEGER NOT NULL,
                CreditRequestDate TEXT NOT NULL,
                CreditStatus TEXT NOT NULL,
                CHECK (CreditStatus IN ('Paid', 'AwaitingPayment', 'Created'))
            );
        """);
        
        await con.ExecuteAsync("""
            CREATE TABLE IF NOT EXISTS Invoice (
                InvoiceID INTEGER PRIMARY KEY AUTOINCREMENT,
                InvoiceNumber TEXT NOT NULL,
                InvoiceAmount INTEGER NOT NULL,
                CreditID INTEGER NOT NULL,
                FOREIGN KEY (CreditID) REFERENCES Credit(CreditID)
                    ON DELETE CASCADE ON UPDATE CASCADE
            );
        """);
    }

    public async Task Seed()
    {
        using var con = CreateConnection();

        await con.ExecuteAsync("""
            INSERT INTO Credit (CreditNumber, ClientName, RequestedAmount, CreditRequestDate, CreditStatus) VALUES
                ('CR-001', 'Alice Johnson', 500012, '2025-01-01', 'Created'),
                ('CR-002', 'Bob Smith', 1000000, '2025-01-02', 'AwaitingPayment'),
                ('CR-003', 'Charlie Brown', 750044, '2025-01-03', 'Paid'),
                ('CR-004', 'Diana Prince', 1500000, '2025-01-04', 'Created'),
                ('CR-005', 'Eve Adams', 200000, '2025-01-05', 'AwaitingPayment'),
                ('CR-006', 'Frank Wright', 300155, '2025-01-06', 'Paid'),
                ('CR-007', 'Grace Lee', 400000, '2025-01-07', 'Created'),
                ('CR-008', 'Hank Green', 250000, '2025-01-08', 'AwaitingPayment'),
                ('CR-009', 'Ivy Clark', 600000, '2025-01-09', 'Paid'),
                ('CR-010', 'Jack White', 800000, '2025-01-10', 'Created');
        """);
        
        await con.ExecuteAsync("""
            INSERT INTO Invoice (InvoiceNumber, InvoiceAmount, CreditID) VALUES
                ('INV-001', 100045, 1),
                ('INV-002', 200000, 1),
                ('INV-003', 300055, 2),
                ('INV-004', 100003, 3),
                ('INV-005', 50000, 3),
                ('INV-006', 250077, 4),
                ('INV-007', 400000, 5),
                ('INV-008', 150000, 6),
                ('INV-009', 350000, 6),
                ('INV-010', 200000, 7),
                ('INV-011', 100000, 9),
                ('INV-012', 50000, 10),
                ('INV-013', 300000, 10),
                ('INV-014', 250000, 10);
        """);
    }
}