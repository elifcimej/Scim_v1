using Microsoft.AspNetCore.Mvc;
using Scım_v1.Context;
using Scım_v1.Models;
using System.ComponentModel.DataAnnotations;
namespace Scım_v1.Controllers.Scım
{
    [ApiController]
    [Route("scim/v2")]
    public class ScımController : ControllerBase
    {
        private readonly UserDbContext _context;

        public ScımController(UserDbContext context)
        {
            _context = context;
        }

        [HttpPost("Users")]
        public IActionResult CreateUser([FromBody] ScimUserRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }
            var user = new User
            {
                UserName = request.userName.FirstOrDefault()?.value,
                FirstName = request.name?.givenName,
                LastName = request.name?.familyName,
                IsActive = request.active
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok(user);
        }

        [HttpGet("Users")]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();

            var scimUsers = users.Select(u => new
            {
                schemas = new[]
                {
                   "urn:ietf:params:scim:schemas:core:2.0:User"
                },
                id = u.Id.ToString(),
                userName = u.UserName,
                active = u.IsActive,
                name = new
                {
                    givenName = u.FirstName,
                    familyName = u.LastName,
                    formatted = $"{u.FirstName} {u.LastName}"
                },
                emails = new[]
                {
                    new{value = u.Email, primary = true}
                },
                meta = new
                {
                    resourceType = "User",
                    location = $"{Request.Scheme}://{Request.Host}/scim/v2/Users/{u.Id}"
                }
            }).ToList();

            return Ok(new
            {
                schemas = new[] { "urn:ietf:params:scim:api:messages:2.0:ListResponse" },
                Resources = scimUsers,
                totalResults = scimUsers.Count(),
                itemsPerPage = scimUsers.Count(),
                startIndex = 1,
            });
        }

        [HttpGet("Users/{id}")]
        public IActionResult GetUser(string id)
        {
            if (!int.TryParse(id, out int userId))
                return NotFound();

            var u = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (u == null)
                return NotFound(new
                {
                    schemas = new[] { "urn:ietf:params:scim:api:messages:2.0:Error" },
                    status = 404,
                    detail = $"User {id} not found"
                });

            return Ok(new
            {
                schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:User" },
                id = u.Id.ToString(),
                userName = u.UserName,
                active = u.IsActive,
                name = new
                {
                    givenName = u.FirstName,
                    familyName = u.LastName,
                    formatted = $"{u.FirstName} {u.LastName}"
                },
                emails = new[] { new { value = u.Email, primary = true } },
                meta = new
                {
                    resourceType = "User",
                    location = $"{Request.Scheme}://{Request.Host}/scim/v2/Users/{u.Id}"
                }
            });
        }

        [HttpPut("Users/{id}")]
        public IActionResult UpdateUser(string id, [FromBody] ScimUserRequest request)
        {
            if (!int.TryParse(id, out int userId))
                return NotFound();

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound(new
                {
                    schemas = new[] { "urn:ietf:params:scim:api:messages:2.0:Error" },
                    status = 404,
                    detail = $"User {id} not found"
                });

            user.UserName = request.userName?.FirstOrDefault()?.value ?? user.UserName;
            user.FirstName = request.name?.givenName ?? user.FirstName;
            user.LastName = request.name?.familyName ?? user.LastName;
            user.Email = request.emails?.FirstOrDefault()?.value ?? user.Email;
            user.IsActive = request.active;

            _context.SaveChanges();

            return Ok(new
            {
                schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:User" },
                id = user.Id.ToString(),
                userName = user.UserName,
                active = user.IsActive,
                name = new
                {
                    givenName = user.FirstName,
                    familyName = user.LastName,
                    formatted = $"{user.FirstName} {user.LastName}"
                },
                emails = new[] { new { value = user.Email, primary = true } },
                meta = new
                {
                    resourceType = "User",
                    location = $"{Request.Scheme}://{Request.Host}/scim/v2/Users/{user.Id}"
                }
            });
        }

        [HttpPatch("Users/{id}")]
        public IActionResult PatchUser(string id, [FromBody] ScimPatchRequest request)
        {
            if (!int.TryParse(id, out int userId))
                return NotFound();

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound(new
                {
                    schemas = new[] { "urn:ietf:params:scim:api:messages:2.0:Error" },
                    status = 404,
                    detail = $"User {id} not found"
                });

            foreach (var op in request.Operations)
            {
                if (op.op.ToLower() != "replace") continue;

                switch (op.path?.ToLower())
                {
                    case "active":
                        user.IsActive = Convert.ToBoolean(op.value);
                        break;
                    case "username":
                        user.UserName = op.value?.ToString();
                        break;
                    case "name.givenname":
                        user.FirstName = op.value?.ToString();
                        break;
                    case "name.familyname":
                        user.LastName = op.value?.ToString();
                        break;
                    case "emails[type eq \"work\"].value":
                    case "emails":
                        user.Email = op.value?.ToString();
                        break;
                }
            }

            _context.SaveChanges();

            return Ok(new
            {
                schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:User" },
                id = user.Id.ToString(),
                userName = user.UserName,
                active = user.IsActive,
                name = new
                {
                    givenName = user.FirstName,
                    familyName = user.LastName,
                    formatted = $"{user.FirstName} {user.LastName}"
                },
                emails = new[] { new { value = user.Email, primary = true } },
                meta = new
                {
                    resourceType = "User",
                    location = $"{Request.Scheme}://{Request.Host}/scim/v2/Users/{user.Id}"
                }
            });
        }

        [HttpDelete("Users/{id}")]
        public IActionResult DeleteUser(string id)
        {
            if (!int.TryParse(id, out int userId))
                return NotFound();

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound(new
                {
                    schemas = new[] { "urn:ietf:params:scim:api:messages:2.0:Error" },
                    status = 404,
                    detail = $"User {id} not found"
                });

            user.IsActive = false; // _context.Users.Remove(user); ile siledebilirim ama kullanıcıyı deaktif yapmak daha güvenli geldi
            _context.SaveChanges();
            return NoContent();
        }
    }


}
