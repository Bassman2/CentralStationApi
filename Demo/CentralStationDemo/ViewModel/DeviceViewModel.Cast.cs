using System;
using System.Collections.Generic;
using System.Text;

namespace CentralStationDemo.ViewModel;

partial class DeviceViewModel
{
    private readonly CentralStation centralStation;

    //public DeviceViewModel(Device device, CentralStation centralStation)
    //{
    //    this.centralStation = centralStation;

    //    DeviceId = device.DeviceId;
    //    Version = device.Version;
    //    DeviceType = device.DeviceType;
    //    DeviceTypeName = Enum.IsDefined<DeviceType>(device.DeviceType) ? device.DeviceType.ToString() : ((ushort)device.DeviceType).ToString("X4");
    //    IconUri = device.IconUri;

    //    //Measurements.Add(new DeviceMeasurementViewModel(new DeviceMeasurement() { Name = "Test A" }));
    //}
    public DeviceViewModel(DeviceModel model, CentralStation centralStation)
    {
        this.centralStation = centralStation;

        DeviceId = model.Id;
        Version = model.Version;
        DeviceType = model.Type;
        DeviceTypeName = "XXX"; // Enum.IsDefined<DeviceType>(model.DeviceType) ? model.DeviceType.ToString() : ((ushort)model.DeviceType).ToString("X4");
        ImagePath = model.ImagePath;

        //IconUri = model.IconUri;

        //Measurements.Add(new DeviceMeasurementViewModel(new DeviceMeasurement() { Name = "Test A" }));
    }

    public static List<DeviceViewModel> FromModels(IEnumerable<DeviceModel>? models, CentralStation centralStation)
            => models?.Select(model => new DeviceViewModel(model, centralStation)).ToList() ?? [];
}

static class DeviceViewModelStaticCast
{
    public static List<DeviceViewModel> FromModels(this IEnumerable<DeviceModel>? models, CentralStation centralStation)
        => models?.Select(model => new DeviceViewModel(model, centralStation)).ToList() ?? [];

}

