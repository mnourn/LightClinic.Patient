using lightClinic.Patient.Contracts.Interfaces.Address;
using Microsoft.Extensions.DependencyInjection;

namespace lightClinic.Patient.Tests.Services;

[TestClass]
public class UnitTest_Address : TestBase
{
    [TestMethod]
    public async Task Get_Success()
    {
        Login();
        var createResult = await CreateAddressAsync(locationType: Shared.Enums.LocationType.Work);
        Assert.IsTrue(createResult.Succeeded);

        var service = _serviceProvider.GetRequiredService<IAddressService>();
        var getResult = await service.GetAsync(createResult.Data.Id, CancellationToken.None);
        Assert.IsTrue(getResult.Succeeded);
        Assert.IsNotNull(getResult.Data);
        Assert.AreEqual(createResult.Data.Id, getResult.Data.Id);
    }

    [TestMethod]
    public async Task GetAll_Success()
    {
        Login();
        var createResult = await CreateAddressAsync(locationType: Shared.Enums.LocationType.Work);
        Assert.IsTrue(createResult.Succeeded);

        var service = _serviceProvider.GetRequiredService<IAddressService>();
        var getResult = await service.GetAllAsync(CancellationToken.None);
        Assert.IsTrue(getResult.Succeeded);
        Assert.IsNotNull(getResult.Data);
        Assert.IsTrue(getResult.Data.Any());
    }

    [TestMethod]
    public async Task Get_NonExistentAddress_Failed()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<IAddressService>();
        var getResult = await service.GetAsync(999, CancellationToken.None);
        Assert.IsFalse(getResult.Succeeded);
        Assert.IsNull(getResult.Data);
    }

    [TestMethod]
    public async Task Create_Success()
    {
        Login();
        var result = await CreateAddressAsync(locationType: Shared.Enums.LocationType.Work);
        Assert.IsTrue(result.Succeeded);
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task Update_Success()
    {
        Login();
        var createResult = await CreateAddressAsync(locationType: Shared.Enums.LocationType.Work);
        Assert.IsTrue(createResult.Succeeded);
        Assert.IsNotNull(createResult.Data);

        var updateResult = await UpdateAddressAsync(createResult.Data.Id, locationType: Shared.Enums.LocationType.Work, location: "Updated Location");
        Assert.IsTrue(updateResult.Succeeded);
        Assert.IsNotNull(updateResult.Data);
        Assert.AreEqual("Updated Location", updateResult.Data.Location);
    }

    [TestMethod]
    public async Task Delete_Success()
    {
        Login();
        var createResult = await CreateAddressAsync(locationType: Shared.Enums.LocationType.Work);
        Assert.IsTrue(createResult.Succeeded);

        var service = _serviceProvider.GetRequiredService<IAddressService>();
        var deleteResult = await service.DeleteAsync(createResult.Data.Id, CancellationToken.None);
        Assert.IsTrue(deleteResult.Succeeded);

        // Verify Address is deleted
        var getResult = await service.GetAsync(createResult.Data.Id, CancellationToken.None);
        Assert.IsFalse(getResult.Succeeded);
        Assert.IsNull(getResult.Data);
    }
}
