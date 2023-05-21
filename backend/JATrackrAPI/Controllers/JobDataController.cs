using JATrackrAPI.Models;
using JATrackrAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace JATrackrAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobDataController : ControllerBase {
    
    private readonly UserService _userService;
    private readonly JobDataService _jobDataService;

    public JobDataController(UserService userService, JobDataService jobDataService)
    {
        _userService = userService;
        _jobDataService = jobDataService;
    }

    [HttpGet("jobs/all",Name = "GetAllJobData")]
    // GET list of all job applications, for all users, on the system
    // api/jobdata/jobs/all
    public async Task<List<JobData>> GetJobDatasAsync() =>
        await _jobDataService.GetAllJobAppsAsync();

    /*
    [HttpGet("jobapps/{userId:length(24)}", Name = "GetJobDataByUserID")]
    // GET list of all job applications, associated with a single User (by their userid)
    public async Task<ActionResult<List<JobData>>> GetJobDataForUserID(string id)
    {   
        return null;
    }
    */

    [HttpGet("users/{username}/jobapps",Name = "GetJobDataByUsername")]
    // GET list of all job applications associated with single User (by their userid)
    // api/users/{username}/jobapps
    public async Task<ActionResult<List<JobData>>> GetUserJobApps(string username) 
    {
        var userJobApps = await _jobDataService.GetJobApplicationsForUserUNAsync(username);
        if (userJobApps is null || userJobApps.Count.Equals(0))
            return NotFound();
        return userJobApps;
    }

    /*
    [HttpPost]
    public async Task<IActionResult> CreateJobData(JobData jobData)
    {
        await _jobDataService.CreateJobDataAsync(jobData);

        return CreatedAtRoute("GetJobData", new { id = jobData.Id }, jobData);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> UpdateJobData(string id, JobData updatedJobData)
    {
        var existingJobData = await _jobDataService.GetJobDataByIdAsync(id);
        if (existingJobData == null)
        {
            return NotFound();
        }

        await _jobDataService.UpdateJobDataAsync(id, updatedJobData);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> DeleteJobData(string id)
    {
        var jobData = await _jobDataService.GetJobDataByIdAsync(id);
        if (jobData == null)
        {
            return NotFound();
        }

        await _jobDataService.DeleteJobDataAsync(jobData.Id);

        return NoContent();
    }
    */
}