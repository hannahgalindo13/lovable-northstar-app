using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/privacy")]
[Authorize]
public class PrivacyController : ControllerBase
{
    [HttpGet]
    public ActionResult Get()
    {
        return Ok(new
        {
            title = "Privacy Policy",
            collectedData = "We collect account email, authentication cookies, donation records, and basic technical logs.",
            purpose = "Data is collected to secure accounts, process donations, and operate reporting features.",
            deletionRequest = "Users can request deletion by contacting privacy@northstarsanctuary.org with their account email."
        });
    }
}
