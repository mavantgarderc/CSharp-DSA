using Csdsa.Domain.Interfaces;
using Csdsa.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Csdsa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArrayController : ControllerBase
{
    private readonly IArrayManipulationService _manipulationService;
    private readonly IArraySearchService _searchService;
    private readonly IArrayTransformationService _transformationService;

    public ArrayController(
        IArrayManipulationService manipulationService,
        IArraySearchService searchService,
        IArrayTransformationService transformationService)
    {
        _manipulationService = manipulationService;
        _searchService = searchService;
        _transformationService = transformationService;
    }

    [HttpPost("rotate-right")]
    public IActionResult RotateRight([FromBody] RotateRequest request)
    {
        try
        {
            var array = request.Array.ToArray(); // Create copy to avoid modifying input
            _manipulationService.RotateRight(array, request.Positions);
            return Ok(new { Result = array });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("rotate-left")]
    public IActionResult RotateLeft([FromBody] RotateRequest request)
    {
        try
        {
            var array = request.Array.ToArray();
            _manipulationService.RotateLeft(array, request.Positions);
            return Ok(new { Result = array });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("find-max")]
    public IActionResult FindMax([FromBody] int[] array)
    {
        try
        {
            var max = _searchService.FindMax(array);
            return Ok(new { Maximum = max });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("find-min")]
    public IActionResult FindMin([FromBody] int[] array)
    {
        try
        {
            var min = _searchService.FindMin(array);
            return Ok(new { Minimum = min });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("linear-search")]
    public IActionResult LinearSearch([FromBody] SearchRequest request)
    {
        try
        {
            var index = _searchService.LinearSearch(request.Array, request.Target);
            return Ok(new { Index = index, Found = index >= 0 });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("binary-search")]
    public IActionResult BinarySearch([FromBody] SearchRequest request)
    {
        try
        {
            var index = _searchService.BinarySearch(request.Array, request.Target);
            return Ok(new { Index = index, Found = index >= 0 });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("merge-sorted")]
    public IActionResult MergeSortedArrays([FromBody] MergeRequest request)
    {
        try
        {
            var result = _transformationService.MergeSortedArrays(request.ArrayA, request.ArrayB);
            return Ok(new { Result = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("dynamic-array-demo")]
    public IActionResult DynamicArrayDemo()
    {
        try
        {
            var dynamicArray = new DynamicArray<int>();
            dynamicArray.Add(1);
            dynamicArray.Add(2);
            dynamicArray.Add(3);
            dynamicArray.Insert(1, 99);

            return Ok(new
            {
                Array = dynamicArray.ToString(),
                Count = dynamicArray.Count,
                Capacity = dynamicArray.Capacity
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}

// Request DTOs
public record RotateRequest(int[] Array, int Positions);
public record SearchRequest(int[] Array, int Target);
public record MergeRequest(int[] ArrayA, int[] ArrayB);
