using Dapper;

namespace RepairShop.Dapper;

/// <summary>Запити з об'єднанням, фільтрацією та агрегатами. Dapper мапить стовпці на параметри record-ів за іменами.</summary>
public static class Reports
{
    public record OrderRow(int OrderID, string Client, string Device, string? Master, string Status);
    public record MasterStat(string Master, int Orders, decimal Revenue);
    public record FilteredOrder(int OrderID, string Issue, DateTime ReceivedAt);

    /// <summary>З об'єднанням таблиць: замовлення з клієнтом, пристроєм і майстром.</summary>
    public static List<OrderRow> OrdersWithDetails()
    {
        using var cn = Db.Open();
        return cn.Query<OrderRow>(@"
            SELECT o.OrderID, c.FullName AS Client, d.Brand + N' ' + d.Model AS Device, m.FullName AS Master, o.Status
            FROM RepairOrders AS o
            JOIN Devices AS d ON d.DeviceID = o.DeviceID
            JOIN Clients AS c ON c.ClientID = d.ClientID
            LEFT JOIN Masters AS m ON m.MasterID = o.MasterID
            ORDER BY o.OrderID").ToList();
    }

    /// <summary>З фільтрацією: замовлення в заданому статусі, прийняті після заданої дати.</summary>
    public static List<(int OrderID, string Issue, DateTime ReceivedAt)> OrdersByStatus(string status, DateTime since)
    {
        using var cn = Db.Open();
        return cn.Query<FilteredOrder>(
                "SELECT OrderID, IssueDescription AS Issue, ReceivedAt FROM RepairOrders WHERE Status = @status AND ReceivedAt >= @since ORDER BY ReceivedAt",
                new { status, since })
            .Select(o => (o.OrderID, o.Issue, o.ReceivedAt)).ToList();
    }

    /// <summary>З агрегатними функціями: кількість замовлень і виручка по майстрах (за оплатами).</summary>
    public static List<MasterStat> RevenueByMaster()
    {
        using var cn = Db.Open();
        return cn.Query<MasterStat>(@"
            SELECT m.FullName AS Master, COUNT(DISTINCT o.OrderID) AS Orders, ISNULL(SUM(p.Amount), 0) AS Revenue
            FROM Masters AS m
            JOIN RepairOrders AS o ON o.MasterID = m.MasterID
            LEFT JOIN Payments AS p ON p.OrderID = o.OrderID
            GROUP BY m.FullName
            HAVING COUNT(DISTINCT o.OrderID) > 0
            ORDER BY Revenue DESC").ToList();
    }
}
