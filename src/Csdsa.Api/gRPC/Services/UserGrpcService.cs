// using Csdsa.Application.Users.Queries.GetUserById;
// using Grpc.Contracts;
// using MediatR;
//
// public class UserGrpcService : UserService.UserServiceBase
// {
//     private readonly ISender _sender;
//
//     public UserGrpcService(ISender sender)
//     {
//         _sender = sender;
//     }
//
//     public override async Task<UserReply> GetUserById(UserRequest request, ServerCallContext context)
//     {
//         var user = await _sender.Send(new GetUserByIdQuery(Guid.Parse(request.Id)));
//         if (user is null)
//             throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
//
//         return new UserReply
//         {
//             Id = user.Id.ToString(),
//             Username = user.Username,
//             Email = user.Email
//         };
//     }
// }


// uncomment when ready to activate
