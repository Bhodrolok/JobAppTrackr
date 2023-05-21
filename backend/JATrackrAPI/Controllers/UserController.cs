using JATrackrAPI.Models;
using JATrackrAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;

namespace JATrackrAPI.Controllers;

// Attributes to define the User WebAPI controller
[ApiController]
// Route prefix: match to api/users; add 's' at end for Users not User...
[Route("api/[controller]s")]
public class UserController : ControllerBase 
{
    private readonly UserService _userService;
    private readonly JobDataService _jobDataService;

    public UserController(UserService userService, JobDataService jobDataService)
    {
        _userService = userService;
        _jobDataService = jobDataService;
    }
    // Define correct attributes to handle respective HTTP verbs (https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods) and dispatch to relevant executable endpoints

    /// <summary>
    /// Retrieve all registered users in the system 
    /// </summary>
    /// <returns>
    /// List of User objects, each representing a user account in the system. 
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Each User object contains: user's unique identifier (id), username, email address, and a list of job application document IDs associated with the user (to avoid embedding the whole thing!).
    ///         This endpoint does NOT require any parameters to be passed in the request.
    ///     </para>
    /// Sample request:
    ///
    ///     GET /api/Users
    ///
    /// </remarks>
    /// <response code="200">Returns list of user objects</response>
    /// <response code="500">If server encounters internal server error</response>
    [HttpGet(Name = "GetAllUsers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<List<User>> GetAllUsers() =>
        await _userService.GetAllUsersAsync();


    /// <summary>
    /// Retrieve single user account by their unique identifier (id).
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// Single User object, if the request id matches with one existing in the collection. 
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         id = MongoDB ObjectID ||
    ///         Needs to be 24 chars minimum --> ObjectID = 24 chars hex string / 12 bytes
    ///     </para>
    /// Sample request:
    ///
    ///     GET /api/Users/645419e2c6b366ba3639e928
    ///
    /// </remarks>
    /// <response code="200">Returns the user object </response>
    /// <response code="404">If user with given ID not found in collection</response>
    /// <response code="500">If server encounters internal server error</response>
    [HttpGet("{id:length(24)}", Name = "GetUserByID")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<User>> GetUserByID(string id)
    {
        var user = await _userService.GetUserByIDAsync(id);
        if (user is null)
            return NotFound();
        return user;
    }

    /// <summary>
    /// Retrieve single user account by their username, email address or both.
    /// </summary>
    /// <returns>
    /// Single User object, if username or email matches existing one in the collection. 
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Needs atleast one of the queries to be non null/empty.
    ///     </para>
    /// Sample request:
    ///
    ///     GET /api/Users/user?username=johnwickdoe
    ///
    ///     GET /api/Users/user?email=winston@thecontinental.org
    ///
    /// </remarks>
    [HttpGet("user")]
    public async Task<ActionResult<User>> GetUserSomehow(
        [FromQuery(Name = "username")] string username = null,
        [FromQuery(Name = "email")] string email = null
        )
    {
            if (username == null && email == null)
            {
                return BadRequest("At least one of `username` and `email` must be provided to locate the user account.");
            }

            User user;
            if (username != null)
            {
                user = await _userService.GetUserByUNAsync(username);
            }
            else if (email != null)
            {
                user = await _userService.GetUserByEmailAsync(email);
            }
            else
            {   
                user = await _userService.GetUserByUNAndEmailAsync(username, email);
            }

            if (user == null)
            {
                return NotFound();
            }
            
            return user;
    }

    /// <summary>
    /// Create new User object
    /// </summary>
    /// <param></param>
    /// <returns>
    /// Newly created User object, adds to the collection. 
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Username and email address fields are required and cannot be null. The (object)id will be auto generated when it is created anyways.
    ///     </para>
    /// Sample request:
    ///
    ///     POST /api/Users
    ///     {
    ///         "username": "winston",
    ///         "email": "winston@thecontinental.org"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created item</response>
    /// <response code="400">If the request is invalid or missing required fields</response>
    [HttpPost(Name = "CreateNewUser")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUser(User newUser)
    {
        // Basic conditional to check if there is an existing user with the same username (or email)
        if (await _userService.UserExists(newUser.Username, newUser.Email))
        {
            // Think its better to have the service do the checking and return just the boolean rather than
            // use something like GetUserByIDOrUNAsync here...
            return BadRequest("User with the same username or email already exists in the database!");
        }

        // As the UN/Email properties are both 'Required', controller 'automatically' validates that they are not empty; else BadRequest-ed
        await _userService.CreateUserAsync(newUser);

        // Create 201 OK Response with newly created User object, location header will point to this newly created resource 
        return CreatedAtAction(nameof(GetUserByID), new { id =  newUser.Id }, newUser);
    }

    /// <summary>
    /// Updates an existing User by their ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedUser"></param>
    /// <returns>
    /// </returns>
    /// <remarks>
    ///     <para>
    ///             Can be partial updated, only the changed fields that are received for the User will be modified, other fields will remain unchanged.
    ///     </para>
    /// Sample request:
    ///
    ///     PATCH /api/Users/645419e2c6b366ba3639e928
    ///     {
    ///         "username": "nosequel"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Returns the newly created item</response>
    /// <response code="400">If the request is invalid or missing required fields</response>
    // PUT = complete replace document; PATCH = modify existing (partial update; https://stackoverflow.com/a/30118175/21074625)
    [HttpPatch("{id:length(24)}", Name = "UpdateUserByID")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUserByID(string id, User updatedUser)
    {
        var user = await _userService.GetUserByIDAsync(id);
        if (user is null)
        {
            return NotFound();
        }
        updatedUser.Id = user.Id;

        await _userService.UpdateUserByIDAsync(id, updatedUser);

        return NoContent();
    }

    /// <summary>
    /// Updates an existing User by their username
    /// </summary>
    /// <param name="username"></param>
    /// <param name="updatedUser"></param>
    /// <returns>
    /// </returns>
    /// <remarks>
    ///     <para>
    ///             Can be partial updated, only the changed fields that are received for the User will be modified, other fields will remain unchanged.
    ///     </para>
    /// Sample request:
    ///
    ///     PATCH /api/Users/johnwickdoe
    ///     [
    ///         "email": "nosequel"
    ///     ]
    ///
    /// </remarks>
    /// <response code="200">Returns the newly created item</response>
    /// <response code="400">If the request is invalid or missing required fields</response>
    [HttpPatch("{username}", Name = "UpdateUserByUsername")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUserByUN(string username, User updatedUser)
    {
        var user = await _userService.GetUserByUNAsync(username);
        if (user is null)
        {
            return NotFound();
        }
        updatedUser.Id = user.Id;

        await _userService.UpdateUserByUNAsync(username, updatedUser);

        return NoContent();
    }


    /// <summary>
    /// Deletes an existing User by their ID, Username, or both
    /// </summary>
    /// <returns>
    /// </returns>
    /// <remarks>
    ///     <para>
    ///             F 
    ///     </para>
    /// Sample request:
    ///
    ///     DELETE /api/Users/user?id=645419e2c6b366ba3639e928
    ///
    ///     DELETE /api/Users/user?username=johnwickdoe
    ///
    /// </remarks>
    [HttpDelete("user")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<User>> DeleteUserSomehow(
        [FromQuery(Name = "id")] string id = null,
        [FromQuery(Name = "username")] string username = null
        )
    {
            if (id == null && username == null)
            {
                return BadRequest("Must provide atleast one of `id` and `username` to locate the user account!");
            }

            User user;
            if (username != null)
            {
                user = await _userService.GetUserByUNAsync(username);
                await _userService.DeleteUserByUNAsync(username);
            }
            else if (id != null)
            {
                user = await _userService.GetUserByIDAsync(id);
                await _userService.DeleteUserByIDAsync(id);
            }
            else
            {   
                user = await _userService.GetUserByIDAndUNAsync(id, username);
                await _userService.DeleteUserByIDAndUNAsync(id, username);
            }

            if (user == null)
            {
                return NotFound();
            }
            
            return NoContent();
    }

}