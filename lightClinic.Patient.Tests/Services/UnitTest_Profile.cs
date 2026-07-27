using lightClinic.Patient.Contracts.DTOs.FamilyMember;
using lightClinic.Patient.Contracts.DTOs.Profile;
using lightClinic.Patient.Contracts.Interfaces.Profile;
using Microsoft.Extensions.DependencyInjection;

namespace lightClinic.Patient.Tests.Services;

[TestClass]
public class UnitTest_Profile : TestBase
{
    [TestMethod]
    public async Task Create_Success()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<IProfileService>();
        var userDto = new CreateUserDto
        {
            Email = "newuser@example.com",
            FullName = "New User",
            PhoneNumber = "01023456789",
            Gender = Shared.Enums.Gender.Male,
            Address = "Cairo, Egypt",
        };
        var result = await service.CreateAsync(userDto, CancellationToken.None);
        Assert.IsTrue(result.Succeeded);
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task Get_Success()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<IProfileService>();
        var result = await service.GetAsync(CancellationToken.None);
        Assert.IsTrue(result.Succeeded);
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task UpdateProfile_Success()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<IProfileService>();
        var updateDto = new UpdateProfileDto
        {
            FullName = "Jane",
            PhoneNumber = "01023456789"
        };
        var result = await service.UpdateAsync(updateDto, CancellationToken.None);
        Assert.IsTrue(result.Succeeded);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual(updateDto.FullName, result.Data.FullName);
        Assert.AreEqual(updateDto.PhoneNumber, result.Data.PhoneNumber);
    }

    [TestMethod]
    public async Task ChangePassword_Success()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<IProfileService>();
        var changePasswordDto = new UpdatePasswordDto
        {
            OldPassword = "123456a@A",
            NewPassword = "NewPassword123!",
        };
        var result = await service.UpdatePasswordAsync(changePasswordDto, CancellationToken.None);
        Assert.IsTrue(result.Succeeded);
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task AddFamilyMember_Success()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<IProfileService>();
        var addFamilyMemberDto = new CreateFamilyMemberDto
        {
            FullName = "John Doe",
            PhoneNumber = "01023456789",
            FamilyMembership = Shared.Enums.FamilyMembership.Son,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-20))
        };
        var result = await service.CreateFamilyMemberAsync(addFamilyMemberDto, CancellationToken.None);
        Assert.IsTrue(result.Succeeded);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual(addFamilyMemberDto.FullName, result.Data.FullName);
        Assert.AreEqual(addFamilyMemberDto.PhoneNumber, result.Data.PhoneNumber);
        Assert.AreEqual(addFamilyMemberDto.FamilyMembership, result.Data.FamilyMembership);
    }

    [TestMethod]
    public async Task UpdateFamilyMember_Success()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<IProfileService>();
        
        // First add a family member
        var addFamilyMemberDto = new CreateFamilyMemberDto
        {
            FullName = "John Doe",
            PhoneNumber = "01023456789",
            FamilyMembership = Shared.Enums.FamilyMembership.Son,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-20))
        };
        var addResult = await service.CreateFamilyMemberAsync(addFamilyMemberDto, CancellationToken.None);
        Assert.IsTrue(addResult.Succeeded);

        // Then update the family member
        var updateFamilyMemberDto = new UpdateFamilyMemberDto
        {
            Id = addResult.Data.Id,
            FullName = "John Smith",
            PhoneNumber = "01023456789",
            FamilyMembership = Shared.Enums.FamilyMembership.Daughter,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-18))
        };
        var updateResult = await service.UpdateFamilyMemberAsync(updateFamilyMemberDto, CancellationToken.None);
        Assert.IsTrue(updateResult.Succeeded);
        Assert.IsNotNull(updateResult.Data);
        Assert.AreEqual(updateFamilyMemberDto.FullName, updateResult.Data.FullName);
        Assert.AreEqual(updateFamilyMemberDto.PhoneNumber, updateResult.Data.PhoneNumber);
        Assert.AreEqual(updateFamilyMemberDto.FamilyMembership, updateResult.Data.FamilyMembership);
    }

    [TestMethod]
    public async Task UpdateFamilyMember_NonExistentMember_Failed()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<IProfileService>();
        var updateFamilyMemberDto = new UpdateFamilyMemberDto
        {
            Id = Guid.NewGuid(),
            FullName = "John Smith",
            PhoneNumber = "01023456789",
            FamilyMembership = Shared.Enums.FamilyMembership.Daughter,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-18))
        };
        var result = await service.UpdateFamilyMemberAsync(updateFamilyMemberDto, CancellationToken.None);
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public async Task RemoveFamilyMember_Success()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<IProfileService>();
        
        // First add a family member
        var addFamilyMemberDto = new CreateFamilyMemberDto
        {
            FullName = "John Doe",
            PhoneNumber = "01023456789",
            FamilyMembership = Shared.Enums.FamilyMembership.Son,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-20))
        };
        var addResult = await service.CreateFamilyMemberAsync(addFamilyMemberDto, CancellationToken.None);
        Assert.IsTrue(addResult.Succeeded);

        // Then remove the family member
        var removeResult = await service.DeleteFamilyMemberAsync(addResult.Data.Id, CancellationToken.None);
        Assert.IsTrue(removeResult.Succeeded);

        // Verify family member is removed
        var getResult = await service.GetAllFamilyMembersAsync(CancellationToken.None);
        Assert.IsTrue(getResult.Succeeded);
        Assert.IsFalse(getResult.Data.Any(fm => fm.Id == addResult.Data.Id));
    }

    [TestMethod]
    public async Task RemoveFamilyMember_NonExistentMember_Failed()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<IProfileService>();
        var result = await service.DeleteFamilyMemberAsync(Guid.NewGuid(), CancellationToken.None);
        Assert.IsFalse(result.Succeeded);
    }

    [TestMethod]
    public async Task GetFamilyMembers_Success()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<IProfileService>();
        
        // First add a family member
        var addFamilyMemberDto = new CreateFamilyMemberDto
        {
            FullName = "John Doe",
            PhoneNumber = "01023456789",
            FamilyMembership = Shared.Enums.FamilyMembership.Son,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-20))
        };
        var addResult = await service.CreateFamilyMemberAsync(addFamilyMemberDto, CancellationToken.None);
        Assert.IsTrue(addResult.Succeeded);

        // Then get all family members
        var getResult = await service.GetAllFamilyMembersAsync(CancellationToken.None);
        Assert.IsTrue(getResult.Succeeded);
        Assert.IsNotNull(getResult.Data);
        Assert.IsTrue(getResult.Data.Any());
        Assert.IsTrue(getResult.Data.Any(fm => fm.Id == addResult.Data.Id));
    }

    [TestMethod]
    public async Task SearchAsync_Success()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<IProfileService>();
        
        // First create a profile to search for
        var createUserDto = new CreateUserDto
        {
            Email = "searchtest@example.com",
            FullName = "Search Test User",
            PhoneNumber = "01023456789",
            Gender = Shared.Enums.Gender.Male,
            Address = "Cairo, Egypt",
        };
        var createResult = await service.CreateAsync(createUserDto, CancellationToken.None);
        Assert.IsTrue(createResult.Succeeded);
        
        // Search using part of the profile ID
        var profileId = createResult.Data.Id;
        var searchTerm = profileId.ToString().Substring(0, 8); // Use first 8 characters of GUID
        var result = await service.SearchAsync(searchTerm, CancellationToken.None);
        
        Assert.IsTrue(result.Succeeded);
        Assert.IsNotNull(result.Data);
        Assert.IsTrue(result.Data.Any());
        Assert.IsTrue(result.Data.Any(p => p.Id == profileId));
    }

    [TestMethod]
    public async Task SearchAsync_EmptySearchTerm_Failed()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<IProfileService>();
        
        var result = await service.SearchAsync("", CancellationToken.None);
        
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.Data);
        Assert.AreEqual("Search term cannot be empty!", result.Message);
    }

    [TestMethod]
    public async Task SearchAsync_WhitespaceSearchTerm_Failed()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<IProfileService>();
        
        var result = await service.SearchAsync("   ", CancellationToken.None);
        
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.Data);
        Assert.AreEqual("Search term cannot be empty!", result.Message);
    }

    [TestMethod]
    public async Task SearchAsync_NoResults_Success()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<IProfileService>();
        
        // Search for a non-existent profile ID
        var result = await service.SearchAsync("00000000-0000-0000-0000-000000000000", CancellationToken.None);
        
        Assert.IsTrue(result.Succeeded);
        Assert.IsNotNull(result.Data);
        Assert.IsFalse(result.Data.Any());
    }

    [TestMethod]
    public async Task GetByProfileAsync_Success()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<IProfileService>();
        
        // First create a profile to get
        var createUserDto = new CreateUserDto
        {
            Email = "getprofiletest@example.com",
            FullName = "Get Profile Test User",
            PhoneNumber = "01023456789",
            Gender = Shared.Enums.Gender.Female,
            Address = "Alexandria, Egypt",
        };
        var createResult = await service.CreateAsync(createUserDto, CancellationToken.None);
        Assert.IsTrue(createResult.Succeeded);
        
        // Get the profile by ID
        var result = await service.GetByProfileAsync(createResult.Data.Id, CancellationToken.None);
        
        Assert.IsTrue(result.Succeeded);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual(createResult.Data.Id, result.Data.Id);
        Assert.AreEqual(createUserDto.FullName, result.Data.FullName);
        Assert.AreEqual(createUserDto.PhoneNumber, result.Data.PhoneNumber);
        Assert.AreEqual(createUserDto.Gender, result.Data.Gender);
        Assert.AreEqual(createUserDto.Address, result.Data.Address);
    }

    [TestMethod]
    public async Task GetByProfileAsync_EmptyGuid_Failed()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<IProfileService>();
        
        var result = await service.GetByProfileAsync(Guid.Empty, CancellationToken.None);
        
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.Data);
        Assert.AreEqual("Invalid Profile Id!", result.Message);
    }

    [TestMethod]
    public async Task GetByProfileAsync_NonExistentProfile_Failed()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<IProfileService>();
        
        var result = await service.GetByProfileAsync(Guid.NewGuid(), CancellationToken.None);
        
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.Data);
        Assert.AreEqual("Profile not found!", result.Message);
    }

    [TestMethod]
    public async Task GetByIdsAsync_Success()
    {
        Login();
        var service = _serviceProvider.GetRequiredService<IProfileService>();
        var userDto = new CreateUserDto
        {
            Email = "newuser@example.com",
            FullName = "New User",
            PhoneNumber = "01023456789",
            Gender = Shared.Enums.Gender.Male,
            Address = "Cairo, Egypt",
        };
        var result = await service.CreateAsync(userDto, CancellationToken.None);
        Assert.IsTrue(result.Succeeded);
        Assert.IsNotNull(result.Data);

        Login(result.Data.UserId);

        var addressResult = await CreateAddressAsync();
        Assert.IsTrue(addressResult.Succeeded);
        Assert.IsNotNull(addressResult.Data);

        var getResult = await service.GetByIdsAsync([result.Data.UserId], CancellationToken.None);

        Assert.IsTrue(getResult.Succeeded);
        Assert.IsNotNull(getResult.Data);
        Assert.AreEqual(1, getResult.Data.Count);
        Assert.IsNotNull(getResult.Data[0].CityId);
        Assert.IsNotNull(getResult.Data[0].GovernmentId);
        Assert.IsNotNull(getResult.Data[0].Latitude);
        Assert.IsNotNull(getResult.Data[0].Longitude);
    }
}