using Microsoft.Data.SqlClient;

namespace RepairShop.Dapper;

/// <summary>Фабрика з'єднань. Мапінг результатів на об'єкти й параметри виконує Dapper (методи розширення IDbConnection).</summary>
public static class Db
{
    public static string ConnectionString { get; set; } =
        "Server=(localdb)\\MSSQLLocalDB;Database=RepairShop;Integrated Security=true;TrustServerCertificate=true";

    public static SqlConnection Open()
    {
        var cn = new SqlConnection(ConnectionString);
        cn.Open();
        return cn;
    }
}
