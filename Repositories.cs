using Dapper;

namespace RepairShop.Dapper;

// Micro ORM: SQL пишемо самі, а Dapper прив'язує параметри (анонімні об'єкти) і перетворює рядки результату на record-и.
// Порівняно з ADO.NET зникли ручні SqlCommand/SqlDataReader та функції Map.

public class ClientDao
{
    const string Cols = "ClientID, FullName, Phone, Email, RegisteredAt";

    public int Create(string fullName, string phone, string? email)
    {
        using var cn = Db.Open();
        return cn.ExecuteScalar<int>(
            "INSERT INTO Clients (FullName, Phone, Email) OUTPUT INSERTED.ClientID VALUES (@fullName, @phone, @email)",
            new { fullName, phone, email });
    }
    public List<Client> GetAll() { using var cn = Db.Open(); return cn.Query<Client>($"SELECT {Cols} FROM Clients ORDER BY ClientID").ToList(); }
    public Client? GetById(int id) { using var cn = Db.Open(); return cn.QuerySingleOrDefault<Client>($"SELECT {Cols} FROM Clients WHERE ClientID = @id", new { id }); }
    public int Update(int id, string fullName, string phone, string? email)
    {
        using var cn = Db.Open();
        return cn.Execute("UPDATE Clients SET FullName=@fullName, Phone=@phone, Email=@email WHERE ClientID=@id", new { id, fullName, phone, email });
    }
    public int Delete(int id) { using var cn = Db.Open(); return cn.Execute("DELETE FROM Clients WHERE ClientID=@id", new { id }); }
}

public class MasterDao
{
    const string Cols = "MasterID, FullName, Specialization, Phone, HireDate, CommissionPct, IsActive";

    public int Create(string fullName, string spec, string phone, DateTime hired, decimal commission)
    {
        using var cn = Db.Open();
        return cn.ExecuteScalar<int>(
            "INSERT INTO Masters (FullName, Specialization, Phone, HireDate, CommissionPct) OUTPUT INSERTED.MasterID VALUES (@fullName,@spec,@phone,@hired,@commission)",
            new { fullName, spec, phone, hired, commission });
    }
    public List<Master> GetAll() { using var cn = Db.Open(); return cn.Query<Master>($"SELECT {Cols} FROM Masters ORDER BY MasterID").ToList(); }
    public Master? GetById(int id) { using var cn = Db.Open(); return cn.QuerySingleOrDefault<Master>($"SELECT {Cols} FROM Masters WHERE MasterID=@id", new { id }); }
    public int Update(int id, decimal commission, bool isActive)
    {
        using var cn = Db.Open();
        return cn.Execute("UPDATE Masters SET CommissionPct=@commission, IsActive=@isActive WHERE MasterID=@id", new { id, commission, isActive });
    }
    public int Delete(int id) { using var cn = Db.Open(); return cn.Execute("DELETE FROM Masters WHERE MasterID=@id", new { id }); }
}

public class DeviceDao
{
    const string Cols = "DeviceID, ClientID, DeviceType, Brand, Model, SerialNumber";

    public int Create(int clientId, string type, string brand, string model, string serial)
    {
        using var cn = Db.Open();
        return cn.ExecuteScalar<int>(
            "INSERT INTO Devices (ClientID, DeviceType, Brand, Model, SerialNumber) OUTPUT INSERTED.DeviceID VALUES (@clientId,@type,@brand,@model,@serial)",
            new { clientId, type, brand, model, serial });
    }
    public List<Device> GetAll() { using var cn = Db.Open(); return cn.Query<Device>($"SELECT {Cols} FROM Devices ORDER BY DeviceID").ToList(); }
    public Device? GetById(int id) { using var cn = Db.Open(); return cn.QuerySingleOrDefault<Device>($"SELECT {Cols} FROM Devices WHERE DeviceID=@id", new { id }); }
    public int Update(int id, string model) { using var cn = Db.Open(); return cn.Execute("UPDATE Devices SET Model=@model WHERE DeviceID=@id", new { id, model }); }
    public int Delete(int id) { using var cn = Db.Open(); return cn.Execute("DELETE FROM Devices WHERE DeviceID=@id", new { id }); }
}

public class RepairOrderDao
{
    const string Cols = "OrderID, DeviceID, MasterID, ReceivedAt, CompletedAt, IssueDescription, Status, WarrantyDays";

    public int Create(int deviceId, int? masterId, string issue)
    {
        using var cn = Db.Open();
        return cn.ExecuteScalar<int>(
            "INSERT INTO RepairOrders (DeviceID, MasterID, IssueDescription) OUTPUT INSERTED.OrderID VALUES (@deviceId,@masterId,@issue)",
            new { deviceId, masterId, issue });
    }
    public List<RepairOrder> GetAll() { using var cn = Db.Open(); return cn.Query<RepairOrder>($"SELECT {Cols} FROM RepairOrders ORDER BY OrderID").ToList(); }
    public RepairOrder? GetById(int id) { using var cn = Db.Open(); return cn.QuerySingleOrDefault<RepairOrder>($"SELECT {Cols} FROM RepairOrders WHERE OrderID=@id", new { id }); }
    public int UpdateStatus(int id, string status, DateTime? completedAt)
    {
        using var cn = Db.Open();
        return cn.Execute("UPDATE RepairOrders SET Status=@status, CompletedAt=@completedAt WHERE OrderID=@id", new { id, status, completedAt });
    }
    public int Delete(int id) { using var cn = Db.Open(); return cn.Execute("DELETE FROM RepairOrders WHERE OrderID=@id", new { id }); }
}

public class ServiceDao
{
    const string Cols = "ServiceID, Name, BasePrice, DurationMinutes";

    public int Create(string name, decimal price, int minutes)
    {
        using var cn = Db.Open();
        return cn.ExecuteScalar<int>(
            "INSERT INTO Services (Name, BasePrice, DurationMinutes) OUTPUT INSERTED.ServiceID VALUES (@name,@price,@minutes)", new { name, price, minutes });
    }
    public List<Service> GetAll() { using var cn = Db.Open(); return cn.Query<Service>($"SELECT {Cols} FROM Services ORDER BY ServiceID").ToList(); }
    public Service? GetById(int id) { using var cn = Db.Open(); return cn.QuerySingleOrDefault<Service>($"SELECT {Cols} FROM Services WHERE ServiceID=@id", new { id }); }
    public int Update(int id, decimal price) { using var cn = Db.Open(); return cn.Execute("UPDATE Services SET BasePrice=@price WHERE ServiceID=@id", new { id, price }); }
    public int Delete(int id) { using var cn = Db.Open(); return cn.Execute("DELETE FROM Services WHERE ServiceID=@id", new { id }); }
}

public class PartDao
{
    const string Cols = "PartID, Name, Article, Price, StockQty";

    public int Create(string name, string article, decimal price, int qty)
    {
        using var cn = Db.Open();
        return cn.ExecuteScalar<int>(
            "INSERT INTO Parts (Name, Article, Price, StockQty) OUTPUT INSERTED.PartID VALUES (@name,@article,@price,@qty)", new { name, article, price, qty });
    }
    public List<Part> GetAll() { using var cn = Db.Open(); return cn.Query<Part>($"SELECT {Cols} FROM Parts ORDER BY PartID").ToList(); }
    public Part? GetById(int id) { using var cn = Db.Open(); return cn.QuerySingleOrDefault<Part>($"SELECT {Cols} FROM Parts WHERE PartID=@id", new { id }); }
    public int UpdateStock(int id, int qty) { using var cn = Db.Open(); return cn.Execute("UPDATE Parts SET StockQty=@qty WHERE PartID=@id", new { id, qty }); }
    public int Delete(int id) { using var cn = Db.Open(); return cn.Execute("DELETE FROM Parts WHERE PartID=@id", new { id }); }
}
