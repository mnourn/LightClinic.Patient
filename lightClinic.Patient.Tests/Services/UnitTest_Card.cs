using lightClinic.Patient.Contracts.Interfaces.Card;
using Microsoft.Extensions.DependencyInjection;

namespace lightClinic.Patient.Tests.Services;

[TestClass]
public class UnitTest_Card : TestBase
{
    [TestMethod]
    public async Task Get_Success()
    {
        Login();
        var createResult = await CreateCardAsync();
        Assert.IsTrue(createResult.Succeeded);

        var service = _serviceProvider.GetRequiredService<ICardService>();
        var getResult = await service.GetAsync(createResult.Data.Id, CancellationToken.None);
        Assert.IsTrue(getResult.Succeeded);
        Assert.IsNotNull(getResult.Data);
        Assert.AreEqual(createResult.Data.Id, getResult.Data.Id);
    }

    [TestMethod]
    public async Task GetAll_Success()
    {
        Login();
        var createResult = await CreateCardAsync();
        Assert.IsTrue(createResult.Succeeded);

        var service = _serviceProvider.GetRequiredService<ICardService>();
        var getResult = await service.GetAllAsync(CancellationToken.None);
        Assert.IsTrue(getResult.Succeeded);
        Assert.IsNotNull(getResult.Data);
        Assert.IsTrue(getResult.Data.Any());
    }

    [TestMethod]
    public async Task Get_NonExistentCard_Failed()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<ICardService>();
        var getResult = await service.GetAsync(999, CancellationToken.None);
        Assert.IsFalse(getResult.Succeeded);
        Assert.IsNull(getResult.Data);
    }

    [TestMethod]
    public async Task Create_Success()
    {
        Login();
        var result = await CreateCardAsync();
        Assert.IsTrue(result.Succeeded);
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task Update_Success()
    {
        Login();
        var createResult = await CreateCardAsync();
        Assert.IsTrue(createResult.Succeeded);
        Assert.IsNotNull(createResult.Data);

        var updateResult = await UpdateCardAsync(createResult.Data.Id, cardNumber: 5555555555554444);
        Assert.IsTrue(updateResult.Succeeded);
        Assert.IsNotNull(updateResult.Data);
        Assert.AreEqual(5555555555554444, updateResult.Data.CardNumber);
    }

    [TestMethod]
    public async Task Delete_Success()
    {
        Login();
        var createResult = await CreateCardAsync();
        Assert.IsTrue(createResult.Succeeded);

        var service = _serviceProvider.GetRequiredService<ICardService>();
        var deleteResult = await service.DeleteAsync(createResult.Data.Id, CancellationToken.None);
        Assert.IsTrue(deleteResult.Succeeded);

        // Verify Card is deleted
        var getResult = await service.GetAsync(createResult.Data.Id, CancellationToken.None);
        Assert.IsFalse(getResult.Succeeded);
        Assert.IsNull(getResult.Data);
    }
}