// // namespace UniAPI.Controllers.EntityControllers
// // {
// //     [Route("api/[controller]")]
// //     [ApiController]
// //     public class UserController : ControllerBase
// //     {
//
// namespace Csdsa.Api.Controllers.LinearStructures;
//
// [ApiController]
// [Route("api/[controller]")]
// public class ArrayController : ControllerBase
// {
//     [HttpPost("rootate-right")]
//     public IActionResult RotateRigth([FromBody] RotateRequest request)
//     {
//         try
//         {
//             var array = (int[])request.Array.Clone();
//             Modules.DataStructures.Array.RotateRight(array, request.K);
//             return Ok(new ArrayWebApi.Models.ArrayResponse(array));
//         }
//         catch (Exception ex)
//         {
//             return BadRequest($"Error: {ex.message}");
//         }
//     }
// }
