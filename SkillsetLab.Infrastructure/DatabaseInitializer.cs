using Application.Infrastructure;
using Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SkillsetLab.Infrastructure;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<MyDbContext>();
        await context.Database.MigrateAsync();

        await SeedDatabaseDataAsync(context, services);
    }
    
    private static async Task SeedDatabaseDataAsync(
        MyDbContext context, IServiceProvider services)
    {
        var adminUserSettings = services.GetService<IOptions<AdminUser>>()?.Value
                                ?? throw new Exception("Админ не задан");

        var adminUser = context.Users.FirstOrDefault(u => u.UserRoles.Select(ur => ur.Role.Name)
            .Contains(Role.Administrator));

        var userManager = services.GetService<UserManager<User>>()
                          ?? throw new Exception("Сервис юзеров не задан");

        await Transaction.Do(async () =>
        {
            if (adminUser == null)
            {
                // create new admin user
                await CreateUserAsync(adminUserSettings, userManager);
            }
            else
            {
                await UpdateUserAsync(adminUserSettings, adminUser, userManager);
            }
        });
    }
    
    private static async Task UpdateUserAsync(
        AdminUser adminUserSettings,
        User adminUser, UserManager<User> userManager)
    {
        var isValidPassword = await userManager.CheckPasswordAsync(
            adminUser, adminUserSettings.Password);

        if (!isValidPassword)
        {
            // update password
            var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(adminUser);
            var resetPasswordResult = await userManager.ResetPasswordAsync(
                adminUser, passwordResetToken, adminUserSettings.Password);

            if (!resetPasswordResult.Succeeded)
                throw new Exception("Ошибка смены пароля админа");
        }

        // update existing user
        adminUser.UserName = adminUserSettings.Name;
        adminUser.Email = adminUserSettings.Email;

        var result = await userManager.UpdateAsync(adminUser);
        if (!result.Succeeded)
            throw new Exception("Ошибка обновления админа");
    }
    
    private static async Task CreateUserAsync(
        AdminUser adminUserSettings, UserManager<User> userManager)
    {
        var user = new User
        {
            UserName = adminUserSettings.Name,
            Email = adminUserSettings.Email,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true
        };

        var result = await userManager.CreateAsync(user, adminUserSettings.Password);
        if (!result.Succeeded)
            throw new Exception("Ошибка инициализации админа");

        result = await userManager.AddToRoleAsync(user, Role.Administrator);
        if (!result.Succeeded)
            throw new Exception("Ошибка добавления роли админа");
    }
}

public class AdminUser
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}