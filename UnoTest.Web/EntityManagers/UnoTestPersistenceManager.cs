using Breeze.Persistence.EFCore;
using UnoTest.Web.Data;

public class UnoTestPersistenceManager : EFPersistenceManager<UnoTestDbContext>
{
    public UnoTestPersistenceManager(UnoTestDbContext dbContext) : base(dbContext) { }
}
