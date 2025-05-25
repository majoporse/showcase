using Commons.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PresentationLayer.Mvc.ActionFilters;

public class RedirectIfNotAdminActionFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;

        if (user.Identity is null || !user.Identity.IsAuthenticated ||
            !user.IsInRole(UserRole.Admin.ToString()))
            context.Result = new RedirectToActionResult(Constants.DefaultAction, Constants.DefaultController,
                new { area = Constants.DefaultArea });

        base.OnActionExecuting(context);
    }
}