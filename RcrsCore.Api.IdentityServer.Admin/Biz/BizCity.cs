using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using RcrsCore.IdentityServer.Dto.DomainModel.City;
using RcrsCore.Api.IdentityServer.Admin.Models.DbFirst.City;

namespace RcrsCore.Api.IdentityServer.Admin.Biz
{
    //---------------------------------------------------------------
    /// <summary>
    /// 市区町村ビズネスクラス
    /// </summary>
    //---------------------------------------------------------------
    public class BizCity
    {
        /// <summary></summary>
        private readonly CityContext _dbCity;

        //---------------------------------------------------------------
        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="cityDb"></param>
        //---------------------------------------------------------------
        public BizCity(CityContext cityDb)
        {
            _dbCity = cityDb;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 市区町村リストを取得
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        public List<SelectListItem> GetSelectList()
        {
            List<SelectListItem> cityList = (from city in _dbCity.M_市区町村s
                                             select new SelectListItem
                                             {
                                                 Value = city.市区町村cd,
                                                 Text = city.都道府県名 + city.市区町村名
                                             }).ToList();
            //空行をinsertします。
            cityList.Insert(0, new SelectListItem() { Value = "", Text = "" });

            return cityList;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 市区町村の全件を取得します。
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        public List<M_市区町村> GetAll()
        {
            return _dbCity.M_市区町村s.ToList();
        }
    }
}