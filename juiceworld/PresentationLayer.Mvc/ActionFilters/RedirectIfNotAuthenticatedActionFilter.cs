using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PresentationLayer.Mvc.ActionFilters;

public class RedirectIfNotAuthenticatedActionFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;

        if (user.Identity is null || !user.Identity.IsAuthenticated ||
            !int.TryParse(user.FindFirstValue(ClaimTypes.Sid) ?? string.Empty, out _))
            context.Result = new RedirectToActionResult(Constants.DefaultAction, Constants.DefaultController,
                new { area = Constants.DefaultArea });

        base.OnActionExecuting(context);
    }
}