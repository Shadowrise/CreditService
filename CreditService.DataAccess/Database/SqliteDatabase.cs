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
                RequestedAmount REAL NOT NULL,
                CreditRequestDate TEXT NOT NULL,
                CreditStatus TEXT NOT NULL,
                CHECK (CreditStatus IN ('Paid', 'AwaitingPayment', 'Created'))
            );
        """);
        
        await con.ExecuteAsync("""
            CREATE TABLE IF NOT EXISTS Invoice (
                InvoiceID INTEGER PRIMARY KEY AUTOINCREMENT,
                InvoiceNumber TEXT NOT NULL,
                InvoiceAmount REAL NOT NULL,
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
                ('CR-001', 'Alice Johnson', 5000.00, '2025-01-01', 'Created'),
                ('CR-002', 'Bob Smith', 10000.00, '2025-01-02', 'AwaitingPayment'),
                ('CR-003', 'Charlie Brown', 7500.00, '2025-01-03', 'Paid'),
                ('CR-004', 'Diana Prince', 15000.00, '2025-01-04', 'Created'),
                ('CR-005', 'Eve Adams', 2000.00, '2025-01-05', 'AwaitingPayment'),
                ('CR-006', 'Frank Wright', 3000.00, '2025-01-06', 'Paid'),
                ('CR-007', 'Grace Lee', 4000.00, '2025-01-07', 'Created'),
                ('CR-008', 'Hank Green', 2500.00, '2025-01-08', 'AwaitingPayment'),
                ('CR-009', 'Ivy Clark', 6000.00, '2025-01-09', 'Paid'),
                ('CR-010', 'Jack White', 8000.00, '2025-01-10', 'Created');
        """);
        
        await con.ExecuteAsync("""
            INSERT INTO Invoice (InvoiceNumber, InvoiceAmount, CreditID) VALUES
                ('INV-001', 1000.00, 1),
                ('INV-002', 2000.00, 1),
                ('INV-003', 3000.00, 2),
                ('INV-004', 1000.00, 3),
                ('INV-005', 500.00, 3),
                ('INV-006', 2500.00, 4),
                ('INV-007', 4000.00, 5),
                ('INV-008', 1500.00, 6),
                ('INV-009', 3500.00, 6),
                ('INV-010', 2000.00, 7),
                ('INV-011', 1000.00, 9),
                ('INV-012', 500.00, 10),
                ('INV-013', 3000.00, 10),
                ('INV-014', 2500.00, 10);
        """);
    }
}