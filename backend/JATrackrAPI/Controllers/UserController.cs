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
    ///     </para>
    ///     <para>
    ///     This endpoint does not require any parameters to be passed in the request.
    ///     </para>
    /// Sample request:
    ///
    ///     GET /api/Users
    ///
    /// </remarks>
    /// <response code="200">Returns the newly created item</response>
    /// <response code="400">If the item is null</response>
    [HttpGet(Name = "GetAllUsers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<List<User>> GetAllUsers() =>
        await _userService.GetAllUsersAsync();

    /// <summary>
    /// Retrieve all registered users in the system 
    /// </summary>
    /// <returns>
    /// List of User objects, each representing a user account in the system. 
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Each User object contains: user's unique identifier (id), username, email address, and a list of job application document IDs associated with the user (to avoid embedding the whole thing!).
    ///     </para>
    ///     <para>
    ///     This endpoint does not require any parameters to be passed in the request.
    ///     </para>
    /// Sample request:
    ///
    ///     GET /api/Users
    ///
    /// </remarks>
    /// <response code="200">Returns the newly created item</response>
    /// <response code="400">If the item is null</response>
    
    [HttpGet("{id:length(24)}", Name = "GetUserByID")]
    // GET single user account by id
    // 24 chars minimum --> ObjectID = 24 chars hex string / 12 bytes
    // api/user/{id}
    public async Task<ActionResult<User>> GetUserByID(string id)
    {
        var user = await _userService.GetUserByIDAsync(id);
        if (user is null)
            return NotFound();
        return user;
    }

    /// <summary>
    /// Retrieve all registered users in the system 
    /// </summary>
    /// <returns>
    /// List of User objects, each representing a user account in the system. 
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Each User object contains: user's unique identifier (id), username, email address, and a list of job application document IDs associated with the user (to avoid embedding the whole thing!).
    ///     </para>
    ///     <para>
    ///     This endpoint does not require any parameters to be passed in the request.
    ///     </para>
    /// Sample request:
    ///
    ///     GET /api/Users
    ///
    /// </remarks>
    /// <response code="200">Returns the newly created item</response>
    /// <response code="400">If the item is null</response>
    [HttpGet("{username}/details", Name = "GetUserByUsername")]
    // GET single user account by username
    // api/user/username
    public async Task<ActionResult<User>> GetUserUN(string username)
    {
        var user = await _userService.GetUserByUNAsync(username);
        if (user is null)
            return NotFound();
        return user;
    }

    /// <summary>
    /// Retrieve all registered users in the system 
    /// </summary>
    /// <returns>
    /// List of User objects, each representing a user account in the system. 
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Each User object contains: user's unique identifier (id), username, email address, and a list of job application document IDs associated with the user (to avoid embedding the whole thing!).
    ///     </para>
    ///     <para>
    ///     This endpoint does not require any parameters to be passed in the request.
    ///     </para>
    /// Sample request:
    ///
    ///     GET /api/Users
    ///
    /// </remarks>
    /// <response code="200">Returns the newly created item</response>
    /// <response code="400">If the item is null</response>

    [HttpGet("{useremail}/details", Name = "GetUserByEmail")]
    // GET single user account by email
    // api/user/useremail
    public async Task<ActionResult<User>> GetUserEmail(string useremail)
    {
        var user = await _userService.GetUserByEmailAsync(useremail);
        if (user is null)
            return NotFound();
        return user;
    }

    /// <summary>
    /// Retrieve all registered users in the system 
    /// </summary>
    /// <returns>
    /// List of User objects, each representing a user account in the system. 
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Each User object contains: user's unique identifier (id), username, email address, and a list of job application document IDs associated with the user (to avoid embedding the whole thing!).
    ///     </para>
    ///     <para>
    ///     This endpoint does not require any parameters to be passed in the request.
    ///     </para>
    /// Sample request:
    ///
    ///     GET /api/Users
    ///
    /// </remarks>
    /// <response code="200">Returns the newly created item</response>
    /// <response code="400">If the item is null</response>

    [HttpPost(Name = "CreateNewUser")]
    // POST a single new user account with all necessary data 
    // api/user
    public async Task<IActionResult> CreateUser(User newUser)
    {
        await _userService.CreateUserAsync(newUser);

        // Create 201 OK Response with newly created User object, location header will point to this newly created resource 
        return CreatedAtAction(nameof(GetUserByID), new { id =  newUser.Id }, newUser);
    }

    /// <summary>
    /// Retrieve all registered users in the system 
    /// </summary>
    /// <returns>
    /// List of User objects, each representing a user account in the system. 
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Each User object contains: user's unique identifier (id), username, email address, and a list of job application document IDs associated with the user (to avoid embedding the whole thing!).
    ///     </para>
    ///     <para>
    ///     This endpoint does not require any parameters to be passed in the request.
    ///     </para>
    /// Sample request:
    ///
    ///     GET /api/Users
    ///
    /// </remarks>
    /// <response code="200">Returns the newly created item</response>
    /// <response code="400">If the item is null</response>
    [HttpPut("{id:length(24)}", Name = "UpdateUserByID")]
    // UPDATE single existing user account, filtered by ID
    // api/user/id
    public async Task<IActionResult> UpdateUser_ID(string id, User updatedUser)
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
    /// Retrieve all registered users in the system 
    /// </summary>
    /// <returns>
    /// List of User objects, each representing a user account in the system. 
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Each User object contains: user's unique identifier (id), username, email address, and a list of job application document IDs associated with the user (to avoid embedding the whole thing!).
    ///     </para>
    ///     <para>
    ///     This endpoint does not require any parameters to be passed in the request.
    ///     </para>
    /// Sample request:
    ///
    ///     GET /api/Users
    ///
    /// </remarks>
    /// <response code="200">Returns the newly created item</response>
    /// <response code="400">If the item is null</response>
    [HttpPut("{username}", Name = "UpdateUserByUsername")]
    // UPDATE single existing user account, filtered by Username
    // api/user/username 
    public async Task<IActionResult> UpdateUser_UN(string username, User updatedUser)
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
    /// Retrieve all registered users in the system 
    /// </summary>
    /// <returns>
    /// List of User objects, each representing a user account in the system. 
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Each User object contains: user's unique identifier (id), username, email address, and a list of job application document IDs associated with the user (to avoid embedding the whole thing!).
    ///     </para>
    ///     <para>
    ///     This endpoint does not require any parameters to be passed in the request.
    ///     </para>
    /// Sample request:
    ///
    ///     GET /api/Users
    ///
    /// </remarks>
    /// <response code="200">Returns the newly created item</response>
    /// <response code="400">If the item is null</response>
    [HttpDelete("{id:length(24)}", Name = "DeleteUserByID")]
    // DELETE single existing user account, found using id
    // api/user/id
    public async Task<IActionResult> DeleteUser_ID(string id)
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
    /// Retrieve all registered users in the system 
    /// </summary>
    /// <returns>
    /// List of User objects, each representing a user account in the system. 
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Each User object contains: user's unique identifier (id), username, email address, and a list of job application document IDs associated with the user (to avoid embedding the whole thing!).
    ///     </para>
    ///     <para>
    ///     This endpoint does not require any parameters to be passed in the request.
    ///     </para>
    /// Sample request:
    ///
    ///     GET /api/Users
    ///
    /// </remarks>
    /// <response code="200">Returns the newly created item</response>
    /// <response code="400">If the item is null</response>
    [HttpDelete("{username}", Name = "DeleteUserByUN")]
    // DELETE single existing user account, found using id
    // api/user/username
    public async Task<IActionResult> DeleteUser_UN(string username)
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