using AutoMapper;
using Csdsa.Application.Services.EntityServices.Users.Requests;
using MediatR;
namespace Csdsa.Domain.Repository.IRepositories;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, bool>
{
    //private readonly IMapper _mapper;
    //private readonly IUnitOfWork _unitOfWork;
    //public CreateUserCommandHandler(IMapper mapper, IUnitOfWork unitOfWork)
    //{
    //    _mapper = mapper;
    //    _unitOfWork = unitOfWork;
    //}

    public Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
