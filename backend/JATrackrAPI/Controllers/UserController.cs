using JATrackrAPI.Models;
using JATrackrAPI.Services;
using Microsoft.AspNetCore.Mvc;

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
    /// Single User object, if id matches with one existing in the collection. 
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         id = MongoDB ObjectID !
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
    /// Retrieve single user account by their username.
    /// </summary>
    /// <param name="username"></param>
    /// <returns>
    /// Single User object, if username matches with existing one in the collection. 
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Lorem ipsum.
    ///     </para>
    /// Sample request:
    ///
    ///     GET /api/Users/johnwickdoe
    ///
    /// </remarks>
    /// <response code="200">Returns the User object</response>
    /// <response code="404">If user with given ID not found in collection</response>
    /// <response code="500">If server encounters internal server error</response>
    [HttpGet("{username}/details", Name = "GetUserByUsername")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    // GET single user account by username
    // api/user/username
    public async Task<ActionResult<User>> GetUserByUsername(string username)
    {
        var user = await _userService.GetUserByUNAsync(username);
        if (user is null)
            return NotFound();
        return user;
    }

    /// <summary>
    /// Retrieve single user account by their email address.
    /// </summary>
    /// <param name="useremail"></param>
    /// <returns>
    /// Single User object, if email matches with existing one in the collection. 
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Lorem ipsum.
    ///     </para>
    /// Sample request:
    ///
    ///     GET /api/Users/johnwickdoe@thecontinental.org
    ///
    /// </remarks>
    /// <response code="200">Returns the User object</response>
    /// <response code="404">If user with given email address not found in collection</response>
    /// <response code="500">If server encounters internal server error</response>
    [HttpGet("{useremail}/details", Name = "GetUserByEmail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<User>> GetUserByEmail(string useremail)
    {
        var user = await _userService.GetUserByEmailAsync(useremail);
        if (user is null)
            return NotFound();
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
        await _userService.CreateUserAsync(newUser);

        // Create 201 OK Response with newly created User object, location header will point to this newly created resource 
        return CreatedAtAction(nameof(GetUserByID), new { id =  newUser.Id }, newUser);
    }

    /// <summary>
    /// Updates an existing User by their ID
    /// </summary>
    /// <param name="id"></param>
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
    /// <returns>
    /// </returns>
    /// <remarks>
    ///     <para>
    ///             Can be partial updated, only the changed fields that are received for the User will be modified, other fields will remain unchanged.
    ///     </para>
    /// Sample request:
    ///
    ///     PATCH /api/Users/johnwickdoe
    ///     {
    ///         "email": "nosequel"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Returns the newly created item</response>
    /// <response code="400">If the request is invalid or missing required fields</response>
    [HttpPut("{username}", Name = "UpdateUserByUsername")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUserByUsername(string username, User updatedUser)
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
    /// Deletes an existing User by their ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// </returns>
    /// <remarks>
    ///     <para>
    ///             F
    ///     </para>
    /// Sample request:
    ///
    ///     DELETE /api/Users/645419e2c6b366ba3639e928
    ///
    /// </remarks>
    /// <response code="204">no content</response>
    /// <response code="404">user with ID not found in collection/does not exist</response>
    [HttpDelete("{id:length(24)}", Name = "DeleteUserByID")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUserByID(string id)
    {
        var user = await _userService.GetUserByIDAsync(id);
        if (user is null)
        {
            return NotFound();
        }
        await _userService.DeleteUserByIDAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Deletes an existing User by their username
    /// </summary>
    /// <param name="username"></param>
    /// <returns>
    /// </returns>
    /// <remarks>
    ///     <para>
    ///             F
    ///     </para>
    /// Sample request:
    ///
    ///     DELETE /api/Users/winston
    ///
    /// </remarks>
    /// <response code="204">no content</response>
    /// <response code="404">user with ID not found in collection/does not exist</response>
    [HttpDelete("{username}", Name = "DeleteUserByUN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUserByUN(string username)
    {
        var user = await _userService.GetUserByUNAsync(username);
        if (user is null)
        {
            return NotFound();
        }
        await _userService.DeleteUserByUNAsync(username);
        return NoContent();
    }
}