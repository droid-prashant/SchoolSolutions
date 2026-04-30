using Application.Common.Interfaces;
using Application.Identity.Dtos;
using Application.Identity.Interfaces;
using Application.Identity.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Transactions;

namespace Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public IdentityService(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task CreateRoleAsync(UserRoleDto userRoleDto, CancellationToken cancellationToken)
        {
            try
            {

                IdentityResult finalResult = new IdentityResult();
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    ApplicationRole appRole = new ApplicationRole
                    {
                        Name = userRoleDto.RoleName,
                        Description = userRoleDto.Description
                    };
                    IdentityResult result = await _roleManager.CreateAsync(appRole);
                    if (result.Succeeded)
                    {
                        ApplicationRole? role = await _roleManager.FindByNameAsync(userRoleDto.RoleName);
                        if (role != null)
                        {
                            foreach (var permission in userRoleDto.UserPermissions)
                            {
                                if (Enum.IsDefined(typeof(Domain.Enums.Permission), permission.PermissionValue))
                                {
                                    Claim claim = new Claim(CustomClaimType.Permission, permission.PermissionValue.ToString());
                                    finalResult = await _roleManager.AddClaimAsync(role, claim);
                                }
                            }
                        }
                    }
                    scope.Complete();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateUserAsync(UserDto userDto, CancellationToken cancellationToken)
        {
            IdentityResult finalResult = new IdentityResult();
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))

            {
                ApplicationUser appUser = new ApplicationUser
                {
                    UserName = userDto.UserName,
                    Address = userDto.Address,
                    Email = userDto.Email,
                    IsActive = true
                };
                IdentityResult user = await _userManager.CreateAsync(appUser);
                if (user.Succeeded)
                {
                    IdentityResult addPasswordHash = await _userManager.AddPasswordAsync(appUser, userDto.Password);
                    if (addPasswordHash.Succeeded)
                    {
                        finalResult = await _userManager.AddToRolesAsync(appUser, userDto.Roles);
                    }
                }
                scope.Complete();
            }
        }

        public async Task<TokenViewModel> LoginUserAsync(LoginUserDto loginUserDto, CancellationToken cancellationToken)
        {
            var identityUser = await _userManager.FindByNameAsync(loginUserDto.UserName);
            if (identityUser == null)
            {
                return new TokenViewModel { Error = "User does not exist", StatusCode = 401, Succeded = false };
            }
            else if (identityUser.IsActive == false)
            {
                return new TokenViewModel { Error = "User is deactivated", StatusCode = 401, Succeded = false };
            }
            else
            {
                var result = await _signInManager.CheckPasswordSignInAsync(identityUser, loginUserDto.Password, true);
                if (!result.Succeeded)
                {
                    if (result.IsLockedOut)
                    {
                        identityUser.IsActive = false;
                        return new TokenViewModel { Error = "Your account has been locked, please contact the admin", StatusCode = 401, Succeded = false };
                    }
                    identityUser.AccessFailedCount++;
                    return new TokenViewModel { Error = "Credential is invalid", StatusCode = 401, Succeded = false };
                }
                else
                {
                    List<Claim> claims = await ConstructUserClaimAsync(identityUser, loginUserDto.AcademicYear);
                    JwtSecurityToken token = GenerateJwtTokenAsync(claims);
                    TokenViewModel tokenResult = new TokenViewModel
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        Expiration = token.ValidTo,
                        Succeded = true
                    };
                    return tokenResult;
                }
            }
        }
        private async Task<List<Claim>> ConstructUserClaimAsync(ApplicationUser user, string academicYear)
        {
            var roles = await _userManager.GetRolesAsync(user);
            bool isAcademicClaimExist = await CheckAcademicClaimExist(user);
            if (isAcademicClaimExist)
            {
                var existingClaim = (await _userManager.GetClaimsAsync(user)).First(x => x.Type == CustomClaimType.AcademicYear);
                await _userManager.RemoveClaimAsync(user, existingClaim);
            }
            Claim claim = new Claim(CustomClaimType.AcademicYear, academicYear);
            await _userManager.AddClaimAsync(user, claim);
            List<Claim> claims = (await _userManager.GetClaimsAsync(user)).ToList();

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                claims.Add(new Claim("roles", role));

                ApplicationRole? appRole = await _roleManager.FindByNameAsync(role);
                if (appRole != null)
                {
                    IList<Claim> roleClaims = await _roleManager.GetClaimsAsync(appRole);
                    claims.AddRange(roleClaims);
                }
            }

            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("userId", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim("username", user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            });

            return claims
                .GroupBy(x => new { x.Type, x.Value })
                .Select(x => x.First())
                .ToList();
        }

        private async Task<bool> CheckAcademicClaimExist(ApplicationUser user)
        {
            bool isExist = (await _userManager.GetClaimsAsync(user)).Any(x => x.Type == CustomClaimType.AcademicYear);
            return isExist;
        }
        private JwtSecurityToken GenerateJwtTokenAsync(List<Claim> claims)
        {
            var jwtKey = _configuration["Tokens:JwtKey"];
            var jwtIssuer = _configuration["Tokens:JwtIssuer"];
            var jwtAudience = _configuration["Tokens:JwtAudience"];
            var jwtValidMinutes = _configuration["Tokens:JwtValidMinutes"];
            var token = new JwtSecurityToken();
            if (jwtKey != null && jwtIssuer != null && jwtAudience != null && jwtValidMinutes != null)
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var signingCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtValidMinutes)),
                signingCredentials: signingCred
                );
            }
            return token;
        }
    }
}
