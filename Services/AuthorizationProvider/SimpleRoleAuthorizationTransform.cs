using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace WebRegistry.Services.AuthorizationProvider
{
    public class SimpleRoleAuthorizationTransform : IClaimsTransformation
    {
        private static readonly string RoleClaimType = $"http://{typeof(SimpleRoleAuthorizationTransform).FullName.Replace('.', '/')}/role";
        private readonly ISimpleRoleProvider _roleProvider;
        public SimpleRoleAuthorizationTransform(ISimpleRoleProvider roleProvider)
        {
            _roleProvider = roleProvider ?? throw new ArgumentNullException(nameof(roleProvider));
        }

        async Task<ClaimsPrincipal> IClaimsTransformation.TransformAsync(ClaimsPrincipal principal)
        {
            // Приведем основной идентификатор в идентификатор утверждений, чтобы получить доступ к утверждениям
            var oldIdentity = (ClaimsIdentity)principal.Identity;

            // "Клонируем" юзера чтобы избежать побочных эффектов
            // NB: Заменяем тип утверждений на наш собственный
            var newIdentity = new ClaimsIdentity(
                oldIdentity.Claims,
                oldIdentity.AuthenticationType,
                oldIdentity.NameClaimType,
                RoleClaimType);

            // Извлекаем роли для пользователя и добавьте утверждения правильного типа, чтобы роли можно было распознать
            var roles = await _roleProvider.GetUserRolesAsync(newIdentity.Name);
            newIdentity.AddClaims(roles.Select(r => new Claim(RoleClaimType, r)));

            // Create and return a new claims principal
            return new ClaimsPrincipal(newIdentity);
        }
    }
}
