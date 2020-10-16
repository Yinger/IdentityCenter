using Microsoft.EntityFrameworkCore;
using RcrsCore.IdentityServer.Models.DomainEntity;

namespace RcrsCore.IdentityServer.Models
{
    //---------------------------------------------------------------
    /// <summary>
    /// 都道府県 市区町村
    /// </summary>
    //---------------------------------------------------------------
    public class CityDbContext : DbContext
    {
        /// <summary>都道府県</summary>
        public DbSet<M_都道府県> M_都道府県s { get; set; }

        /// <summary>市区町村</summary>
        public DbSet<M_市区町村> M_市区町村s { get; set; }

        //---------------------------------------------------------------
        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="options"></param>
        //---------------------------------------------------------------
        public CityDbContext(DbContextOptions<CityDbContext> options)
            : base(options)
        {
        }
    }
}