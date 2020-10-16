//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace RcrsCore.IdentityServer.Models.DomainEntity
//{
//    //---------------------------------------------------------------
//    /// <summary>
//    ///
//    /// </summary>
//    //---------------------------------------------------------------
//    [Table("M_市区町村")]
//    public class M_市区町村
//    {
//        /// <summary></summary>
//        [Key]
//        public int ID { get; set; }

//        /// <summary></summary>
//        [StringLength(6)]
//        public string 市区町村CD { get; set; }

//        /// <summary></summary>
//        [StringLength(2)]
//        public string 都道府県CD { get; set; }

//        /// <summary></summary>
//        [StringLength(100)]
//        public string 都道府県名 { get; set; }

//        /// <summary></summary>
//        [StringLength(100)]
//        public string 市区町村名 { get; set; }

//        /// <summary></summary>
//        [StringLength(50)]
//        public string 郵便番号 { get; set; }

//        /// <summary></summary>
//        public string 住所 { get; set; }

//        /// <summary></summary>
//        public string メモ { get; set; }
//    }
//}