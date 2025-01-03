using gRPC_AspNetCore.Models;
using Microsoft.EntityFrameworkCore;

namespace gRPC_AspNetCore.Context
{
    public class GrpcContext:DbContext
    {
        #region Constructor
        public GrpcContext(DbContextOptions<GrpcContext> options):base(options)
        {
            
        }
        #endregion

        #region DbSets
        public DbSet<Product> Products { get; set; }
        #endregion

    }
}
