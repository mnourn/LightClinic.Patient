using lightClinic.Patient.Contracts.DTOs.Auth;
using lightClinic.Patient.Contracts.Interfaces.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace lightClinic.Patient.Tests.Services;

[TestClass]
public class UnitTest_User : TestBase
{
    //[TestMethod]
    //public async Task Login_Success()
    //{
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var loginDto = new LoginDto { Email = "test@example.com", Password = "Password123!" };
    //    var result = await service.LoginAsync(loginDto);
    //    Assert.IsTrue(result.Succeeded);
    //    Assert.IsNotNull(result.Data);
    //    Assert.IsNotNull(result.Data.Token);
    //}

    //[TestMethod]
    //public async Task Login_InvalidCredentials_Failed()
    //{
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var loginDto = new LoginDto { Email = "test@example.com", Password = "WrongPassword" };
    //    var result = await service.LoginAsync(loginDto);
    //    Assert.IsFalse(result.Succeeded);
    //    Assert.IsNull(result.Data);
    //}

    [TestMethod]
    public async Task Register_Success()
    {
        var service = _serviceProvider.GetRequiredService<IUserService>();
        var registerDto = new RegisterUserDto
        {
            Email = "newuser@example.com",
            Password = "Password123!",
            FullName = "New User",
            PhoneNumber = "01023456789",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-25)),
            Gender = Shared.Enums.Gender.Male,
        };
        var result = await service.RegisterAsync(registerDto, CancellationToken.None);
        Assert.IsTrue(result.Succeeded);
        Assert.IsNotNull(result.Data);
    }

    //[TestMethod]
    //public async Task Register_InvalidData_Failed()
    //{
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var registerDto = new RegisterUserDto 
    //    { 
    //        Email = "newuser@example.com",
    //        Password = "Password123!",
    //        ConfirmPassword = "DifferentPassword"
    //    };
    //    var result = await service.RegisterAsync(registerDto, CancellationToken.None);
    //    Assert.IsFalse(result.Succeeded);
    //    Assert.IsNull(result.Data);
    //}

    //[TestMethod]
    //public async Task ForgetPassword_Success()
    //{
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var request = new ForgetPasswordRequestDto { Email = "test@example.com" };
    //    var result = await service.ForgetPasswordAsync(request, CancellationToken.None);
    //    Assert.IsTrue(result.Succeeded);
    //    Assert.IsNotNull(result.Data);
    //}

    //[TestMethod]
    //public async Task ForgetPassword_NonExistentEmail_Failed()
    //{
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var request = new ForgetPasswordRequestDto { Email = "nonexistent@example.com" };
    //    var result = await service.ForgetPasswordAsync(request, CancellationToken.None);
    //    Assert.IsFalse(result.Succeeded);
    //    Assert.IsNull(result.Data);
    //}

    //[TestMethod]
    //public async Task ResetPassword_Success()
    //{
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var request = new ResetPasswordRequestDto 
    //    { 
    //        Email = "test@example.com",
    //        NewPassword = "NewPassword123!",
    //        ConfirmPassword = "NewPassword123!",
    //        Otp = "123456"
    //    };
    //    var result = await service.ResetPasswordAsync(request, CancellationToken.None);
    //    Assert.IsTrue(result.Succeeded);
    //    Assert.IsNotNull(result.Data);
    //}

    //[TestMethod]
    //public async Task ResetPassword_InvalidData_Failed()
    //{
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var request = new ResetPasswordRequestDto 
    //    { 
    //        Email = "test@example.com",
    //        NewPassword = "NewPassword123!",
    //        ConfirmPassword = "DifferentPassword",
    //        Otp = "123456"
    //    };
    //    var result = await service.ResetPasswordAsync(request, CancellationToken.None);
    //    Assert.IsFalse(result.Succeeded);
    //    Assert.IsNull(result.Data);
    //}

    //[TestMethod]
    //public async Task Exists_ExistingUser_Success()
    //{
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var userId = Guid.NewGuid();
    //    var result = await service.ExistsAsync(userId, CancellationToken.None);
    //    Assert.IsTrue(result.Succeeded);
    //    Assert.IsTrue(result.Data);
    //}

    //[TestMethod]
    //public async Task Exists_NonExistingUser_Success()
    //{
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var userId = Guid.NewGuid();
    //    var result = await service.ExistsAsync(userId, CancellationToken.None);
    //    Assert.IsTrue(result.Succeeded);
    //    Assert.IsFalse(result.Data);
    //}

    //[TestMethod]
    //public async Task Get_Success()
    //{
    //    Login();
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var getResult = await service.GetAsync(_currentUserId, CancellationToken.None);
    //    Assert.IsTrue(getResult.Succeeded);
    //    Assert.IsNotNull(getResult.Data);
    //    Assert.AreEqual(_currentUserId, getResult.Data.Id);
    //}

    //[TestMethod]
    //public async Task GetAll_Success()
    //{
    //    Login();
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var getResult = await service.GetAllAsync(CancellationToken.None);
    //    Assert.IsTrue(getResult.Succeeded);
    //    Assert.IsNotNull(getResult.Data);
    //    Assert.IsTrue(getResult.Data.Any());
    //}

    //[TestMethod]
    //public async Task Get_NonExistentUser_Failed()
    //{
    //    Login();
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var getResult = await service.GetAsync(999, CancellationToken.None);
    //    Assert.IsFalse(getResult.Succeeded);
    //    Assert.IsNull(getResult.Data);
    //}

    //[TestMethod]
    //public async Task Create_Success()
    //{
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var createUserDto = new CreateUserDto
    //    {
    //        Email = "test@example.com",
    //        Password = "Test123!",
    //        FirstName = "Test",
    //        LastName = "User",
    //        PhoneNumber = "1234567890"
    //    };
    //    var result = await service.CreateAsync(createUserDto, CancellationToken.None);
    //    Assert.IsTrue(result.Succeeded);
    //    Assert.IsNotNull(result.Data);
    //}

    //[TestMethod]
    //public async Task Update_Success()
    //{
    //    Login();
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var updateUserDto = new UpdateUserDto
    //    {
    //        Id = _currentUserId,
    //        FirstName = "Updated",
    //        LastName = "User",
    //        PhoneNumber = "0987654321"
    //    };
    //    var result = await service.UpdateAsync(updateUserDto, CancellationToken.None);
    //    Assert.IsTrue(result.Succeeded);
    //    Assert.IsNotNull(result.Data);
    //    Assert.AreEqual("Updated", result.Data.FirstName);
    //}

    //[TestMethod]
    //public async Task Delete_Success()
    //{
    //    Login();
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var deleteResult = await service.DeleteAsync(_currentUserId, CancellationToken.None);
    //    Assert.IsTrue(deleteResult.Succeeded);

    //    // Verify User is deleted
    //    var getResult = await service.GetAsync(_currentUserId, CancellationToken.None);
    //    Assert.IsFalse(getResult.Succeeded);
    //    Assert.IsNull(getResult.Data);
    //}

    //[TestMethod]
    //public async Task ChangePassword_Success()
    //{
    //    Login();
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var changePasswordDto = new ChangePasswordDto
    //    {
    //        CurrentPassword = "Test123!",
    //        NewPassword = "NewTest123!",
    //        ConfirmPassword = "NewTest123!"
    //    };
    //    var result = await service.ChangePasswordAsync(_currentUserId, changePasswordDto, CancellationToken.None);
    //    Assert.IsTrue(result.Succeeded);
    //}

    //[TestMethod]
    //public async Task ChangePassword_InvalidCurrentPassword_Failed()
    //{
    //    Login();
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var changePasswordDto = new ChangePasswordDto
    //    {
    //        CurrentPassword = "WrongPassword",
    //        NewPassword = "NewTest123!",
    //        ConfirmPassword = "NewTest123!"
    //    };
    //    var result = await service.ChangePasswordAsync(_currentUserId, changePasswordDto, CancellationToken.None);
    //    Assert.IsFalse(result.Succeeded);
    //}

    //[TestMethod]
    //public async Task ChangePassword_PasswordsDoNotMatch_Failed()
    //{
    //    Login();
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var changePasswordDto = new ChangePasswordDto
    //    {
    //        CurrentPassword = "Test123!",
    //        NewPassword = "NewTest123!",
    //        ConfirmPassword = "DifferentPassword"
    //    };
    //    var result = await service.ChangePasswordAsync(_currentUserId, changePasswordDto, CancellationToken.None);
    //    Assert.IsFalse(result.Succeeded);
    //}

    //[TestMethod]
    //public async Task ResetPassword_PasswordsDoNotMatch_Failed()
    //{
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var resetPasswordDto = new ResetPasswordDto
    //    {
    //        Email = "test@example.com",
    //        Token = "valid_token",
    //        NewPassword = "NewTest123!",
    //        ConfirmPassword = "DifferentPassword"
    //    };
    //    var result = await service.ResetPasswordAsync(resetPasswordDto, CancellationToken.None);
    //    Assert.IsFalse(result.Succeeded);
    //}

    //[TestMethod]
    //public async Task ForgotPassword_Success()
    //{
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var forgotPasswordDto = new ForgotPasswordDto
    //    {
    //        Email = "test@example.com"
    //    };
    //    var result = await service.ForgotPasswordAsync(forgotPasswordDto, CancellationToken.None);
    //    Assert.IsTrue(result.Succeeded);
    //}

    //[TestMethod]
    //public async Task ForgotPassword_NonExistentEmail_Failed()
    //{
    //    var service = _serviceProvider.GetRequiredService<IUserService>();
    //    var forgotPasswordDto = new ForgotPasswordDto
    //    {
    //        Email = "nonexistent@example.com"
    //    };
    //    var result = await service.ForgotPasswordAsync(forgotPasswordDto, CancellationToken.None);
    //    Assert.IsFalse(result.Succeeded);
    //}
} 