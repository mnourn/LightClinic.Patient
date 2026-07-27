using lightClinic.Patient.Contracts.DTOs.Setting;
using lightClinic.Patient.Contracts.Interfaces.Setting;
using Microsoft.Extensions.DependencyInjection;

namespace lightClinic.Patient.Tests.Services;

[TestClass]
public class UnitTest_Setting : TestBase
{
    [TestMethod]
    public async Task Get_Success()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<ISettingService>();
        var getResult = await service.GetAsync(CancellationToken.None);
        Assert.IsTrue(getResult.Succeeded);
        Assert.IsNotNull(getResult.Data);
    }

    [TestMethod]
    public async Task Update_Success()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<ISettingService>();
        var getResult = await service.GetAsync(CancellationToken.None);
        Assert.IsTrue(getResult.Succeeded);
        Assert.IsNotNull(getResult.Data);

        UpdateSettingDto settingDto = new UpdateSettingDto();
        settingDto.Language = getResult.Data.Language;
        settingDto.NotificationEnabled = !getResult.Data.NotificationEnabled;
        var updateResult = await service.UpdateAsync(settingDto, CancellationToken.None);
        getResult = await service.GetAsync(CancellationToken.None);
        Assert.IsTrue(updateResult.Succeeded);
        Assert.IsNotNull(updateResult.Data);
        Assert.AreEqual(settingDto.NotificationEnabled, getResult.Data.NotificationEnabled);
    }
}