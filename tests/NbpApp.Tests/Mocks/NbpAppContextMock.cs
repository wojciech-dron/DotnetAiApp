using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NbpApp.Db;

namespace NbpApp.Tests.Mocks;

public class NbpAppContextMock : NbpAppContext
{
    private NbpAppContextMock(DbContextOptions<NbpAppContext> options) : base(options)
    { }

    public static NbpAppContextMock Create()
    {
        var conn = new SqliteConnection("DataSource=:memory:");
        conn.Open();

        var options = new DbContextOptionsBuilder<NbpAppContext>()
            .UseSqlite(conn)
            .Options;

        var context = new NbpAppContextMock(options);

        context.Database.EnsureCreated();

        return context;
    }
}