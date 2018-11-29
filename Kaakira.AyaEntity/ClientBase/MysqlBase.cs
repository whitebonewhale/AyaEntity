using AyaEntity.SQLTools;
using AyaEntity.Tools;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AyaEntity.Base
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


        public override PagingResult<T> GetCustomPageList<T>(Pagination pag, string tableName, string caluse, string columns = null)
        {
            IEnumerable<string> fields = pag.DyParameters.ParameterNames;
            if (string.IsNullOrEmpty(caluse))
            {
                caluse = fields.Join(" and ", m => m + "=@" + m);
            }
            return new PagingResult<T>
            {
                Total = this.Connection.ExecuteScalar<int>(BuildPageQueryTotal(tableName, caluse).ToString(), pag.PageDyParameters),
                Rows = this.Connection.Query<T>(BuildePageQuerySql(pag, tableName, caluse).ToString(), pag.PageDyParameters)
            };
        }


        public override PagingResult<T> GetPageList<T>(Pagination pag, string caluse = null)
        {
            IEnumerable<string> fields = pag.DyParameters.ParameterNames;
            if (string.IsNullOrEmpty(caluse))
            {
                caluse = fields.Join(" and ", m => m + "=@" + m);
            }
            string tableName = this.GetTableName(typeof(T));
            return new PagingResult<T>
            {
                Total = this.Connection.ExecuteScalar<int>(BuildPageQueryTotal(tableName, caluse).ToString(), pag.PageDyParameters),
                Rows = this.Connection.Query<T>(BuildePageQuerySql(pag, tableName, caluse).ToString(), pag.PageDyParameters)
            };
        }

        public override int AutoIdByAdd<T>(T dyparam)
        {
            if (dyparam == null)
                throw new ArgumentNullException("dyparam");
            string tableName = GetTableName(typeof(T));
            return Connection.ExecuteScalar<int>(state.ToInsert(tableName, dyparam?.GetType()) + ";select @@IDENTITY;", dyparam);
        }
    }
}
