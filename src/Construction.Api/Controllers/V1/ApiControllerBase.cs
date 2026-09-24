using Construction.Api.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Route(ApiRoutes.Version1 + "/[controller]")]
public abstract class ApiControllerBase : ControllerBase;
