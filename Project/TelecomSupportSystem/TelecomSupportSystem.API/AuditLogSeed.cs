using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.API;

public static class AuditLogSeed
{
    public static void Seed(ApplicationDbContext db)
    {
        // Seed AuditLog zapisi
        if (db.AuditLogs.Any())
            return;

        var adminId = db.Users.First(u => u.Email == "admin@test.com").UserId;
        var agentId = db.Users.First(u => u.Email == "agent@test.com")?.UserId;
        var aminaId = db.Users.First(u => u.Email == "amina.hodzic@telecom.ba")?.UserId;
        var kenanId = db.Users.First(u => u.Email == "kenan.imamovic@telecom.ba")?.UserId;
        var clientId = db.Users.First(u => u.Email == "client@test.com").UserId;

    var auditLogs = new List<AuditLog>
    {
        // USER_LOGIN
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-7), UserId = adminId, ActionType = AuditActionType.USER_LOGIN.ToString(), EntityType = "User", EntityId = adminId.ToString(), Description = "Korisnik admin se prijavio", IpAddress = "192.168.1.100" },
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-6.5), UserId = agentId, ActionType = AuditActionType.USER_LOGIN.ToString(), EntityType = "User", EntityId = agentId.ToString(), Description = "Korisnik agent se prijavio", IpAddress = "192.168.1.101" },
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-6), UserId = aminaId, ActionType = AuditActionType.USER_LOGIN.ToString(), EntityType = "User", EntityId = aminaId.ToString(), Description = "Korisnik Amina Hodžić se prijavio", IpAddress = "192.168.1.102" },
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-5), UserId = kenanId, ActionType = AuditActionType.USER_LOGIN.ToString(), EntityType = "User", EntityId = kenanId.ToString(), Description = "Korisnik Kenan Imamović se prijavio", IpAddress = "192.168.1.103" },

        // USER_LOGOUT
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-7).AddHours(2), UserId = adminId, ActionType = AuditActionType.USER_LOGOUT.ToString(), EntityType = "User", EntityId = adminId.ToString(), Description = "Korisnik admin se odjavio", IpAddress = "192.168.1.100" },
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-6.5).AddHours(2), UserId = agentId, ActionType = AuditActionType.USER_LOGOUT.ToString(), EntityType = "User", EntityId = agentId.ToString(), Description = "Korisnik agent se odjavio", IpAddress = "192.168.1.101" },

        // USER_LOGIN_FAILED
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-5), UserId = null, ActionType = AuditActionType.USER_LOGIN_FAILED.ToString(), EntityType = "User", EntityId = null, Description = "Neuspješan pokušaj prijave za: nepostojeći@example.com", IpAddress = "192.168.1.104" },
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-4), UserId = null, ActionType = AuditActionType.USER_LOGIN_FAILED.ToString(), EntityType = "User", EntityId = null, Description = "Neuspješan pokušaj prijave za: admin@test.com", IpAddress = "192.168.1.105" },

        // USER_CREATED
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-4), UserId = adminId, ActionType = AuditActionType.USER_CREATED.ToString(), EntityType = "User", EntityId = clientId.ToString(), Description = "Novi korisnik kreiran", NewValue = "{\"firstName\":\"Client\",\"lastName\":\"User\",\"email\":\"client@test.com\",\"role\":\"CLIENT\"}" },
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-3), UserId = adminId, ActionType = AuditActionType.USER_CREATED.ToString(), EntityType = "User", EntityId = aminaId.ToString(), Description = "Novi korisnik kreiran", NewValue = "{\"firstName\":\"Amina\",\"lastName\":\"Hodžić\",\"email\":\"amina.hodzic@telecom.ba\",\"role\":\"AGENT\"}" },

        // USER_UPDATED
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-3), UserId = adminId, ActionType = AuditActionType.USER_UPDATED.ToString(), EntityType = "User", EntityId = clientId.ToString(), Description = "Korisnik ažuriran", OldValue = "{\"phone\":\"\",\"location\":\"SARAJEVO\"}", NewValue = "{\"phone\":\"+387611234567\",\"location\":\"MOSTAR\"}" },

        // USER_DEACTIVATED
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-2), UserId = adminId, ActionType = AuditActionType.USER_DEACTIVATED.ToString(), EntityType = "User", EntityId = clientId.ToString(), Description = "Korisnik client@test.com deaktiviran od strane admin", IpAddress = "192.168.1.100" },

        // USER_REACTIVATED
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-1), UserId = adminId, ActionType = AuditActionType.USER_REACTIVATED.ToString(), EntityType = "User", EntityId = clientId.ToString(), Description = "Korisnik client@test.com reaktiviran od strane admin", IpAddress = "192.168.1.100" },

        // TICKET_CREATED
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-5), UserId = clientId, ActionType = AuditActionType.TICKET_CREATED.ToString(), EntityType = "Ticket", EntityId = "1", Description = "Tiket kreiran: Internet ne radi", NewValue = "{\"title\":\"Internet ne radi\",\"priority\":\"HIGH\",\"category\":\"INTERNET\"}" },
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-4), UserId = clientId, ActionType = AuditActionType.TICKET_CREATED.ToString(), EntityType = "Ticket", EntityId = "2", Description = "Tiket kreiran: Pogrešan iznos na računu", NewValue = "{\"title\":\"Pogrešan iznos na računu\",\"priority\":\"HIGH\",\"category\":\"BILLING\"}" },
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-3), UserId = clientId, ActionType = AuditActionType.TICKET_CREATED.ToString(), EntityType = "Ticket", EntityId = "3", Description = "Tiket kreiran: TV signal isčezava", NewValue = "{\"title\":\"TV signal isčezava\",\"priority\":\"LOW\",\"category\":\"TV\"}" },

        // TICKET_STATUS_CHANGED
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-4.5), UserId = aminaId, ActionType = AuditActionType.TICKET_STATUS_CHANGED.ToString(), EntityType = "Ticket", EntityId = "1", Description = "Tiket #1: status promijenjen sa OPEN na IN_PROGRESS", OldValue = "{\"status\":\"OPEN\"}", NewValue = "{\"status\":\"IN_PROGRESS\"}" },
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-2), UserId = aminaId, ActionType = AuditActionType.TICKET_STATUS_CHANGED.ToString(), EntityType = "Ticket", EntityId = "1", Description = "Tiket #1: status promijenjen sa IN_PROGRESS na CLOSED", OldValue = "{\"status\":\"IN_PROGRESS\"}", NewValue = "{\"status\":\"CLOSED\"}" },

        // TICKET_PRIORITY_CHANGED
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-4), UserId = aminaId, ActionType = AuditActionType.TICKET_PRIORITY_CHANGED.ToString(), EntityType = "Ticket", EntityId = "2", Description = "Tiket #2: prioritet promijenjen sa HIGH na MEDIUM", OldValue = "{\"priority\":\"HIGH\"}", NewValue = "{\"priority\":\"MEDIUM\"}" },

        // TICKET_FORWARDED
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-3.5), UserId = adminId, ActionType = AuditActionType.TICKET_FORWARDED.ToString(), EntityType = "Ticket", EntityId = "2", Description = "Tiket #2 proslijeđen od Amine ka Kenanu", IpAddress = "192.168.1.100" },
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-3), UserId = adminId, ActionType = AuditActionType.TICKET_FORWARDED.ToString(), EntityType = "Ticket", EntityId = "3", Description = "Tiket #3 proslijeđen od Amine ka Dinu", IpAddress = "192.168.1.100" },

        // TICKET_CLOSURE_REQUESTED
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-1.5), UserId = clientId, ActionType = AuditActionType.TICKET_CLOSURE_REQUESTED.ToString(), EntityType = "Ticket", EntityId = "1", Description = "Korisnik je zahtjevao zatvaranje tiketa #1", IpAddress = "192.168.1.106" },

        // TICKET_CLOSED
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-1), UserId = aminaId, ActionType = AuditActionType.TICKET_CLOSED.ToString(), EntityType = "Ticket", EntityId = "1", Description = "Tiket #1 zatvoren od strane agenta", NewValue = "{\"closedDate\":\"2025-05-23T10:30:00Z\",\"resolution\":\"Problem riješen\"}" },

        // PACKAGE_CREATED
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-6), UserId = adminId, ActionType = AuditActionType.PACKAGE_CREATED.ToString(), EntityType = "CatalogPackage", EntityId = "1", Description = "Novi paket kreiran", NewValue = "{\"name\":\"Internet Start 100 Mbps\",\"type\":\"INTERNET\",\"price\":29.90}" },
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-5.5), UserId = adminId, ActionType = AuditActionType.PACKAGE_CREATED.ToString(), EntityType = "CatalogPackage", EntityId = "2", Description = "Novi paket kreiran", NewValue = "{\"name\":\"TV Premium\",\"type\":\"TV\",\"price\":24.90}" },

        // PACKAGE_UPDATED
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-4), UserId = adminId, ActionType = AuditActionType.PACKAGE_UPDATED.ToString(), EntityType = "CatalogPackage", EntityId = "1", Description = "Paket ažuriran", OldValue = "{\"price\":29.90}", NewValue = "{\"price\":31.90}" },

        // PACKAGE_DEACTIVATED
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-2), UserId = adminId, ActionType = AuditActionType.PACKAGE_DEACTIVATED.ToString(), EntityType = "CatalogPackage", EntityId = "9", Description = "Paket 'Internet Legacy ADSL' deaktiviran", IpAddress = "192.168.1.100" },

        // SUBSCRIPTION_ASSIGNED
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-3), UserId = adminId, ActionType = AuditActionType.SUBSCRIPTION_ASSIGNED.ToString(), EntityType = "ClientSubscription", EntityId = "1", Description = "Paket 'Internet Start 100 Mbps' dodijeljen klijentu client@test.com", IpAddress = "192.168.1.100" },
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-2.5), UserId = adminId, ActionType = AuditActionType.SUBSCRIPTION_ASSIGNED.ToString(), EntityType = "ClientSubscription", EntityId = "2", Description = "Paket 'TV Premium' dodijeljen klijentu client@test.com", IpAddress = "192.168.1.100" },

        // SUBSCRIPTION_DEACTIVATED
        new AuditLog { Timestamp = DateTime.UtcNow.AddDays(-1), UserId = adminId, ActionType = AuditActionType.SUBSCRIPTION_DEACTIVATED.ToString(), EntityType = "ClientSubscription", EntityId = "1", Description = "Pretplata na paket 'Internet Start 100 Mbps' deaktivirana za client@test.com", IpAddress = "192.168.1.100" },

        // Dodatni zapisi za testiranje
        new AuditLog { Timestamp = DateTime.UtcNow.AddHours(-12), UserId = aminaId, ActionType = AuditActionType.TICKET_STATUS_CHANGED.ToString(), EntityType = "Ticket", EntityId = "5", Description = "Tiket #5: status promijenjen sa OPEN na IN_PROGRESS", OldValue = "{\"status\":\"OPEN\"}", NewValue = "{\"status\":\"IN_PROGRESS\"}" },
        new AuditLog { Timestamp = DateTime.UtcNow.AddHours(-10), UserId = kenanId, ActionType = AuditActionType.TICKET_STATUS_CHANGED.ToString(), EntityType = "Ticket", EntityId = "4", Description = "Tiket #4: status promijenjen sa OPEN na IN_PROGRESS", OldValue = "{\"status\":\"OPEN\"}", NewValue = "{\"status\":\"IN_PROGRESS\"}" },
        new AuditLog { Timestamp = DateTime.UtcNow.AddHours(-8), UserId = adminId, ActionType = AuditActionType.USER_LOGIN.ToString(), EntityType = "User", EntityId = adminId.ToString(), Description = "Korisnik admin se prijavio", IpAddress = "192.168.1.107" },
        new AuditLog { Timestamp = DateTime.UtcNow.AddHours(-6), UserId = aminaId, ActionType = AuditActionType.TICKET_CREATED.ToString(), EntityType = "Ticket", EntityId = "15", Description = "Interni tiket kreiran", NewValue = "{\"title\":\"Internal support request\",\"type\":\"INTERNAL\"}" },
        new AuditLog { Timestamp = DateTime.UtcNow.AddHours(-4), UserId = adminId, ActionType = AuditActionType.USER_LOGIN_FAILED.ToString(), EntityType = "User", EntityId = null, Description = "Neuspješan pokušaj prijave za: test@example.com", IpAddress = "192.168.1.108" },
        new AuditLog { Timestamp = DateTime.UtcNow.AddHours(-2), UserId = kenanId, ActionType = AuditActionType.TICKET_PRIORITY_CHANGED.ToString(), EntityType = "Ticket", EntityId = "4", Description = "Tiket #4: prioritet promijenjen sa MEDIUM na HIGH", OldValue = "{\"priority\":\"MEDIUM\"}", NewValue = "{\"priority\":\"HIGH\"}" },
    };

        db.AuditLogs.AddRange(auditLogs);
        db.SaveChanges();
    }
}
