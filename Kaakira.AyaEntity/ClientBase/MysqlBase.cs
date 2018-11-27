using System;
using System.Collections.Generic;
using Dapper;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;
using System.Data;
using System.Data.Common;
using KakiEntity.Base;
using MySql.Data.MySqlClient;
using KiraEntity.Base;
using KiraEntity;
using KiraEntity.Tools;

namespace KakiEntity.Base
{
    public class MySqlBase : SqlClientBase
    {
        public MySqlBase(string connstr) : base(connstr)
        {
            Connection = new MySqlConnection(this.ConnectionString);

        }
        /// <summary>
        /// 构建分页查询语句
        /// </summary>
        /// <param name="pag"></param>
        /// <param name="tableName"></param>
        /// <param name="caluse"></param>
        /// <returns></returns>
        private StringBuilder BuildePageQuerySql(Pagination pag, string tableName, string caluse, string columns = null)
        {
            IEnumerable<string> fields = pag.DyParameters.ParameterNames;
            if (string.IsNullOrEmpty(columns))
            {
                columns = "*";
            }
            StringBuilder sqlmem = new StringBuilder("select " + columns + " from " + tableName);

            if (!string.IsNullOrEmpty(caluse))
            {
                sqlmem.Append(" where ").Append(caluse);
            }
            sqlmem.Append(" Order by ").Append(pag.OrderField).Append(" ").Append(pag.OrderType);
            sqlmem.Append(" limit @StartRow,@PageSize;");
            return sqlmem;
        }
        /// 构建分页查询数据总条数语句
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="caluse"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private StringBuilder BuildPageQueryTotal(string tableName, string caluse)
        {
            StringBuilder sqlmem = new StringBuilder("SELECT count(*) from " + tableName);
            if (string.IsNullOrEmpty(caluse))
            {
                sqlmem.Append(" where ").Append(caluse);
            }
            return sqlmem;
        }


        public override IEnumerable<T> GetCustomPageList<T>(Pagination pag, string tableName, string caluse, string columns = null)
        {
            IEnumerable<string> fields = pag.DyParameters.ParameterNames;
            if (string.IsNullOrEmpty(caluse))
            {
                caluse = fields.Join(" and ", m => m + "=@" + m);
            }
            StringBuilder sqlmem = BuildePageQuerySql(pag, tableName, caluse);
            pag.TotalCount = this.Connection.ExecuteScalar<int>(BuildPageQueryTotal(tableName, caluse).ToString(), pag.PageDyParameters);
            return this.Connection.Query<T>(sqlmem.ToString(), pag.PageDyParameters);
        }


        public override IEnumerable<T> GetPageList<T>(Pagination pag, string caluse = null)
        {
            IEnumerable<string> fields = pag.DyParameters.ParameterNames;
            if (string.IsNullOrEmpty(caluse))
            {
                caluse = fields.Join(" and ", m => m + "=@" + m);
            }
            string tableName = this.GetTableName(typeof(T));
            StringBuilder sqlmem = BuildePageQuerySql(pag, tableName, caluse);
            pag.TotalCount = this.Connection.ExecuteScalar<int>(BuildPageQueryTotal(tableName, caluse).ToString(), pag.PageDyParameters);
            return this.Connection.Query<T>(sqlmem.ToString(), pag.PageDyParameters);
        }

    public override int AutoIdByAdd<T>(T data)
    {
      throw new NotImplementedException();
    }
  }
}
