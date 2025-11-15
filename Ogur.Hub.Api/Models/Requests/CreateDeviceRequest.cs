namespace Ogur.Hub.Api.Models.Requests;

public record CreateDeviceRequest(
    int LicenseId,
    string Hwid,
    string DeviceGuid,
    string DeviceName);