using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using DotnetAiApp.Db;

namespace DotnetAiApp.Tests.Mocks;

public class DotentAiAppContextMock : DotentAiAppContext
{
    private DotentAiAppContextMock(DbContextOptions<DotentAiAppContext> options) : base(options)
    { }

    public static DotentAiAppContextMock Create()
    {
        var conn = new SqliteConnection("DataSource=:memory:");
        conn.Open();

        var options = new DbContextOptionsBuilder<DotentAiAppContext>()
            .UseSqlite(conn)
            .Options;

        var context = new DotentAiAppContextMock(options);

        context.Database.EnsureCreated();

        return context;
    }
}