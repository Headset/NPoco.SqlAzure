using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using NPoco.Linq;

namespace NPoco.SqlAzure
{
    public class SqlAzureDatabase : IDatabase
    {
        private Database InternalDb { get; }
        public RetryPolicy RetryPolicy { get; set; }

        public SqlAzureDatabase(string connectionString)
            : this(connectionString, DatabaseType.SqlServer2012, 3) { }

        public SqlAzureDatabase(string connectionString, DatabaseType databaseType)
            :this(connectionString, databaseType, 3) { }

        public SqlAzureDatabase(string connectionString, DatabaseType databaseType, int retryCount)
            :this(connectionString, databaseType, SqlClientFactory.Instance, retryCount) { }

        public SqlAzureDatabase(string connectionString, DatabaseType databaseType, DbProviderFactory provider, int retryCount)
        {
            InternalDb = new Database(connectionString, databaseType, provider);
            CreateRetryPolicy(retryCount);
        }
        

        public SqlAzureDatabase(DbConnection connection) : this(connection, 3) { }

        private SqlAzureDatabase(DbConnection connection, int retryCount)
        {
            InternalDb = new Database(connection);
            CreateRetryPolicy(retryCount);
        }

        private void CreateRetryPolicy(int retryCount)
        {
            RetryPolicy = new RetryPolicy(new SqlAzureTransientDetectionStrategy(), retryCount);
        }

        public DbConnection Connection => InternalDb.Connection;
        public DbTransaction Transaction => InternalDb.Transaction;
        public IDictionary<string, object> Data => InternalDb.Data;
        public int OneTimeCommandTimeout
        {
            get { return InternalDb.OneTimeCommandTimeout; }
            set { InternalDb.OneTimeCommandTimeout = value; }
        }

        public MapperCollection Mappers
        {
            get { return InternalDb.Mappers; }
            set { InternalDb.Mappers = value; }
        }
        public IPocoDataFactory PocoDataFactory
        {
            get { return InternalDb.PocoDataFactory; }
            set { InternalDb.PocoDataFactory = value; }
        }
        public DatabaseType DatabaseType => InternalDb.DatabaseType;
        public List<IInterceptor> Interceptors => InternalDb.Interceptors;
        public string ConnectionString => InternalDb.ConnectionString;

        public DbParameter CreateParameter()
        {
            return InternalDb.CreateParameter();
        }

        public void Dispose()
        {
            InternalDb.Dispose();
        }

        public IDatabase OpenSharedConnection()
        {
            return InternalDb.OpenSharedConnection();
        }

        public void CloseSharedConnection()
        {
            InternalDb.CloseSharedConnection();
        }

        public void BuildPageQueries<T>(long skip, long take, string sql, ref object[] args, out string sqlCount, out string sqlPage)
        {
            InternalDb.BuildPageQueries<T>(skip, take, sql, ref args, out sqlCount, out sqlPage);
        }

        public int Execute(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Execute(sql, args));
        }

        public int Execute(Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Execute(sql));
        }

        public int Execute(string sql, CommandType commandType, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Execute(sql, commandType, args));
        }

        public T ExecuteScalar<T>(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.ExecuteScalar<T>(sql, args));
        }

        public T ExecuteScalar<T>(Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.ExecuteScalar<T>(sql));
        }

        public T ExecuteScalar<T>(string sql, CommandType commandType, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.ExecuteScalar<T>(sql, commandType, args));
        }

        public List<object> Fetch(Type type, string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Fetch(type, sql, args));
        }

        public List<object> Fetch(Type type, Sql Sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Fetch(type, Sql));
        }

        public IEnumerable<object> Query(Type type, string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Query(type, sql, args));
        }

        public IEnumerable<object> Query(Type type, Sql Sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Query(type, Sql));
        }

        public List<T> Fetch<T>()
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Fetch<T>());
        }

        public List<T> Fetch<T>(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Fetch<T>(sql, args));
        }

        public List<T> Fetch<T>(Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Fetch<T>(sql));
        }

        public List<T> Fetch<T>(long page, long itemsPerPage, string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Fetch<T>(page, itemsPerPage, sql, args));
        }

        public List<T> Fetch<T>(long page, long itemsPerPage, Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Fetch<T>(page, itemsPerPage, sql));
        }

        public Page<T> Page<T>(long page, long itemsPerPage, string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Page<T>(page, itemsPerPage, sql, args));
        }

        public Page<T> Page<T>(long page, long itemsPerPage, Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Page<T>(page, itemsPerPage, sql));
        }

        public List<T> SkipTake<T>(long skip, long take, string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.SkipTake<T>(skip, take, sql, args));
        }

        public List<T> SkipTake<T>(long skip, long take, Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.SkipTake<T>(skip, take, sql));
        }

        public List<T> FetchOneToMany<T>(Expression<Func<T, IList>> many, string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FetchOneToMany(many, sql, args));
        }

        public List<T> FetchOneToMany<T>(Expression<Func<T, IList>> many, Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FetchOneToMany(many, sql));
        }

        public List<T> FetchOneToMany<T>(Expression<Func<T, IList>> many, Func<T, object> idFunc, string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FetchOneToMany(many, idFunc, sql, args));
        }

        public List<T> FetchOneToMany<T>(Expression<Func<T, IList>> many, Func<T, object> idFunc, Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FetchOneToMany(many, idFunc, sql));
        }

        public IEnumerable<T> Query<T>(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Query<T>(sql, args));
        }

        public IEnumerable<T> Query<T>(Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Query<T>(sql));
        }

        public IQueryProviderWithIncludes<T> Query<T>()
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Query<T>());
        }

        public T SingleById<T>(object primaryKey)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.SingleById<T>(primaryKey));
        }

        public T SingleOrDefaultById<T>(object primaryKey)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.SingleOrDefaultById<T>(primaryKey));
        }

        public T Single<T>(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Single<T>(sql, args));
        }

        public T SingleInto<T>(T instance, string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.SingleInto(instance, sql, args));
        }

        public T SingleOrDefault<T>(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.SingleOrDefault<T>(sql, args));
        }

        public T SingleOrDefaultInto<T>(T instance, string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.SingleOrDefaultInto(instance, sql, args));
        }

        public T First<T>(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.First<T>(sql, args));
        }

        public T FirstInto<T>(T instance, string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FirstInto(instance, sql, args));
        }

        public T FirstOrDefault<T>(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FirstOrDefault<T>(sql, args));
        }

        public T FirstOrDefaultInto<T>(T instance, string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FirstOrDefaultInto(instance, sql, args));
        }

        public T Single<T>(Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Single<T>(sql));
        }

        public T SingleInto<T>(T instance, Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.SingleInto(instance, sql));
        }

        public T SingleOrDefault<T>(Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.SingleOrDefault<T>(sql));
        }

        public T SingleOrDefaultInto<T>(T instance, Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.SingleOrDefaultInto(instance, sql));
        }

        public T First<T>(Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.First<T>(sql));
        }

        public T FirstInto<T>(T instance, Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FirstInto(instance, sql));
        }

        public T FirstOrDefault<T>(Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FirstOrDefault<T>(sql));
        }

        public T FirstOrDefaultInto<T>(T instance, Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FirstOrDefaultInto(instance, sql));
        }

        public Dictionary<TKey, TValue> Dictionary<TKey, TValue>(Sql Sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Dictionary<TKey, TValue>(Sql));
        }

        public Dictionary<TKey, TValue> Dictionary<TKey, TValue>(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Dictionary<TKey, TValue>(sql, args));
        }

        public bool Exists<T>(object primaryKey)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Exists<T>(primaryKey));
        }

        public TRet FetchMultiple<T1, T2, TRet>(Func<List<T1>, List<T2>, TRet> cb, string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FetchMultiple(cb, sql, args));
        }

        public TRet FetchMultiple<T1, T2, T3, TRet>(Func<List<T1>, List<T2>, List<T3>, TRet> cb, string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FetchMultiple(cb, sql, args));
        }

        public TRet FetchMultiple<T1, T2, T3, T4, TRet>(Func<List<T1>, List<T2>, List<T3>, List<T4>, TRet> cb, string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FetchMultiple(cb, sql, args));
        }

        public TRet FetchMultiple<T1, T2, TRet>(Func<List<T1>, List<T2>, TRet> cb, Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FetchMultiple(cb, sql));
        }

        public TRet FetchMultiple<T1, T2, T3, TRet>(Func<List<T1>, List<T2>, List<T3>, TRet> cb, Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FetchMultiple(cb, sql));
        }

        public TRet FetchMultiple<T1, T2, T3, T4, TRet>(Func<List<T1>, List<T2>, List<T3>, List<T4>, TRet> cb, Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FetchMultiple(cb, sql));
        }

        public Tuple<List<T1>, List<T2>> FetchMultiple<T1, T2>(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FetchMultiple<T1, T2>(sql, args));
        }

        public Tuple<List<T1>, List<T2>, List<T3>> FetchMultiple<T1, T2, T3>(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FetchMultiple<T1, T2, T3>(sql, args));
        }

        public Tuple<List<T1>, List<T2>, List<T3>, List<T4>> FetchMultiple<T1, T2, T3, T4>(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FetchMultiple<T1, T2, T3, T4>(sql, args));
        }

        public Tuple<List<T1>, List<T2>> FetchMultiple<T1, T2>(Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FetchMultiple<T1, T2>(sql));
        }

        public Tuple<List<T1>, List<T2>, List<T3>> FetchMultiple<T1, T2, T3>(Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FetchMultiple<T1, T2, T3>(sql));
        }

        public Tuple<List<T1>, List<T2>, List<T3>, List<T4>> FetchMultiple<T1, T2, T3, T4>(Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.FetchMultiple<T1, T2, T3, T4>(sql));
        }

        public Task<T> SingleByIdAsync<T>(object primaryKey)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.SingleByIdAsync<T>(primaryKey));
        }

        public Task<T> SingleOrDefaultByIdAsync<T>(object primaryKey)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.SingleOrDefaultByIdAsync<T>(primaryKey));
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object[] args)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.QueryAsync<T>(sql, args));
        }

        public Task<IEnumerable<T>> QueryAsync<T>(Sql sql)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.QueryAsync<T>(sql));
        }

        public Task<List<T>> FetchAsync<T>(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.FetchAsync<T>(sql, args));
        }

        public Task<List<T>> FetchAsync<T>(Sql sql)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.FetchAsync<T>(sql));
        }

        public Task<List<T>> FetchAsync<T>()
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.FetchAsync<T>());
        }

        public Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.PageAsync<T>(page, itemsPerPage, sql, args));
        }

        public Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, Sql sql)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.PageAsync<T>(page, itemsPerPage, sql));
        }

        public Task<List<T>> FetchAsync<T>(long page, long itemsPerPage, string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.FetchAsync<T>(page, itemsPerPage, sql, args));
        }

        public Task<List<T>> FetchAsync<T>(long page, long itemsPerPage, Sql sql)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.FetchAsync<T>(page, itemsPerPage, sql));
        }

        public Task<List<T>> SkipTakeAsync<T>(long skip, long take, string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.SkipTakeAsync<T>(skip, take, sql, args));
        }

        public Task<List<T>> SkipTakeAsync<T>(long skip, long take, Sql sql)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.SkipTakeAsync<T>(skip, take, sql));
        }

        public Task<T> ExecuteScalarAsync<T>(string sql, object[] args)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.ExecuteScalarAsync<T>(sql, args));
        }

        public Task<T> ExecuteScalarAsync<T>(Sql sql)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.ExecuteScalarAsync<T>(sql));
        }

        public Task<int> ExecuteAsync(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.ExecuteAsync(sql, args));
        }

        public Task<int> ExecuteAsync(Sql sql)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.ExecuteAsync(sql));
        }

        public void AddParameter(DbCommand cmd, object value)
        {
            InternalDb.AddParameter(cmd, value);
        }

        public DbCommand CreateCommand(DbConnection connection, CommandType commandType, string sql, params object[] args)
        {
            return InternalDb.CreateCommand(connection, commandType, sql, args);
        }

        public ITransaction GetTransaction()
        {
            return InternalDb.GetTransaction();
        }

        public ITransaction GetTransaction(IsolationLevel isolationLevel)
        {
            return InternalDb.GetTransaction(isolationLevel);
        }

        public void SetTransaction(DbTransaction tran)
        {
            InternalDb.SetTransaction(tran);
        }

        public void BeginTransaction()
        {
            InternalDb.BeginTransaction();
        }

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            InternalDb.BeginTransaction(isolationLevel);
        }

        public void AbortTransaction()
        {
            InternalDb.AbortTransaction();
        }

        public void CompleteTransaction()
        {
            InternalDb.CompleteTransaction();
        }

        public object Insert<T>(string tableName, string primaryKeyName, bool autoIncrement, T poco)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Insert(tableName, primaryKeyName, autoIncrement, poco));
        }

        public object Insert<T>(string tableName, string primaryKeyName, T poco)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Insert(tableName, primaryKeyName, poco));
        }

        public object Insert<T>(T poco)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Insert(poco));
        }

        public Task<object> InsertAsync<T>(T poco)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.InsertAsync(poco));
        }

        public Task<int> UpdateAsync(object poco)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.UpdateAsync(poco));
        }

        public Task<int> UpdateAsync(object poco, IEnumerable<string> columns)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.UpdateAsync(poco, columns));
        }

        public Task<int> DeleteAsync(object poco)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.DeleteAsync(poco));
        }

        public void InsertBulk<T>(IEnumerable<T> pocos)
        {
            RetryPolicy.ExecuteAction(() => InternalDb.InsertBulk(pocos));
        }

        public void InsertBatch<T>(IEnumerable<T> pocos, BatchOptions options = null)
        {
            RetryPolicy.ExecuteAction(() => InternalDb.InsertBatch(pocos, options));
        }

        public int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Update(tableName, primaryKeyName, poco, primaryKeyName));
        }

        public int Update(string tableName, string primaryKeyName, object poco)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Update(tableName, primaryKeyName, poco));
        }

        public int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Update(tableName, primaryKeyName, poco, primaryKeyValue, columns));
        }

        public int Update(string tableName, string primaryKeyName, object poco, IEnumerable<string> columns)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Update(tableName, primaryKeyName, poco, columns));
        }

        public int Update(object poco, IEnumerable<string> columns)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Update(poco, columns));
        }

        public int Update(object poco, object primaryKeyValue, IEnumerable<string> columns)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Update(poco, primaryKeyValue, columns));
        }

        public int Update(object poco)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Update(poco));
        }

        public int Update<T>(T poco, Expression<Func<T, object>> fields)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Update(poco, fields));
        }

        public int Update(object poco, object primaryKeyValue)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Update(poco, primaryKeyValue));
        }

        public int Update<T>(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Update(sql, args));
        }

        public int Update<T>(Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Update(sql));
        }

        public IUpdateQueryProvider<T> UpdateMany<T>()
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.UpdateMany<T>());
        }

        public int Delete(string tableName, string primaryKeyName, object poco)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Delete(tableName, primaryKeyName, poco));
        }

        public int Delete(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Delete(tableName, primaryKeyName, poco, primaryKeyValue));
        }

        public int Delete(object poco)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Delete(poco));
        }

        public int Delete<T>(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Delete<T>(sql, args));
        }

        public int Delete<T>(Sql sql)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Delete<T>(sql));
        }

        public int Delete<T>(object pocoOrPrimaryKey)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.Delete<T>(pocoOrPrimaryKey));
        }

        public IDeleteQueryProvider<T> DeleteMany<T>()
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.DeleteMany<T>());
        }

        public void Save<T>(T poco)
        {
            RetryPolicy.ExecuteAction(() => InternalDb.Save(poco));
        }

        public bool IsNew<T>(T poco)
        {
            return RetryPolicy.ExecuteAction(() => InternalDb.IsNew(poco));
        }

        public Task<int> UpdateAsync<T>(T poco, Expression<Func<T, object>> fields)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.UpdateAsync<T>(poco, fields));
        }

        public Task<T> SingleAsync<T>(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.SingleAsync<T>(sql, args));
        }

        public Task<T> SingleAsync<T>(Sql sql)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.SingleAsync<T>(sql));
        }

        public Task<T> SingleOrDefaultAsync<T>(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.SingleOrDefaultAsync<T>(sql, args));
        }

        public Task<T> SingleOrDefaultAsync<T>(Sql sql)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.SingleOrDefaultAsync<T>(sql));
        }

        public Task<T> FirstAsync<T>(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.FirstAsync<T>(sql, args));
        }

        public Task<T> FirstAsync<T>(Sql sql)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.FirstAsync<T>(sql));
        }

        public Task<T> FirstOrDefaultAsync<T>(string sql, params object[] args)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.FirstOrDefaultAsync<T>(sql, args));
        }

        public Task<T> FirstOrDefaultAsync<T>(Sql sql)
        {
            return RetryPolicy.ExecuteAsync(() => InternalDb.FirstOrDefaultAsync<T>(sql));
        }
    }
}