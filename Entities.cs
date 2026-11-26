namespace RepairShop.Dapper;

public record Client(int ClientID, string FullName, string Phone, string? Email, DateTime RegisteredAt);
public record Master(int MasterID, string FullName, string Specialization, string Phone, DateTime HireDate, decimal CommissionPct, bool IsActive);
public record Device(int DeviceID, int ClientID, string DeviceType, string Brand, string Model, string SerialNumber);
public record RepairOrder(int OrderID, int DeviceID, int? MasterID, DateTime ReceivedAt, DateTime? CompletedAt, string IssueDescription, string Status, int WarrantyDays);
public record Service(int ServiceID, string Name, decimal BasePrice, int DurationMinutes);
public record Part(int PartID, string Name, string Article, decimal Price, int StockQty);
