using Microsoft.AspNetCore.Mvc;
using Redis.Starter.Models;
using Redis.Starter.Services;
using System.Text.Json;

namespace Redis.Starter.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly ILogger<ValuesController> _logger;
    private readonly ICacheService _cacheService;

    public ValuesController(ILogger<ValuesController> logger, ICacheService cacheService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }
    // GET: api/<ValuesController>
    [HttpGet]
    public async Task<IResult> Get()
    {
        try
        {
            var result = await _cacheService.GetData<string>("redis");
            if(!result.HasValue)
            {
                return TypedResults.NotFound("No available data for key redis");
            }

            var data = JsonSerializer.Deserialize<Person>(result);
            return TypedResults.Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return TypedResults.BadRequest(ex.Message);
        }
    }

    // GET api/<ValuesController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<ValuesController>
    [HttpPost]
    public async Task<IResult> Post([FromBody] Person person)
    {
        try
        {
            var result = await _cacheService.SetData("redis", person);
            if(result)
            {
                return TypedResults.Ok("Saved succesfully");
            }

            return TypedResults.BadRequest("Could not save");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return TypedResults.BadRequest(ex.Message);
        }
    }

    // PUT api/<ValuesController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<ValuesController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
