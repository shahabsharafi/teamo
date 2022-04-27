using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Teamo.Identity.API.Infrastructure;
using Teamo.Identity.API.Infrastructure.Domain;
using Teamo.Identity.API.Infrastructure.Models;

namespace Teamo.Identity.API.Infrastructure.Domain
{
    public class ChangePasswordCommand : IRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;        
    }
}

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<ChangePasswordHandler> _logger;

    public ChangePasswordHandler(
        ILogger<ChangePasswordHandler> logger,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    /// <summary>
    /// <para>Handle change password proccess</para>
    /// <br>1- check user is exists</br> 
    /// <br>2- change password</br> 
    /// <br>3- update</br> 
    /// <br>3- refresh signin</br> 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken = default)
    {
        var userIdentity = await _userManager.FindByNameAsync(request.UserName);
        if (userIdentity == null || userIdentity.IsDeleted)
            throw new Exception(Constants.Message.USER_NOT_FOUND);

        IdentityResult changePasswordResult = 
            await _userManager.ChangePasswordAsync(userIdentity, request.OldPassword, request.NewPassword);
        if (!changePasswordResult.Succeeded)        
            throw new Exception(Constants.Message.CHANGE_PASSWORD_FAILD);

        IdentityResult updateResult =
            await _userManager.UpdateAsync(userIdentity);
        if (!updateResult.Succeeded)
            throw new Exception(Constants.Message.CHANGE_PASSWORD_FAILD);

        await _signInManager.RefreshSignInAsync(userIdentity);

        _logger.LogInformation($"{request.UserName} pasword recoverd successfully");

        return Unit.Value;        
    }
}
