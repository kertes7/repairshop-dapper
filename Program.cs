using RepairShop.Dapper;

// Micro ORM (Dapper). Демонстраційний прогін: CRUD для 6 таблиць (створення → читання → оновлення → видалення) і 3 запити.
// Створені записи видаляються в кінці, тому дані бази не змінюються.
var clients = new ClientDao(); var masters = new MasterDao(); var devices = new DeviceDao();
var orders = new RepairOrderDao(); var services = new ServiceDao(); var parts = new PartDao();

void H(string t) { Console.WriteLine(); Console.WriteLine($"=== {t} ==="); }

H("Підключення");
Console.WriteLine($"Клієнтів у базі: {clients.GetAll().Count}, майстрів: {masters.GetAll().Count}, замовлень: {orders.GetAll().Count}");

H("CRUD: Clients");
int clientId = clients.Create("Тестовий Клієнт Петрович", "+380991112233", "test.client@example.com");
Console.WriteLine($"CREATE  -> ClientID = {clientId}");
Console.WriteLine($"READ    -> {clients.GetById(clientId)}");
Console.WriteLine($"UPDATE  -> змінено рядків: {clients.Update(clientId, "Тестовий Клієнт Іванович", "+380991112233", null)}");
Console.WriteLine($"READ    -> {clients.GetById(clientId)}");

H("CRUD: Masters");
int masterId = masters.Create("Тестовий Майстер Олегович", "ноутбуки", "+380671119999", new DateTime(2026, 11, 1), 18.5m);
Console.WriteLine($"CREATE  -> MasterID = {masterId}");
Console.WriteLine($"READ    -> {masters.GetById(masterId)}");
Console.WriteLine($"UPDATE  -> змінено рядків: {masters.Update(masterId, 25m, false)}");
Console.WriteLine($"READ    -> {masters.GetById(masterId)}");

H("CRUD: Devices");
int deviceId = devices.Create(clientId, "планшет", "Lenovo", "Tab M10", "ADO-TEST-0001");
Console.WriteLine($"CREATE  -> DeviceID = {deviceId}");
Console.WriteLine($"READ    -> {devices.GetById(deviceId)}");
Console.WriteLine($"UPDATE  -> змінено рядків: {devices.Update(deviceId, "Tab M10 Plus")}");
Console.WriteLine($"READ    -> {devices.GetById(deviceId)}");

H("CRUD: RepairOrders");
int orderId = orders.Create(deviceId, masterId, "Не вмикається планшет");
Console.WriteLine($"CREATE  -> OrderID = {orderId}");
Console.WriteLine($"READ    -> {orders.GetById(orderId)}");
Console.WriteLine($"UPDATE  -> змінено рядків: {orders.UpdateStatus(orderId, "готово", DateTime.Now)}");
Console.WriteLine($"READ    -> {orders.GetById(orderId)}");

H("CRUD: Services");
int serviceId = services.Create("Тестова послуга", 250m, 20);
Console.WriteLine($"CREATE  -> ServiceID = {serviceId}");
Console.WriteLine($"READ    -> {services.GetById(serviceId)}");
Console.WriteLine($"UPDATE  -> змінено рядків: {services.Update(serviceId, 275m)}");
Console.WriteLine($"READ    -> {services.GetById(serviceId)}");

H("CRUD: Parts");
int partId = parts.Create("Тестова запчастина", "ADO-TEST-PART", 99.90m, 5);
Console.WriteLine($"CREATE  -> PartID = {partId}");
Console.WriteLine($"READ    -> {parts.GetById(partId)}");
Console.WriteLine($"UPDATE  -> змінено рядків: {parts.UpdateStock(partId, 4)}");
Console.WriteLine($"READ    -> {parts.GetById(partId)}");

H("Запит 1 (з об'єднанням таблиць): замовлення з клієнтом, пристроєм і майстром");
foreach (var o in Reports.OrdersWithDetails().Take(8))
    Console.WriteLine($"#{o.OrderID,-3} {o.Client,-28} {o.Device,-22} {o.Master ?? "—",-30} {o.Status}");
Console.WriteLine("...");

H("Запит 2 (з фільтрацією): замовлення в статусі «в роботі» з 1 листопада 2025");
foreach (var o in Reports.OrdersByStatus("в роботі", new DateTime(2025, 11, 1)))
    Console.WriteLine($"#{o.OrderID,-3} {o.ReceivedAt:yyyy-MM-dd HH:mm}  {o.Issue}");

H("Запит 3 (з агрегатними функціями): кількість замовлень і виручка по майстрах");
foreach (var s in Reports.RevenueByMaster())
    Console.WriteLine($"{s.Master,-32} замовлень: {s.Orders,-2} виручка: {s.Revenue,10:N2} грн");

H("CRUD: Delete (прибирання тестових даних у зворотному порядку залежностей)");
Console.WriteLine($"DELETE RepairOrders -> {orders.Delete(orderId)}; Devices -> {devices.Delete(deviceId)}; Clients -> {clients.Delete(clientId)}");
Console.WriteLine($"DELETE Masters -> {masters.Delete(masterId)}; Services -> {services.Delete(serviceId)}; Parts -> {parts.Delete(partId)}");
Console.WriteLine($"READ після видалення клієнта: {(clients.GetById(clientId) is null ? "запис не знайдено (видалено)" : "ще існує")}");
Console.WriteLine($"Клієнтів у базі: {clients.GetAll().Count}, майстрів: {masters.GetAll().Count}, замовлень: {orders.GetAll().Count}");
