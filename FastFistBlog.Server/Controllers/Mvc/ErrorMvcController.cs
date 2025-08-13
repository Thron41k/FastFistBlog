using Microsoft.AspNetCore.Mvc;

namespace FastFistBlog.Server.Controllers.Mvc;

public class ErrorMvcController : Controller
{
    [Route("ErrorMvc/{code}")]
    public IActionResult HandleError(int code)
    {
        Response.StatusCode = code;

        return code switch
        {
            403 => View("AccessDenied"),
            404 => View("NotFound"),
            500 => View("ServerError"),
            _ => View("ServerError") 
        };
    }

    [Route("ErrorMvc/500")]
    public IActionResult ServerError()
    {
        Response.StatusCode = 500;
        return View("ServerError");
    }
}