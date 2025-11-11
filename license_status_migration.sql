CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `Applications` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `DisplayName` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `Description` varchar(1000) CHARACTER SET utf8mb4 NULL,
    `CurrentVersion` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `ApiKey` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
    `IsActive` tinyint(1) NOT NULL DEFAULT TRUE,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NULL,
    CONSTRAINT `PK_Applications` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Users` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Username` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `Email` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `PasswordHash` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `IsAdmin` tinyint(1) NOT NULL DEFAULT FALSE,
    `IsActive` tinyint(1) NOT NULL DEFAULT TRUE,
    `LastLoginAt` datetime(6) NULL,
    `FailedLoginAttempts` int NOT NULL DEFAULT 0,
    `LockedOutUntil` datetime(6) NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_Users` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `ApplicationVersions` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ApplicationId` int NOT NULL,
    `Version` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `ReleaseNotes` text CHARACTER SET utf8mb4 NULL,
    `DownloadUrl` varchar(1000) CHARACTER SET utf8mb4 NULL,
    `IsRequired` tinyint(1) NOT NULL DEFAULT FALSE,
    `ReleasedAt` datetime(6) NOT NULL,
    `IsLatest` tinyint(1) NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_ApplicationVersions` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ApplicationVersions_Applications_ApplicationId` FOREIGN KEY (`ApplicationId`) REFERENCES `Applications` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `AuditLogs` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` int NULL,
    `Action` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `EntityType` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `EntityId` int NULL,
    `DetailsJson` json NULL,
    `IpAddress` varchar(45) CHARACTER SET utf8mb4 NULL,
    `OccurredAt` datetime(6) NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_AuditLogs` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AuditLogs_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Licenses` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ApplicationId` int NOT NULL,
    `UserId` int NOT NULL,
    `LicenseKey` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
    `MaxDevices` int NOT NULL DEFAULT 2,
    `StartDate` datetime(6) NOT NULL,
    `EndDate` datetime(6) NULL,
    `IsActive` tinyint(1) NOT NULL DEFAULT TRUE,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_Licenses` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Licenses_Applications_ApplicationId` FOREIGN KEY (`ApplicationId`) REFERENCES `Applications` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Licenses_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `Devices` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `LicenseId` int NOT NULL,
    `Fingerprint` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
    `DeviceName` varchar(200) CHARACTER SET utf8mb4 NULL,
    `Status` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `LastIpAddress` varchar(45) CHARACTER SET utf8mb4 NULL,
    `LastSeenAt` datetime(6) NULL,
    `RegisteredAt` datetime(6) NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_Devices` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Devices_Licenses_LicenseId` FOREIGN KEY (`LicenseId`) REFERENCES `Licenses` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `DeviceSessions` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `DeviceId` int NOT NULL,
    `ConnectionId` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `IpAddress` varchar(45) CHARACTER SET utf8mb4 NULL,
    `UserAgent` varchar(500) CHARACTER SET utf8mb4 NULL,
    `ConnectedAt` datetime(6) NOT NULL,
    `DisconnectedAt` datetime(6) NULL,
    `LastHeartbeatAt` datetime(6) NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_DeviceSessions` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_DeviceSessions_Devices_DeviceId` FOREIGN KEY (`DeviceId`) REFERENCES `Devices` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `HubCommands` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `DeviceId` int NOT NULL,
    `CommandType` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Payload` json NOT NULL,
    `Status` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `SentAt` datetime(6) NOT NULL,
    `AcknowledgedAt` datetime(6) NULL,
    `ErrorMessage` varchar(1000) CHARACTER SET utf8mb4 NULL,
    `ClientGuid` char(36) COLLATE ascii_general_ci NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_HubCommands` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_HubCommands_Devices_DeviceId` FOREIGN KEY (`DeviceId`) REFERENCES `Devices` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `Telemetries` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `DeviceId` int NOT NULL,
    `EventType` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `EventDataJson` json NULL,
    `OccurredAt` datetime(6) NOT NULL,
    `ReceivedAt` datetime(6) NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_Telemetries` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Telemetries_Devices_DeviceId` FOREIGN KEY (`DeviceId`) REFERENCES `Devices` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE UNIQUE INDEX `IX_Applications_Name` ON `Applications` (`Name`);

CREATE INDEX `IX_ApplicationVersions_ApplicationId_ReleasedAt` ON `ApplicationVersions` (`ApplicationId`, `ReleasedAt`);

CREATE UNIQUE INDEX `IX_ApplicationVersions_ApplicationId_Version` ON `ApplicationVersions` (`ApplicationId`, `Version`);

CREATE INDEX `IX_AuditLogs_EntityType_EntityId` ON `AuditLogs` (`EntityType`, `EntityId`);

CREATE INDEX `IX_AuditLogs_OccurredAt` ON `AuditLogs` (`OccurredAt`);

CREATE INDEX `IX_AuditLogs_UserId_OccurredAt` ON `AuditLogs` (`UserId`, `OccurredAt`);

CREATE UNIQUE INDEX `IX_Devices_LicenseId_Fingerprint` ON `Devices` (`LicenseId`, `Fingerprint`);

CREATE UNIQUE INDEX `IX_DeviceSessions_ConnectionId` ON `DeviceSessions` (`ConnectionId`);

CREATE INDEX `IX_DeviceSessions_DeviceId_ConnectedAt` ON `DeviceSessions` (`DeviceId`, `ConnectedAt`);

CREATE UNIQUE INDEX `IX_HubCommands_ClientGuid` ON `HubCommands` (`ClientGuid`);

CREATE INDEX `IX_HubCommands_DeviceId_Status_SentAt` ON `HubCommands` (`DeviceId`, `Status`, `SentAt`);

CREATE INDEX `IX_Licenses_ApplicationId_UserId` ON `Licenses` (`ApplicationId`, `UserId`);

CREATE UNIQUE INDEX `IX_Licenses_LicenseKey` ON `Licenses` (`LicenseKey`);

CREATE INDEX `IX_Licenses_UserId` ON `Licenses` (`UserId`);

CREATE INDEX `IX_Telemetries_DeviceId_EventType_ReceivedAt` ON `Telemetries` (`DeviceId`, `EventType`, `ReceivedAt`);

CREATE INDEX `IX_Telemetries_ReceivedAt` ON `Telemetries` (`ReceivedAt`);

CREATE UNIQUE INDEX `IX_Users_Email` ON `Users` (`Email`);

CREATE UNIQUE INDEX `IX_Users_Username` ON `Users` (`Username`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20251109002817_InitialCreate', '8.0.21');

COMMIT;

START TRANSACTION;

CREATE TABLE `VpsContainers` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ContainerId` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
    `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Image` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `State` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Status` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `StartedAt` datetime(6) NULL,
    `CpuUsagePercent` decimal(5,2) NOT NULL,
    `MemoryUsageBytes` bigint NOT NULL,
    `MemoryLimitBytes` bigint NOT NULL,
    `NetworkRxBytes` bigint NOT NULL,
    `NetworkTxBytes` bigint NOT NULL,
    `LastUpdatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_VpsContainers` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `VpsResourceSnapshots` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Timestamp` datetime(6) NOT NULL,
    `CpuUsagePercent` decimal(5,2) NOT NULL,
    `MemoryTotalBytes` bigint NOT NULL,
    `MemoryUsedBytes` bigint NOT NULL,
    `DiskTotalBytes` bigint NOT NULL,
    `DiskUsedBytes` bigint NOT NULL,
    `NetworkRxBytesPerSec` bigint NOT NULL,
    `NetworkTxBytesPerSec` bigint NOT NULL,
    `LoadAverage1Min` decimal(5,2) NOT NULL,
    `LoadAverage5Min` decimal(5,2) NOT NULL,
    `LoadAverage15Min` decimal(5,2) NOT NULL,
    CONSTRAINT `PK_VpsResourceSnapshots` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `VpsWebsites` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Domain` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ServiceName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ContainerId` int NULL,
    `SslEnabled` tinyint(1) NOT NULL,
    `SslExpiresAt` datetime(6) NULL,
    `IsActive` tinyint(1) NOT NULL,
    `LastCheckedAt` datetime(6) NOT NULL,
    `LastStatusCode` int NULL,
    `LastResponseTimeMs` int NULL,
    CONSTRAINT `PK_VpsWebsites` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_VpsWebsites_VpsContainers_ContainerId` FOREIGN KEY (`ContainerId`) REFERENCES `VpsContainers` (`Id`) ON DELETE SET NULL
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_VpsContainers_ContainerId` ON `VpsContainers` (`ContainerId`);

CREATE INDEX `IX_VpsContainers_Name` ON `VpsContainers` (`Name`);

CREATE INDEX `IX_VpsResourceSnapshots_Timestamp` ON `VpsResourceSnapshots` (`Timestamp`);

CREATE INDEX `IX_VpsWebsites_ContainerId` ON `VpsWebsites` (`ContainerId`);

CREATE UNIQUE INDEX `IX_VpsWebsites_Domain` ON `VpsWebsites` (`Domain`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20251109232658_AddVpsTables', '8.0.21');

COMMIT;

START TRANSACTION;

ALTER TABLE `VpsWebsites` MODIFY COLUMN `LastCheckedAt` datetime(6) NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20251110201258_MakeLastCheckedAtNullable', '8.0.21');

COMMIT;

START TRANSACTION;

ALTER TABLE `Licenses` ADD `Status` int NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20251111130145_AddLicenseStatusColumn', '8.0.21');

COMMIT;

START TRANSACTION;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20251111130338_AddLicenseStatusField', '8.0.21');

COMMIT;

