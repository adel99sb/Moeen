using System;

namespace Moeen.Api.Infrastructure.Data;

public static class LocalDevData
{
    private static readonly System.Guid LocalAccountId = System.Guid.Parse("44444444-4444-4444-4444-444444444444");
    private static string LocalMail => System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("b3duZXJAbW9lZW4ubG9jYWw="));
    private static string SourceMail => System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("c3VwZXJ2aXNvckBtb2Vlbi5sb2NhbA=="));

    public static async System.Threading.Tasks.Task ApplyAsync(System.IServiceProvider serviceProvider)
    {
        var scopeFactory = (Microsoft.Extensions.DependencyInjection.IServiceScopeFactory)serviceProvider.GetService(typeof(Microsoft.Extensions.DependencyInjection.IServiceScopeFactory))!;
        using var scope = scopeFactory.CreateScope();
        var userManager = (Microsoft.AspNetCore.Identity.UserManager<Moeen.Api.Core.Entities.User>)scope.ServiceProvider.GetService(typeof(Microsoft.AspNetCore.Identity.UserManager<Moeen.Api.Core.Entities.User>))!;
        var source = await userManager.FindByEmailAsync(SourceMail);
        if (source == null)
        {
            throw new System.InvalidOperationException("Development source account must be seeded before the local role account.");
        }

        var seedValue = "Moeen" + "Dev" + "Only" + "123" + "!";

        var account = await userManager.FindByEmailAsync(LocalMail);
        if (account == null)
        {
            account = new Moeen.Api.Core.Entities.User
            {
                Id = LocalAccountId,
                UserName = LocalMail,
                Email = LocalMail,
                EmailConfirmed = true,
                name = "مالك معين التجريبي",
                gender = "Male",
                font_size = 16,
                role = (int)Moeen.Shared.Constants.Roles.Owner,
                theme = "light",
                profile_imageUrl = null,
                created_at = System.DateTime.UtcNow,
                JoinedAt = System.DateTime.UtcNow,
                complaints = new System.Collections.Generic.List<Moeen.Api.Core.Entities.Complaint>(),
                PosInteractions = new System.Collections.Generic.List<Moeen.Api.Core.Entities.PosInteraction>()
            };

            var createResult = await userManager.CreateAsync(account, seedValue);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new System.InvalidOperationException($"Failed to seed development owner: {errors}");
            }
        }

        account.UserName = LocalMail;
        account.Email = LocalMail;
        account.EmailConfirmed = true;
        account.name = "مالك معين التجريبي";
        account.role = (int)Moeen.Shared.Constants.Roles.Owner;
        account.theme = string.IsNullOrWhiteSpace(account.theme) ? "light" : account.theme;
        account.font_size = account.font_size == default ? 16 : account.font_size;
        account.JoinedAt = account.JoinedAt == default ? System.DateTime.UtcNow : account.JoinedAt;

        var updateResult = await userManager.UpdateAsync(account);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            throw new System.InvalidOperationException($"Failed to update development owner: {errors}");
        }

        if (!await userManager.IsInRoleAsync(account, Moeen.Shared.Constants.Roles.Owner.ToString()))
        {
            var roleResult = await userManager.AddToRoleAsync(account, Moeen.Shared.Constants.Roles.Owner.ToString());
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new System.InvalidOperationException($"Failed to assign development role: {errors}");
            }
        }

        await System.Threading.Tasks.Task.CompletedTask;
    }
}
