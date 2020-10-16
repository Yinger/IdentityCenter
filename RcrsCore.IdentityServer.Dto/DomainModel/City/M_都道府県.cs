using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RcrsCore.IdentityServer.Dto.DomainModel.City
{
    //---------------------------------------------------------------
    /// <summary>
    ///
    /// </summary>
    //---------------------------------------------------------------
    [Table("M_都道府県")]
    public class M_都道府県
    {
        /// <summary></summary>
        [Key]
        public int ID { get; set; }

        /// <summary></summary>
        [StringLength(2)]
        public string 都道府県CD { get; set; }

        /// <summary></summary>
        [StringLength(100)]
        public string 都道府県名 { get; set; }
    }
}