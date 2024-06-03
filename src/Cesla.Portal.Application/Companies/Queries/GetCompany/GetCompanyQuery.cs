using Cesla.Portal.Application.Dtos;
using ErrorOr;
using MediatR;

namespace Cesla.Portal.Application.Companies.Queries.GetCompany;
public record GetCompanyQuery : IRequest<ErrorOr<CompanyDto>>;
